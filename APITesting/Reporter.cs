using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APITesting
{
    public static class Reporter
    {
        public static ExtentReports extentReports;
        public static ExtentHtmlReporter htmlReporter;
        public static ExtentTest test;

        // Setting up the extenter reports here
        // Location is given here
        // Change the location of the report based on user preference in this function
        // Test will create the folder if not avaible already
        public static void SetupExtentReport(string reportName, string documentTitle)
        {
            htmlReporter = new ExtentHtmlReporter(@"C:\reports\index.html");
            htmlReporter.Config.Theme = Theme.Dark;
            htmlReporter.Config.DocumentTitle = documentTitle;
            htmlReporter.Config.ReportName = reportName;
            extentReports = new ExtentReports();
            extentReports.AttachReporter(htmlReporter);
        }
        
        // Function to create individual test for reporting
        public static void CreateTest(string testName)
        {
            test = extentReports.CreateTest(testName);
        }

        // Function to log the report with test status and with a message
        public static void LogToReport(Status status, string message)
        {
            test.Log(status, message);
        }

        // Function to cleardown reporter at the end of tests run
        public static void FlushReport()
        {
            extentReports.Flush();
        }

        // Function to report the status of the individual tests
        public static void TestStatus(string status)
        {
            if (status.Equals("Pass"))
            {
                test.Pass("Test is passed");
            }
            else
            {
                test.Pass("Test is failed");
            }
        }
    }
}
