using AventStack.ExtentReports;
using NUnit.Framework;
using NUnit.Framework.Internal;
using OpenQA.Selenium;
using System.Runtime.Remoting.Messaging;
using UITest.Core.Factory;
using UITest.Core.LoggingAndReporting;

namespace UITest.Digital.TestCases.Account_Summary
{
    [TestFixture]
    [Parallelizable(ParallelScope.Children)]
    public class moneyCorp : Log
    {

        [SetUp]
        public void setup()
        {
            CallContext.LogicalSetData("Tests.TestContext", new Tests.TestContext(TestExecutionContext.CurrentContext));

            IWebDriver webDriver = BrowserFactory.getBrowser(Config.browser, Config.BaseUrl);
            axeBuilder = new Globant.Selenium.Axe.AxeBuilder(webDriver);

            loggers.TryAdd(TestContext.Test.Name, report.CreateTest(TestContext.Test.Name));

            MoneyCorpElements.Load(webDriver, TestContext); 
        }

        [TearDown]
        public void CleanUp()
        {
            this.GetResult(MoneyCorpElements.Current.WebDriver, "moneyCorp", TestContext);
            BrowserFactory.closeBrowser(MoneyCorpElements.Current.WebDriver, report, GetLogger(TestContext.Test.MethodName));
            ReportTearDown(TestContext);
        }


        [Test, Category("MoneyCorp")]
        public void MoneyCorpTest()
        {
            ExtentTest logger = GetLogger();
            
                MoneyCorpElements.Current.OpenHomepage(logger);
                MoneyCorpElements.Current.SelectLanguageAndRegion(logger); //ToDo parameterize region
                MoneyCorpElements.Current.SelectForeignExchangeSolutions(logger);
                MoneyCorpElements.Current.SelectSearchButton(logger);
                MoneyCorpElements.Current.SearchForTerm(logger,"international payments", true);
                MoneyCorpElements.Current.VerifySearchResultLinks(logger);// ToDo expand to get other pages
            //BUG with first page counts that change on second page
        }

    }
}
