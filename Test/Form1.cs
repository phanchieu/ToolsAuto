using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Security.Policy;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void BtnTestProfile_Click(object sender, EventArgs e)
        {
            string chromeProfilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Google",
                "Chrome",
                "User Data",
                "Profiles"
            );
            string profileBasePath = "Profiles"; // Thay thế bằng đường dẫn thực tế của bạn
            string nameProfile = "Test";
            if (!Directory.Exists(chromeProfilePath))
            {
                Directory.CreateDirectory(chromeProfilePath);
            }
            // Đường dẫn đến ChromeDriver
            string chromedriverPath = "chromedriver.exe"; // Thay thế bằng đường dẫn thực tế của bạn

            // Khởi tạo ChromeDriverService
            ChromeOptions options = new ChromeOptions();
            options.AddArgument($"user-data-dir={chromeProfilePath + '/'+nameProfile}");
            ChromeDriverService chromeService = ChromeDriverService.CreateDefaultService(chromedriverPath);
            chromeService.HideCommandPromptWindow = true;
            // %APPDATA% -> \Local\Google\Chrome\Application\Profiles
            IWebDriver driver = new ChromeDriver(chromeService, options);
            driver.Navigate().GoToUrl("Shopee.vn");
        }
    }
}