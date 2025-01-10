using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using System.Xml;
using Newtonsoft.Json;
using algo.Data;
using Microsoft.EntityFrameworkCore;
using AlgorithmProject.Data;

namespace AlgorithmProject.Controllers
{
    public class ProcessController : Controller
    {
        // GET: Process
        private readonly ApplicationDbContext _context; // _context burada sınıf seviyesinde tanımlandı
        private readonly List<AlgorithmLog> _logQueue; // Kuyruğumuzu bir liste olarak tanımlıyoruz
        private readonly int _batchSize = 5; // Veritabanına gönderilecek toplu veri sayısı

        public ActionResult Index()
        {
            return View();
        }

        private double GetCpuUsage()
        {
            using (var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total"))
            {
                cpuCounter.NextValue();  // İlk okuma (gereksiz, sadece bir başlangıç okumadır)
                Thread.Sleep(100);  // 1 saniye bekle
                return cpuCounter.NextValue();  // Gerçek CPU kullanımı
            }
        }

        public ProcessController(ApplicationDbContext context)
        {
            _context = context;
            _logQueue = new List<AlgorithmLog>();

        }
        // POST: Process/Execute
        [HttpPost]
        public async Task<ActionResult> Execute(string algorithm, int dataSize, string order)
        {
            // Veri oluşturma
            int[] data = GenerateData(dataSize, order);
            int[] originalData = (int[])data.Clone();

            // Kaynak kullanımı takibi
            List<(double Time, double MemoryMB, double Cpu)> resourceUsage = new();
            var stopwatch = Stopwatch.StartNew();

            // Kaynak kullanımını kaydetmek için bir görev başlatıyoruz.
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            // Asenkron görevi başlatıyoruz
            var monitoringTask = Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    // Hafıza ve CPU kullanımını al
                    var memoryUsageBytes = GC.GetTotalMemory(false);
                    var memoryUsageMB = memoryUsageBytes / (1024.0 * 1024.0);  // MB cinsinden
                    var cpuUsage = GetCpuUsage();

                    // Logu kaydet
                    resourceUsage.Add((stopwatch.Elapsed.TotalSeconds, memoryUsageMB, cpuUsage));

                    // 1 ms bekle
                    await Task.Delay(100); // 1 ms bekleme
                }
            }, token);

            // Algoritma çalıştırma
            switch (algorithm.ToLower())
            {
                case "quick":
                    QuickSort(data, 0, data.Length - 1);
                    break;
                case "heap":
                    HeapSort(data);
                    break;
                case "shell":
                    ShellSort(data);
                    break;
                case "radix":
                    RadixSort(data);
                    break;
                case "merge":
                    MergeSort(data, 0, data.Length - 1);
                    break;
                default:
                    return BadRequest("Geçersiz algoritma seçimi.");
            }

            // Ölçümleri durdur
            stopwatch.Stop();
            cancellationTokenSource.Cancel(); // Timer'ı durdur

            // Görev tamamlanana kadar bekleyin
            await monitoringTask;

            // Ortalama kaynak kullanımını hesapla
            var averageMemory = resourceUsage.Any() ? resourceUsage.Average(r => r.MemoryMB) : 0;
            var averageCpu = resourceUsage.Any() ? resourceUsage.Average(r => r.Cpu) : 0;
            var averageTime = resourceUsage.Any() ? resourceUsage.Average(r => r.Time) : 0;

            // Veritabanına kaydetmek için yeni bir log oluşturuyoruz
            var algorithmLog = new AlgorithmLog
            {
                Algorithm = algorithm,
                ArrayType = order,
                ArraySize = dataSize,
                TimeTaken = stopwatch.Elapsed.TotalMilliseconds,
                AverageMemory = averageMemory,
                AverageCpu = averageCpu,
                AverageTime = averageTime,
            };

            _context.AlgorithmLogs.Add(algorithmLog);
            await _context.SaveChangesAsync();

            ViewBag.Algorithm = algorithm;
            ViewBag.ArrayType = order;
            ViewBag.ArraySize = dataSize;
            ViewBag.TimeTaken = stopwatch.Elapsed.TotalMilliseconds;
            ViewBag.OriginalArray = originalData;
            ViewBag.SortedArray = data;
            ViewBag.ResourceUsage = resourceUsage;
            ViewBag.AverageMemory = averageMemory;
            ViewBag.AverageCpu = averageCpu;
            ViewBag.AverageTime = averageTime;

            return View("Result");
        }
        private int[] GenerateData(int size, string order)
        {
            if (size <= 0)
            {
                throw new Exception("Dizi boyutu sıfır veya negatif olamaz.");
            }

            int maxValue = size;
            int[] data = new int[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = i + 1; // 1'den başla, size kadar devam et
            }

            // Diziyi sıralama işlemi
            switch (order.ToLower())
            {
                case "random":
                    ShuffleArray(data);
                    break;

                case "partially sorted":
                    int middleIndex = size / 2;
                    Array.Sort(data, 0, middleIndex);
                    ShuffleArray(data, middleIndex, size);
                    break;

                case "reverse":
                    Array.Reverse(data);
                    break;

                default:
                    throw new Exception("Geçersiz diziliş tipi.");
            }

            return data;
        }

        // Yardımcı fonksiyonlar
        private void ShuffleArray(int[] array, int start = 0, int end = -1)
        {
            Random rand = new Random();
            if (end == -1)
            {
                end = array.Length;
            }

            for (int i = start; i < end; i++)
            {
                int j = rand.Next(i, end);
                int temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
        }

        // Algoritmalar
        private void QuickSort(int[] arr, int low, int high)
        {
            while (low < high)
            {
                // Pivot seçimi ve bölme
                int pi = Partition(arr, low, high);

                // Daha küçük olan alt aralık üzerinde tekrar QuickSort yap
                if (pi - low < high - pi)
                {
                    QuickSort(arr, low, pi - 1);
                    low = pi + 1; // Sol taraf bitti, sağ taraf için devam
                }
                else
                {
                    QuickSort(arr, pi + 1, high);
                    high = pi - 1; // Sağ taraf bitti, sol taraf için devam
                }
            }
        }

        private int Partition(int[] arr, int low, int high)
        {
            int pivot = arr[high]; // Sağdaki elemanı pivot seç
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                if (arr[j] <= pivot)
                {
                    i++;
                    Swap(ref arr[i], ref arr[j]); // Küçük elemanları sola taşı
                }
            }

            Swap(ref arr[i + 1], ref arr[high]); // Pivot elemanını doğru yerine taşı
            return i + 1;
        }

        private void Swap(ref int a, ref int b)
        {
            int temp = a;
            a = b;
            b = temp;
        }

        private void HeapSort(int[] arr)
        {
            int n = arr.Length;
            for (int i = n / 2 - 1; i >= 0; i--)
                Heapify(arr, n, i);
            for (int i = n - 1; i > 0; i--)
            {
                Swap(ref arr[0], ref arr[i]);
                Heapify(arr, i, 0);
            }
        }

        private void Heapify(int[] arr, int n, int i)
        {
            int largest = i;
            int left = 2 * i + 1;
            int right = 2 * i + 2;
            if (left < n && arr[left] > arr[largest])
                largest = left;
            if (right < n && arr[right] > arr[largest])
                largest = right;
            if (largest != i)
            {
                Swap(ref arr[i], ref arr[largest]);
                Heapify(arr, n, largest);
            }
        }

        private void ShellSort(int[] arr)
        {
            int n = arr.Length;
            for (int gap = n / 2; gap > 0; gap /= 2)
            {
                for (int i = gap; i < n; i++)
                {
                    int temp = arr[i];
                    int j;
                    for (j = i; j >= gap && arr[j - gap] > temp; j -= gap)
                        arr[j] = arr[j - gap];
                    arr[j] = temp;
                }
            }
        }

        private void RadixSort(int[] arr)
        {
            int m = arr.Max();
            for (int exp = 1; m / exp > 0; exp *= 10)
                CountSort(arr, exp);
        }

        private void CountSort(int[] arr, int exp)
        {
            int n = arr.Length;
            int[] output = new int[n];
            int[] count = new int[10];

            for (int i = 0; i < n; i++)
                count[(arr[i] / exp) % 10]++;

            for (int i = 1; i < 10; i++)
                count[i] += count[i - 1];

            for (int i = n - 1; i >= 0; i--)
            {
                output[count[(arr[i] / exp) % 10] - 1] = arr[i];
                count[(arr[i] / exp) % 10]--;
            }

            for (int i = 0; i < n; i++)
                arr[i] = output[i];
        }

        private void MergeSort(int[] arr, int left, int right)
        {
            if (left < right)
            {
                int mid = (left + right) / 2;
                MergeSort(arr, left, mid);
                MergeSort(arr, mid + 1, right);
                Merge(arr, left, mid, right);
            }
        }

        private void Merge(int[] arr, int left, int mid, int right)
        {
            int n1 = mid - left + 1;
            int n2 = right - mid;

            int[] leftArray = new int[n1];
            int[] rightArray = new int[n2];

            for (int i = 0; i < n1; i++)
                leftArray[i] = arr[left + i];
            for (int j = 0; j < n2; j++)
                rightArray[j] = arr[mid + 1 + j];

            int k = left, i1 = 0, i2 = 0;
            while (i1 < n1 && i2 < n2)
            {
                if (leftArray[i1] <= rightArray[i2])
                    arr[k++] = leftArray[i1++];
                else
                    arr[k++] = rightArray[i2++];
            }

            while (i1 < n1)
                arr[k++] = leftArray[i1++];
            while (i2 < n2)
                arr[k++] = rightArray[i2++];
        }




        [HttpPost]
        public async Task<ActionResult> RunAllCombinations(int runCount)
        {
            var algorithms = new[] { "heap", "shell", "radix", "merge" };
            var dataSizes = new[] { 1000, 10000, 100000 };
            var orders = new[] { "random", "partially sorted", "reverse" };

            var Results = new List<AlgorithmLog>();
            var batchSize = 10;  // Kuyruğa eklenecek veri sayısı (batch size)

            // En son çalıştırmaların sonuçlarını saklayacağız
            for (int i = 0; i < runCount; i++)
            {
                foreach (var algorithm in algorithms)
                {
                    foreach (var dataSize in dataSizes)
                    {
                        foreach (var order in orders)
                        {
                            var result = ExecuteSingleCombination(algorithm, dataSize, order);
                            Results.Add(result); // En son yapılan çalıştırmaları sakla

                            // Kuyruğa ekledikçe belirli bir büyüklüğe ulaştığında veritabanına ekleyelim
                            if (Results.Count >= batchSize)
                            {
                                // Veritabanına veri eklemek için ProcessQueue'i çağırıyoruz.
                                await ProcessQueue(Results);
                                Results.Clear();  // Kuyruğu temizliyoruz
                            }
                        }
                    }
                }
            }

            // Kuyruğun sonunda kalan verileri kaydet
            if (Results.Count > 0)
            {
                await ProcessQueue(Results);  // Son kalan veriyi kaydediyoruz
            }

            // Sonuçları View'a gönderiyoruz
            ViewBag.LatestResults = Results;
            return View("Result2");
        }

        // Kuyruğu işleyip veritabanına topluca ekler
        private async Task ProcessQueue(List<AlgorithmLog> results)
        {
            try
            {
                if (results.Any())
                {
                    _context.AlgorithmLogs.AddRange(results);  // Toplu insert
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda, loglama yapabilirsiniz
                Console.WriteLine($"Veritabanına veri eklerken hata oluştu: {ex.Message}");
            }
        }
        // Kuyruğu işleyip veritabanına topluca ekler

        private AlgorithmLog ExecuteSingleCombination(string algorithm, int dataSize, string order)
        {
            int[] data = GenerateData(dataSize, order);
            int[] originalData = (int[])data.Clone();

            List<(double Time, double MemoryMB, double Cpu)> resourceUsage = new();
            var stopwatch = Stopwatch.StartNew();

            // Algoritma çalıştırma
            switch (algorithm.ToLower())
            {
                case "quick":
                    QuickSort(data, 0, data.Length - 1);
                    break;
                case "heap":
                    HeapSort(data);
                    break;
                case "shell":
                    ShellSort(data);
                    break;
                case "radix":
                    RadixSort(data);
                    break;
                case "merge":
                    MergeSort(data, 0, data.Length - 1);
                    break;
                default:
                    throw new Exception("Geçersiz algoritma seçimi.");
            }

            stopwatch.Stop();
            // Ortalama kaynak kullanımını hesapla
            var averageMemory = resourceUsage.Any() ? Math.Round(resourceUsage.Average(r => r.MemoryMB), 2) : 0;
            var averageCpu = resourceUsage.Any() ? Math.Round(resourceUsage.Average(r => r.Cpu), 2) : 0;
            var averageTime = resourceUsage.Any() ? Math.Round(resourceUsage.Average(r => r.Time), 2) : 0;


            var algorithmLog = new AlgorithmLog
            {
                Algorithm = algorithm,
                ArrayType = order,
                ArraySize = dataSize,
                TimeTaken = stopwatch.Elapsed.TotalMilliseconds,
                AverageMemory = averageMemory,
                AverageCpu = averageCpu,
                AverageTime = averageTime,
            };

            return algorithmLog;
        }

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> Rapor()
        {
            // Tüm verileri çekiyoruz
            var logs = await _context.AlgorithmLogs.ToListAsync();

            // Verileri sınıflandırıyoruz
            var groupedData = logs
                .GroupBy(log => log.Algorithm)
                .Select(algorithmGroup => new
                {
                    Algorithm = algorithmGroup.Key,
                    ArrayTypes = algorithmGroup
                        .GroupBy(log => log.ArrayType)
                        .Select(arrayTypeGroup => new
                        {
                            ArrayType = arrayTypeGroup.Key,
                            Sizes = arrayTypeGroup
                                .GroupBy(log => log.ArraySize)
                                .Select(sizeGroup => new
                                {
                                    ArraySize = sizeGroup.Key,
                                    Data = sizeGroup.Select(log => new
                                    {
                                        log.TimeTaken,
                                        log.AverageMemory,
                                        log.AverageCpu
                                    }).ToList()
                                }).ToList()
                        }).ToList()
                }).ToList();

            // Veriyi ViewBag ile View'a gönderiyoruz
            ViewBag.GroupedData = groupedData;

            return View("Rapor");
        }


    }
}
