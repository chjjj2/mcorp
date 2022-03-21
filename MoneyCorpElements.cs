using AventStack.ExtentReports;
using OpenQA.Selenium;

namespace UITest
{
    public class MoneyCorpElements : BasePage
    {
        public static MoneyCorpElements Current => GetValue<MoneyCorpElements>($"{GetTest()}.MoneyCorpElements");

        public static void Load(IWebDriver driver, Tests.TestContext context)
        {
            SaveValue($"{context.Test.FullName}.MoneyCorpElements", new MoneyCorpElements(driver, context));
        }
        
        public readonly string AddThisXButton = "//div[@class='addthis_bar_x icon-arrow-up']";
        public readonly string RegionDropDown = "//button[@id = 'language-dropdown-flag']"; //using existing xpath method but would be better to use id rather than xpath here
        public readonly string USRegionSelector = "//a[contains(@href, 'CountrySelector') and contains(@aria-label, 'USA English')]";
        public readonly string SearchButton = "//button[contains(@aria-label, 'Search')]";

        //below should be in another page class really
        public readonly string ForExFindOutMoreButton = "//a[contains(@href, 'foreign-exchange-solutions') and contains(@aria-label, 'Find out more')]";
        public readonly string SearchInput = "//input[@id='nav_search']";  
        public readonly string SearchResultItem = "//div[@class='results clearfix']//a";

        private MoneyCorpElements(IWebDriver driver, Tests.TestContext context) : base(driver, context)
        {
        }

        public void OpenHomepage(ExtentTest logger)
        {
            WebDriver.Navigate().GoToUrl("https://www.moneycorp.com/en-gb/");
            ValidateURL(logger, "https://www.moneycorp.com/en-gb/", 60);
            TryFindElementAndClick(AddThisXButton, "Add This X Button", logger);
        }
        public void SelectLanguageAndRegion(ExtentTest logger) //should parameterize the country value for reuse
        {
            TryFindElementAndClick(RegionDropDown, "Region Selector", logger);
            TryFindElementAndClick(USRegionSelector, "US Region Selector", logger);
        }

        public void SelectForeignExchangeSolutions(ExtentTest logger)
        {
            TryFindElementAndClick(ForExFindOutMoreButton, "ForEx Find Out More Button", logger);
            ValidateURL(logger, "/en-us/business/foreign-exchange-solutions/", 60); //could also check for elements or title etc
        }

        public void SelectSearchButton(ExtentTest logger)
        {
            TryFindElementAndClick(SearchButton, "Search Button", logger);
        }

        public void SearchForTerm(ExtentTest logger, string searchTerm, bool withSubmit = false)
        {
            IWebElement element = TryFindElementAndEnterData(SearchInput, searchTerm, "Search Field", logger);
            if (withSubmit)
                element.SendKeys(Keys.Enter);
            ValidateURL(logger, "https://www.moneycorp.com/en-us/search/?q", 60);
        }

        public void VerifySearchResultLinks(ExtentTest logger)
        {
            var resultItems = WaitForElementsToBeVisible(SearchResultItem);
            foreach (var item in resultItems)
            {
                var link = item.GetAttribute("href");
                if (link.Contains("https://www.moneycorp.com/en-us/")) //should paramterize
                    logger.Log(Status.Pass, $"Expected value Found':" + " " + link);
                else
                    logger.Log(Status.Fail, $"Unexpected value Found':" + " " + link);
            }
            Screenshot(logger);
        }
    }
}