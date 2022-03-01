using APITesting;
using AventStack.ExtentReports;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Engine.ClientProtocol;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using TestExecutionContext = NUnit.Framework.Internal.TestExecutionContext;

namespace APITests
{
    public class Tests
    {
        private WireMockServer server;
        public static RestClient client;
        public HttpStatusCode statusCode;
        public static TestContext TestContext { get; set; }

        // Class related steps before starting the tests
        // Setting up the test client and reporter here
        [OneTimeSetUp]
        public static void ClassSetup()
        {
            // If 9050 port is not free, change it to a free port here
            // And use the same port in the StartServer function as well.
            client = new RestClient("http://localhost:9050");
            Reporter.SetupExtentReport("APITests", "APITestReport");
        }

        // Setup steps for each tests
        // Starting the WireMockServer and capture the individual test name by the reporter
        [SetUp]
        public void TestSetup()
        {
            // WireMockServer is starting in 9050.
            // User needs to make sure that this port is free.
            // If 9050 port is not free, change it to a free port here
            server = WireMockServer.Start(9050);
            Reporter.CreateTest(TestContext.CurrentContext.Test.FullName);
        }

        // Test check the Health of CoinGecko web site
        [Test] 
        public void TestCoinGeckoHealth()
        {
            var health = new CoinGecko();
            var response = health.GetHealth();
            statusCode = response.StatusCode;
            var code = (int)statusCode;
            Assert.AreEqual(200, code);
            Reporter.LogToReport(Status.Pass, "CoinGecko is healthy!");
            var contentData = ContentHelper.GetContent<APIDefinitions>(response.Content);
            Assert.Multiple(() =>
            {
                Assert.AreEqual("(V3) To the Moon!", contentData.gecko_says);
                Assert.AreNotSame("(V2) To the Moon!", contentData.gecko_says);
                Assert.AreNotSame("To the Moon!", contentData.gecko_says);
                Assert.AreNotSame("(V3) To the Sun!", contentData.gecko_says);
            });
            Reporter.LogToReport(Status.Pass, "CoinGecko is to the Moon!");
        }

        // Test to check the exchange rate of BTC. 
        // Two exchange rates are considered. 1) BTC, 2) USD
        [Test]
        public void TestBTCCurrency()
        {
            var currency = new CoinGecko();
            var response = currency.GetCurrency();
            statusCode = response.StatusCode;
            var code = (int)statusCode;
            Assert.AreEqual(200, code);
            var contentData = ContentHelper.GetContent<Currency>(response.Content);

            // Validation of BTC-BTC currency pair
            foreach (var item in contentData.Rates)
            {
                if (item.Key == "btc")
                {
                    Assert.Multiple(() =>
                    {
                        Assert.AreEqual("Bitcoin", item.Value.Name);
                        Assert.AreEqual("BTC", item.Value.Unit);
                        Assert.AreEqual(1, item.Value.Value);
                        Assert.AreEqual("crypto", item.Value.Type);
                    });
                    Reporter.LogToReport(Status.Pass, "BTC exchange rates are fine!");
                    break;
                }
            }

            //Validation of BTC-USD currency pair
            foreach (var item in contentData.Rates)
            {
                if (item.Key == "usd")
                {
                    Assert.Multiple(() =>
                    {
                        Assert.AreEqual("US Dollar", item.Value.Name);
                        Assert.AreEqual("$", item.Value.Unit);
                        Assert.IsNotNull(item.Value.Value);
                        Assert.IsTrue(item.Value.Value >= 0, "USD-BTC exchange rate is not greater than 0");
                        Assert.AreEqual("fiat", item.Value.Type);
                    });
                    Reporter.LogToReport(Status.Pass, "BTC-USD exchange rates are fine!");
                    break;
                }
            }
        }

        // Verify the data returned for kraken futures exchange
        [Test]
        public void TestKrakenExchange()
        {
            var exchange = new CoinGecko();
            var response = exchange.GetExchange("derivatives/exchanges/kraken_futures");
            statusCode = response.StatusCode;
            var code = (int)statusCode;
            Assert.AreEqual(200, code);
            var contentData = ContentHelper.GetContent<Exchange>(response.Content);
            Assert.Multiple(() =>
            {
                Assert.AreEqual("Kraken (Futures)", contentData.Name);
                Assert.IsTrue(contentData.open_interest_btc >= 0, "open_interest_btc is less than 0");
                Assert.IsNotNull(contentData.trade_volume_24h_btc);
                Assert.IsTrue(contentData.number_of_perpetual_pairs >= 0, "number_of_perpetual_pairs less than 0");
                Assert.IsTrue(contentData.number_of_futures_pairs >= 0, "number_of_futures_pairs is less than 0");
                Assert.AreEqual("https://assets.coingecko.com/markets/images/426/small/NX0D_EiD_400x400.jpg?1560770269", contentData.Image.ToString());
                Assert.AreEqual(2019, contentData.year_established);
                Assert.AreEqual("United States", contentData.Country);
                Assert.AreEqual("Kraken Futures was previously known as Crypto Facilities. " +
                    "Crypto Facilities Ltd is an FCA authorised cryptocurrency derivatives platform based in London. " +
                    "Crypto Facilities became a part of the Kraken group of companies and rebranded to Kraken Futures in February 2019. " +
                    "They offer up to 50x leverage on 5 trading pairs. The most popular trading pair is the XBTUSD pair. ", contentData.Description);
                Assert.AreEqual("https://futures.kraken.com/", contentData.Url.ToString());
            });
            Reporter.LogToReport(Status.Pass, "Kraken Future exchange information is correct!");
        }

        // Test the data returned for kraken futures exchange
        // User is passing the ticker 'all'
        [Test]
        public void TestKrakenExchangeIncludeTickersAll()
        {
            var exchange = new CoinGecko();
            var response = exchange.GetExchange("derivatives/exchanges/kraken_futures?include_tickers=all");
            statusCode = response.StatusCode;
            var code = (int)statusCode;
            Assert.AreEqual(200, code);
            Reporter.LogToReport(Status.Pass, "Kraken Future exchange reponse recieved successfully with 'all' tickers");
        }

        // Test the data returned for kraken futures exchange
        // User is passing the ticker 'Unexipred'
        [Test]
        public void TestKrakenExchangeIncludeTickersUnexpired()
        {
            var exchange = new CoinGecko();
            var response = exchange.GetExchange("derivatives/exchanges/kraken_futures?include_tickers=unexpired");
            statusCode = response.StatusCode;
            var code = (int)statusCode;
            Assert.AreEqual(200, code);
            Reporter.LogToReport(Status.Pass, "Kraken Future exchange reponse recieved successfully with 'unexpired' tickers");
        }

        // Test the derivative exchanges endpoint with an invalid exchange name
        [Test]
        public void TestInvalidExchange()
        {
            var exchange = new CoinGecko();
            var response = exchange.GetExchange("derivatives/exchanges/invalid_exchange");
            statusCode = response.StatusCode;
            var code = (int)statusCode;
            Assert.AreEqual(404, code);
            var contentData = ContentHelper.GetContent<Exchange>(response.Content);
            Assert.AreEqual("translation missing: en.api.errors.market_not_found", contentData.Error);
            Reporter.LogToReport(Status.Pass, "Invalid exchange name request returns no information");
        }

        // Test the derivative exchanges endpoint with an invalid URL
        [Test]
        public void TestInvalidExchangeUrl()
        {
            var exchange = new CoinGecko();
            var response = exchange.GetExchange("");
            statusCode = response.StatusCode;
            var code = (int)statusCode;
            Assert.AreEqual(404, code);
            var contentData = ContentHelper.GetContent<Exchange>(response.Content);
            Assert.AreEqual("Incorrect path. Please check https://www.coingecko.com/api/", contentData.Error);
            Reporter.LogToReport(Status.Pass, "Invalid exchange request ends in error");
        }

        // Test the derivative exchanges endpoint to update the BTC exchange rate using stubs
        [Test]
        public void TestBTCPutStub()
        {
            var stub = new APIHelper();
            server = stub.CreateBTCPutStub(server);
            RestRequest request = new RestRequest("/btc/10000", Method.PUT);
            IRestResponse response = client.Execute(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.ContentType, Is.EqualTo("text/plain"));
            Assert.That(response.Content, Is.EqualTo("BTC price set to 10000!"));
            Reporter.LogToReport(Status.Pass, "1 BTC is same as $10000");
        }

        // Cleaning up the test here.
        // Reports and logged
        // WireMockServer is stopped
        [TearDown]
        public void CleaupTest()
        {
            var status = TestContext.CurrentContext.Result.Outcome.Status;
            Status logSatus;
            switch (status)
            {
                case TestStatus.Failed:
                    logSatus = Status.Fail;
                    Reporter.TestStatus(logSatus.ToString());
                    break;
                case TestStatus.Passed:
                    logSatus = Status.Pass;
                    Reporter.TestStatus(logSatus.ToString());
                    break;
                case TestStatus.Warning:
                    logSatus = Status.Warning;
                    Reporter.TestStatus(logSatus.ToString());
                    break;
                case TestStatus.Skipped:
                    logSatus = Status.Skip;
                    Reporter.TestStatus(logSatus.ToString());
                    break;
                default:
                    break;
            }
            server.Stop();
        }

        // Class teardown steps 
        // Cleaning the reporter
        [OneTimeTearDown]
        public void CleanupClass()
        {
            Reporter.FlushReport();
        }
    }
}