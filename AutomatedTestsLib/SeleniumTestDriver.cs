using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;

namespace AutomatedTestsLib
{
    public class SeleniumTestDriver
    {
        private static IWebDriver driver;
        private StringBuilder verificationErrors;
        private bool acceptNextAlert = true;
        
        public enum DriverType
        {
            FireFox = 1,
            GoogleChrome = 2,
            IE = 3
        }
        public SeleniumTestDriver(DriverType driverType)
        {
            InitDriverByType(driverType);
            driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(15);
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(120);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
            driver.Manage().Window.Maximize();
        }

        private void InitDriverByType(DriverType driverType)
        {
            switch (driverType)
            {
                case DriverType.FireFox: {
                        FirefoxOptions options = new FirefoxOptions();
                        options.AcceptInsecureCertificates = true;
                        FirefoxProfile profile = new FirefoxProfile();
                        profile.AcceptUntrustedCertificates = true;
                        profile.AssumeUntrustedCertificateIssuer = true;
                        options.Profile = profile;
                        FirefoxDriverService service = FirefoxDriverService.CreateDefaultService(@"C:\Selenium");
                        service.FirefoxBinaryPath = @"C:\Program Files\Mozilla Firefox\firefox.exe";
                        driver = new FirefoxDriver(service, options, TimeSpan.FromSeconds(180));
                        break;
                    }
                case DriverType.GoogleChrome:
                    {
                        ChromeOptions options = new ChromeOptions();
                        options.AcceptInsecureCertificates = true;
                        options.AddArgument("headless");
                        options.AddArgument("no-sandbox");
                        options.AddArgument("proxy-server='direct://'");
                        options.AddArgument("proxy-bypass-list=*");
                        driver = new ChromeDriver(@"C:\Selenium");
                        break;
                    }
                case DriverType.IE:
                    {
                        InternetExplorerOptions options = new InternetExplorerOptions();
                        options.IgnoreZoomLevel = true;
                        //options.EnsureCleanSession = true;
                        options.EnableNativeEvents = true;
                        //options.EnablePersistentHover = true;
                        options.RequireWindowFocus = true;
                        options.BrowserAttachTimeout = TimeSpan.FromSeconds(120);
                        options.IntroduceInstabilityByIgnoringProtectedModeSettings = true;
                        driver = new InternetExplorerDriver(@"C:\Selenium", options);
                        break;
                    }
                default: {
                        throw (new Exception("Not applicable driver"));
                    }
            }
        }

        public static void CleanupClass()
        {
            try
            {
                //driver.Quit();// quit does not close the window
                driver.Close();
                driver.Dispose();
            }
            catch (Exception)
            {
                // Ignore errors if unable to close the browser
            }
        }

        public void InitializeTest()
        {
            verificationErrors = new StringBuilder();
        }

        public string RunTestCase()
        {
            string Result = "Success";
            try
            {
                driver.Navigate().GoToUrl("https://www.electrolux.net/UK/Login.aspx");
                driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_txtPassWord")).Clear();
                driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_txtPassWord")).SendKeys("hk112");
                driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_txtLogin")).Clear();
                driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_txtLogin")).SendKeys("hk_uk_p_50065931");
                driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_ButtonB2B_Enter")).Click();
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
                wait.Until(d => d.Title.StartsWith("Electrolux Home Product", StringComparison.OrdinalIgnoreCase));
                System.Threading.Thread.Sleep(1000);
                while (IsAlertPresent())
                {
                    CloseAlertAndGetItsText();
                }
                WaitForPageLoad();
                driver.FindElement(By.LinkText("Products")).Click();
                WaitForPageLoad();
                driver.FindElement(By.Id("ctl00_ddlDeliveryAddressHidden")).Click();
                new SelectElement(driver.FindElement(By.Id("ctl00_ddlDeliveryAddressHidden"))).SelectByText("50065931 Ultima Furniture Systems LTD Lidgate Crescent WF9 3NR Pontefract");
                driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Select an address'])[2]/following::option[2]")).Click();
                driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Postal Code'])[3]/following::button[1]")).Click();
                System.Threading.Thread.Sleep(1000);
                driver.FindElement(By.Id("ctl00_lkbLogOff")).Click();
                WaitForPageLoad();
                driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_lnkLogout")).Click();
                CleanupClass();
            }
            catch(Exception ex)
            {
                Result = "Error | " + ex.Message ?? "";
            }
            return Result;
        }
        private bool IsElementPresent(By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        private bool IsAlertPresent()
        {
            try
            {
                driver.SwitchTo().Alert();
                return true;
            }
            catch (NoAlertPresentException)
            {
                return false;
            }
        }

        private void WaitForPageLoad()
        {
            WebDriverWait wait = wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
            wait.Until(jsExec => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        }

        private string CloseAlertAndGetItsText()
        {
            try
            {
                IAlert alert = driver.SwitchTo().Alert();
                string alertText = alert.Text;
                if (acceptNextAlert)
                {
                    alert.Accept();
                }
                else
                {
                    alert.Dismiss();
                }
                return alertText;
            }
            finally
            {
                acceptNextAlert = true;
            }
        }
    }
}
