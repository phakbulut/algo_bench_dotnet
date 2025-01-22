from selenium import webdriver
from selenium.webdriver.chrome.options import Options
from webdriver_manager.chrome import ChromeDriverManager
import time
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import Select

options = Options()
options.binary_location = r"C:\Program Files\Google\Chrome\Application\chrome.exe"  # Chrome'un yüklü olduğu yol

driver = webdriver.Chrome(
    options=options  
)

driver.get("http://localhost:5000")
driver.refresh() 

algorithms = ["quick", "heap", "shell", "radix", "merge"]
data_sizes = ["1000", "10000", "100000"]
orders = ["random", "partially sorted", "reverse"]

for _ in range(500):  
    for algorithm in algorithms:
        for data_size in data_sizes:
            for order in orders:
                try:
                    driver.refresh() 
                    driver.refresh()  

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

                    time.sleep(1)  # Çıkış sayfası yüklendikten sonra bekleme süresi

                    # Geri dön
                    driver.back()  # Geri düğmesi simülasyonu
                    time.sleep(2)  # Sayfanın yeniden yüklenmesini bekle

                except Exception as e:
                    print(f"Hata oluştu: {e}")

driver.quit()
