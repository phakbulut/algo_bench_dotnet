from selenium import webdriver
from selenium.webdriver.chrome.options import Options
from webdriver_manager.chrome import ChromeDriverManager
import time
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import Select

# Chrome için tarayıcı ayarlarını yapılandırıyoruz
options = Options()
options.binary_location = r"C:\Program Files\Google\Chrome\Application\chrome.exe"  # Chrome'un yüklü olduğu yol

# WebDriver Manager ile ChromeDriver'ı indirip başlatıyoruz
driver = webdriver.Chrome(
    options=options  # Tarayıcı seçenekleri
)

# WebDriver ile işlemler yapılabilir
driver.get("http://localhost:5000")
driver.refresh()  # Sayfayı bir kere yeniliyoruz

# Kombinasyonları tanımlama
algorithms = ["quick", "heap", "shell", "radix", "merge"]
data_sizes = ["1000", "10000", "100000"]
orders = ["random", "partially sorted", "reverse"]

# Her kombinasyon için döngü
for _ in range(50):  # 50 kez tekrar edilecek
    for algorithm in algorithms:
        for data_size in data_sizes:
            for order in orders:
                try:
                    # Sayfayı yenileyelim
                    driver.refresh()  # Sayfa her açıldığında bir kere yenilenecek
                    driver.refresh()  # Sayfa her açıldığında bir kere yenilenecek

                    # Algoritmayı seç
                    algorithm_select = Select(driver.find_element(By.ID, "algorithm"))
                    algorithm_select.select_by_value(algorithm)

                    # Veri boyutunu seç
                    data_size_select = Select(driver.find_element(By.ID, "dataSize"))
                    data_size_select.select_by_value(data_size)

                    # Sıralama türünü seç
                    order_select = Select(driver.find_element(By.ID, "order"))
                    order_select.select_by_value(order)

                    # Formu gönder
                    submit_button = driver.find_element(By.CLASS_NAME, "btn-run")
                    submit_button.click()

                    # Çıktıyı bekle (Sayfa yüklenmesini beklemek için süreyi ayarlayabilirsin)
                    time.sleep(1)  # Çıkış sayfası yüklendikten sonra bekleme süresi

                    # Geri dön
                    driver.back()  # Geri düğmesi simülasyonu
                    time.sleep(2)  # Sayfanın yeniden yüklenmesini bekle

                except Exception as e:
                    print(f"Hata oluştu: {e}")

# İşlem tamamlandıktan sonra tarayıcıyı kapat
driver.quit()
