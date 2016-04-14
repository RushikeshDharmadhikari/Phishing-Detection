using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System.Diagnostics;
using System.IO;
using OpenQA.Selenium.PhantomJS;
using System.Net;

namespace FeatureCollector
{
    class Program
    {
        static void Main(string[] args)
        {
            IWebDriver webDriver = new ChromeDriver("C:\\Users\\Rushikesh.Dharmadhik\\Selenium Drivers");
            webDriver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(180));
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(180));
            // PhantomJSDriver webDriver = new PhantomJSDriver("C:\\Users\\Rushikesh.Dharmadhik\\Downloads\\phantomjs-2.1.1-windows\\phantomjs-2.1.1-windows\\bin"); 
            //var service = PhantomJSDriverService.CreateDefaultService();
            //service.SslProtocol = "tlsv1"; //"any" also works
            //IWebDriver webDriver = new PhantomJSDriver(service);
            //  webDriver.Navigate().GoToUrl("http://www.timothy-christian.com/wp-content/themes/redux/functions/tinymce/pulign/a8525d15f06fc6a6a36b8a6a8b6559de/en_US/i/scr/pulign/873d95cded50a31f9cbd96720ff18c61/Confirm.php?cmd=_error_login-run&dispatch=5885d80a13c0db1fb6947b0aeae66fdbfb2119927117e3a6f876e0fd34af436580c63a156ebffe89e7779ac84ebf634880c63a156ebffe89e7779ac84ebf6348");
            // String pageSource = webDriver.PageSource;
            //Console.WriteLine(pageSource);
            //IWebDriver webDriver = new RemoteWebDriver(DesiredCapabilities.PhantomJS());
            int index = 0;
           // int[] legal = new int[1000];
            //int[] illegal = new int[1000];
            String subStr = "";
            int legalCount = 0;
            int illegalCount = 0;
            int counter = 0;
            var reader1 = new StreamReader(File.OpenRead(@"C:\Users\Rushikesh.Dharmadhik\Downloads\verified_online(1).csv"));
            var reader2 = new StreamReader(File.OpenRead(@"C:\Users\Rushikesh.Dharmadhik\Downloads\top-1m.csv"));
            double[][] allData = new double[500][];
            System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Normalized4.txt");
            Console.WriteLine("Reading Files.....");

            //Feature Extraction
            // while (!reader1.EndOfStream && counter < 1000)
            while (!reader2.EndOfStream && counter < 500)
            {
                legalCount = 0;
                illegalCount = 0;
                index = 0;
                allData[counter] = new double[3];
                string line = reader2.ReadLine();
                string[] values = line.Split(',');
                string URL = "";
                try
                {
                    URL = values[1].Trim();
                    URL = "https://www." + URL; //top 1m
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Counter: " + counter);
                }
                // HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(URL);
                //httpReq.AllowAutoRedirect = false;

                //   HttpWebResponse httpRes = (HttpWebResponse)httpReq.GetResponse();

                // if (httpRes.StatusCode == HttpStatusCode.OK)
                //{
                // Code for moved resources goes here.


                // Close the response.
                //httpRes.Close();
                String status = getStatusCode("-cp", "C:\\Users\\Rushikesh.Dharmadhik\\Documents\\Docs", "Check", URL);
                if (status == "")
                    continue;
                Console.WriteLine(status);
                if (status.Contains("Exception") || !status.Contains("200"))
                {
                    continue;
                }
                    webDriver.Navigate().GoToUrl(URL);
                //    webDriver.GetScreenshot();
                try
                {
                    if (webDriver.Title.Contains("Suspend") || webDriver.Title.Contains("suspend") || webDriver.Title.Contains("Unavailable") || webDriver.Title.Contains("unavailable"))
                    {
                        continue;
                    }
                      //  continue;
                }
                catch (Exception excepti) {
                    Console.WriteLine(excepti.Message);
                    //webDriver.Close();
                    webDriver = new ChromeDriver("C:\\Users\\Rushikesh.Dharmadhik\\Selenium Drivers");
                    continue;
                }
                Console.WriteLine(counter);
                String pageSource = "";
                try
                {
                    pageSource = webDriver.PageSource; //handle webDriver exception
                }
                catch (Exception except) {
                    Console.WriteLine(except.Message);
                    //counter++;
                    continue;
                }
                   // Console.WriteLine("I am there");
                    string domainName = extractDomainName("C:\\Users\\Rushikesh.Dharmadhik\\Documents\\extract_domain.py", URL);
                    while (index != -1)
                    {
                    try
                    {
                        index = pageSource.IndexOf("src", index + 1);
                    }
                    catch (Exception e1)
                    {
                        break;
                    }
                        if (index == -1)
                            break;
                        //int index1 = pageSource.IndexOf("src", index+1);
                        //    string url = "http://www.timothy-christian.com/wp-content/themes/redux/functions/tinymce/pulign/a8525d15f06fc6a6a36b8a6a8b6559de/en_US/i/scr/pulign/873d95cded50a31f9cbd96720ff18c61/Confirm.php?cmd=_error_login-run&dispatch=5885d80a13c0db1fb6947b0aeae66fdbfb2119927117e3a6f876e0fd34af436580c63a156ebffe89e7779ac84ebf634880c63a156ebffe89e7779ac84ebf6348";


                        try { subStr = pageSource.Substring(index, 50); }
                        catch (Exception exception)
                        {
                        break;
                            //subStr = pageSource.Substring(index, pageSource.Length - index-2);
                        }
                        //Console.WriteLine("Substring: " + subStr);

                        if (subStr.Contains("https://") || subStr.Contains("http://"))
                        {
                            //Console.WriteLine("Its an URL");
                            if (!subStr.Contains(domainName))
                                illegalCount++;
                            else
                                legalCount++;
                        }
                        else
                            legalCount++;

                    }
                    allData[counter][0] = illegalCount;
                    legalCount = 0;
                    illegalCount = 0;
                    index = 0;
                    while (index != -1)
                    {
                    try {
                        index = pageSource.IndexOf("href", index + 1);
                    }
                    catch(Exception e)
                    {
                        break;
                    }
                        if (index == -1)
                            break;
                        //int index1 = pageSource.IndexOf("src", index+1);
                        //    string url = "http://www.timothy-christian.com/wp-content/themes/redux/functions/tinymce/pulign/a8525d15f06fc6a6a36b8a6a8b6559de/en_US/i/scr/pulign/873d95cded50a31f9cbd96720ff18c61/Confirm.php?cmd=_error_login-run&dispatch=5885d80a13c0db1fb6947b0aeae66fdbfb2119927117e3a6f876e0fd34af436580c63a156ebffe89e7779ac84ebf634880c63a156ebffe89e7779ac84ebf6348";

                        //subStr = pageSource.Substring(index, 50);
                        //   Console.WriteLine("Substring: " + subStr);
                        try { subStr = pageSource.Substring(index, 50); }
                        catch (Exception exception)
                        {

                        break;
                            //subStr = pageSource.Substring(index, pageSource.Length - index - 2);
                        }

                        if (subStr.Contains("https://") || subStr.Contains("http://"))
                        {
                            //Console.WriteLine("Its an URL");
                            if (!subStr.Contains(domainName))
                                illegalCount++;
                            else
                                legalCount++;
                        }
                        else
                            legalCount++;

                    }
                    allData[counter][1] = illegalCount;
                    if (pageSource.Contains("about:blank"))
                        allData[counter][2] = 1;
                    else
                        allData[counter][2] = 0;
                    file.WriteLine(counter + " " + URL);
                    counter++;
                }
            //}
            //Console.WriteLine(subStr);

            //Console.WriteLine(index);
            //Console.WriteLine(index1);
            //Console.WriteLine("Legal: "+ legalCount);
            //Console.WriteLine("Illegal: "+ illegalCount);
            ShowMatrix(allData, 500, 1, true);
            Normalize(allData, new int[] { 0, 1 });
            ShowMatrix(allData, 500, 1, true);
            WriteToFile(allData, 500, 1, true);
            //double[,] data = new double[100, 3];
            file.Close();
            Console.Read();

            Environment.Exit(0);
            webDriver.Close();
        }
        static void ShowMatrix(double[][] matrix, int numRows, int decimals, bool newLine)
        {
            for (int i = 0; i < numRows; ++i)
            {
                Console.Write(i.ToString().PadLeft(3) + ": ");
                for (int j = 0; j < matrix[i].Length; ++j)
                {
                    if (matrix[i][j] >= 0.0) Console.Write(" "); else Console.Write("-");
                    Console.Write(Math.Abs(matrix[i][j]).ToString("F" + decimals) + " ");
                }
                Console.WriteLine("");
            }
            if (newLine == true) Console.WriteLine("");
        }
        static void WriteToFile(double[][] matrix, int numRows, int decimals, bool newLine)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Normalized6.txt");
            for (int i = 0; i < numRows; ++i)
            {
                file.Write(i.ToString().PadLeft(3) + ": ");
                for (int j = 0; j < matrix[i].Length; ++j)
                {
                    if (matrix[i][j] >= 0.0) Console.Write(" "); else file.Write("-");
                    file.Write(Math.Abs(matrix[i][j]).ToString("F" + decimals) + " ");
                }
                file.WriteLine("");
            }
            if (newLine == true) file.WriteLine("");
            file.Close();
        }

        private static string extractDomainName(string cmd, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "C:\\Users\\Rushikesh.Dharmadhik\\AppData\\Local\\Programs\\Python\\Python35\\python.exe";
            start.Arguments = string.Format("{0} {1}", cmd, args);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            string result = "";
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    result = reader.ReadToEnd();
                    //Console.Write(result);
                }
            }
            return result;
        }
        private static string getStatusCode(string cmd, string args1, string args2, string args3)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "C:\\Program Files\\Java\\jdk1.8.0_73\\bin\\java.exe";
            start.Arguments = string.Format("{0} {1} {2} {3}", cmd, args1, args2, args3);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            string result = "";
            using (Process process = Process.Start(start))
            {
                process.WaitForExit(60000);
                using (StreamReader reader = process.StandardOutput)
                {
                    result = reader.ReadToEnd();
               //     Console.Write("Status: " + result);
                }
            }
            return result;
        }
        static void Normalize(double[][] dataMatrix, int[] cols)
        {

            foreach (int col in cols)
            {
                double sum = 0.0;
                for (int i = 0; i < dataMatrix.Length; ++i)
                    sum += dataMatrix[i][col];
                double mean = sum / dataMatrix.Length;
                sum = 0.0;
                for (int i = 0; i < dataMatrix.Length; ++i)
                    sum += (dataMatrix[i][col] - mean) * (dataMatrix[i][col] - mean);

                double sd = Math.Sqrt(sum / (dataMatrix.Length - 1));
                for (int i = 0; i < dataMatrix.Length; ++i)
                    dataMatrix[i][col] = (dataMatrix[i][col] - mean) / sd;
            }
        }
    }
}
