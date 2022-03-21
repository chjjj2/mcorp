using AventStack.ExtentReports;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using UITest.Core.LoggingAndReporting;


namespace UITest
{
    public abstract class BasePage
    {

        public BasePage(IWebDriver driver, Tests.TestContext context)
        {

        }

        public IWebElement WaitForElementToBeVisible(string key, int timeout = 60)
        {
            var wait = new WebDriverWait(this.WebDriver, new TimeSpan(0, 0, timeout));
            try
            {
                return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath(key)));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ReadOnlyCollection<IWebElement> WaitForElementsToBeVisible(string key, int timeout = 30)
        {
            WaitForElementToBeVisible(key);
            var elements = WebDriver.FindElements(By.XPath(key));
            return elements.Count < 1 ? null : elements;
        }

        public IWebElement TryFindElement(string key, string elementDescription, ExtentTest logger, int timeout = 30, bool silent = false)
        {
            var element = WaitForElementToBeVisible(key, timeout);

            if (element != null)
            {
                logger.Log(Status.Pass, $"Successfully located the {elementDescription}.");
                WebDriver.MoveToElement(element);
                return element;
            }
            else
            {
                logger.Log(Status.Fail, $"Failed to locate the {elementDescription} by using this XPath:<pre>{key}</pre>");
                Screenshot(logger);
                if (silent)
                    logger.Log(Status.Info, $"Did not locate the {elementDescription}, but the script will continue.");
                else
                    throw new NoSuchElementException($"Failed to locate the {elementDescription}.");
            }
            return null;
        }

        public IWebElement TryFindElementAndClick(string key, string elementDescription, ExtentTest logger, int timeout = 30, bool silent = false)
        {
            var element = TryFindElement(key, elementDescription, logger, timeout);
            if (element != null)
            {
                Screenshot(logger);
                int i = 0;
                while (true)
                {
                    i++;
                    try
                    {
                        element.Click();
                        break;
                    }
                    catch (ElementClickInterceptedException) when (i < 10)
                    {
                        Thread.Sleep(1000);
                    }
                }

                logger.Log(Status.Pass, $"Successfully clicked on the {elementDescription}.");
                return element;
            }
            else
            {
                Screenshot(logger);
                if (silent)
                    logger.Log(Status.Info, $"Failed to click on the {elementDescription}, but the script will continue.");
                else
                    throw new NoSuchElementException($"Failed to click on the {elementDescription}.");
            }

            return null;
        }

        public IWebElement TryFindElementAndEnterData(string key, string data, string elementDescription, ExtentTest logger, int timeout = 30)
        {
            var element = TryFindElement(key, elementDescription, logger, timeout);

            if (element != null)
            {
                WebDriver.MoveToElement(element);
                element.Clear();
                element.SendKeys(data);
                logger.Log(Status.Pass, $"Successfully entered the value '{data}' into the {elementDescription}.");
                return element;
            }
            else
            {
                logger.Log(Status.Fail, $"Failed to enter the value '{data}' into the {elementDescription}.");
                Screenshot(logger);
                throw new Exception($"Failed to enter the value '{data}' into the {elementDescription}.");
            }
        }


        public void Screenshot(ExtentTest logger, IWebElement moveToElement = null, string filename = null)
        {
            if (moveToElement != null)
                WebDriver.MoveToElement(moveToElement);
            logger.Log(Status.Info, GetScreenShot.EmbedImage(WebDriver, $"{filename ?? "Screenshot"}_{DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}"));
        }

        public bool ValidateURL(ExtentTest logger, string url, int timeout = 30)
        {
            try
            {
                WebDriver.WaitForUrl(timeout).Until(SeleniumExtras.WaitHelpers.ExpectedConditions.UrlContains(url));
                logger.Log(Status.Pass, $"Navigated to the expected page: {WebDriver.Url}");
            }
            catch (WebDriverTimeoutException)
            {
                logger.Log(Status.Fail, $"Failed to navigate to the expected page. Expected URL should contain {url}. Actual URL is {WebDriver.Url}");
                throw new WebDriverTimeoutException($"Failed to navigate to the expected page. Expected URL should contain {url}. Actual URL is {WebDriver.Url}");
            }
            Screenshot(logger);

            try
            {
                var httpErrorElement = WebDriver.FindElement(By.XPath(HttpError));
                if (httpErrorElement != null)
                    throw new Exception($"HTTP error displayed: {httpErrorElement.Text}. Check the test environment.");
            }
            catch { }

            return true;
        }

    }
}
