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
using System.Text.RegularExpressions;
using System.Xml;
using System.Dynamic;
using System.Collections;
using System.Web.Script.Serialization;
using System.Collections.ObjectModel;

namespace FeatureCollector
{
    
    class Program
    {
        /*   static void Main(string[] args)
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
   */

        static void Main(string[] args)
        {

            extractFeatures("http://stackoverflow.com/questions/28163618/fetching-all-href-links-from-the-page-source-using-webdriver");
        }
        static void extractFeatures(String URL)
        {
            int[] featureVector = new int[15];
            //feature 1 URL has ip address
            
            string domainName = extractDomainName("C:\\Users\\Rushikesh.Dharmadhik\\Documents\\extract_domain.py", URL);
            //Regex ip = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
            //MatchCollection result = ip.Matches(domainName);
            Console.WriteLine(domainName);
            domainName.TrimEnd('\r', '\n');
            //IPAddress ipAddress;
            //bool result = IPAddress.TryParse(domainName.Trim(), out ipAddress);
            //Console.WriteLine(result);
            if (parse(domainName) == true)
                featureVector[0] = -1;
            else
                featureVector[0] = 1;
            Console.WriteLine(featureVector[0]);
            // Console.WriteLine(parse(domainName));

            //feature 2 Long URL
            if (URL.Length < 54)
                featureVector[1] = 1;
            else if (URL.Length >= 54 && URL.Length <= 75)
                featureVector[1] = 0;
            else
                featureVector[1] = -1;

            //feature 3 tinyURL
            //see later easy to do extract domains from hrefs in page http://bit.do/list-of-url-shorteners.php

            //feature 4 @ Symbol
            if (URL.Contains("@"))
                featureVector[2] = -1;
            else
                featureVector[2] = 1;

            //feature 5 // after 7th position
            if (URL.LastIndexOf("//") > 7)
                featureVector[3] = -1;
            else
                featureVector[3] = 1;

            //feature 6 - in domain
            if (domainName.Contains('-'))
                featureVector[4] = -1;
            else
                featureVector[4] = 1;

            //feature 7 dots in domain part
            string getSubDomainDomain = extractDomainName("C:\\Users\\Rushikesh.Dharmadhik\\Documents\\extract_domain1.py", URL);
            getSubDomainDomain.Trim();
            string[] temparr = getSubDomainDomain.Trim().Split(' ');
            int dotsCount1 = 0;
            try
            {
                string subdomain = temparr[1];
                dotsCount1 = temparr[1].Split('.').Length - 1;
                
            }

            
            catch (Exception e) {

                Console.WriteLine("No subdomain found");
            }
            int dotsCount = domainName.Trim().Split('.').Length - 1 + dotsCount1;
            if (dotsCount == 1)
                featureVector[5] = 1;
            else if (dotsCount == 2)
                featureVector[5] = 0;
            else
                featureVector[5] = -1;

            //feature 8 use of https certificate issuer is ignored
            if (!URL.Substring(0, 6).Contains("https"))
                featureVector[6] = -1;
            else
                featureVector[6] = 1;

            //feature 9 //domain registration length
            whois.MyMethod(URL);
            string[] my_arr = new string[4];
            my_arr = whois.getArr();
            Console.WriteLine("Array: ");
            for(int i = 0; i < my_arr.Length; i++)
            {
                Console.WriteLine(my_arr[i]);
            }
            //System.IO.StreamReader reader = new System.IO.StreamReader(@"C:\imp.txt");
            int i1 = 0;
            while (i1 < 3) 
            {
                Console.WriteLine("I am here");
                string line = my_arr[i1];
                string[] arr = line.Split('-');
                if (i1 == 2)
                {
                    //string[] arr1 = arr[1].Trim().Split('-');
                    int year = Int32.Parse(arr[0].Trim());
                    //int month = Int32.Parse(arr1[1]);
                    int currentYear = 2016;
                    //int currentMonth = 5;
                    Console.WriteLine("Year: " + year);
                    if (currentYear - year <= 1)
                        featureVector[7] = -1;
                    else
                        featureVector[7] = 1;
                   
                }
                i1++;

            } 

            //feature 10 favicon.ico http://stackoverflow.com/questions/5119041/how-can-i-get-a-web-sites-favicon
            //feature 11 not feasible
            //12 https in domain part
            if (domainName.Contains("https") || getSubDomainDomain.Contains("https"))
                featureVector[8] = -1;
            else
                featureVector[8] = 1;

            //13 Request URL
            IWebDriver webDriver = new ChromeDriver("C:\\Users\\Rushikesh.Dharmadhik\\Selenium Drivers");
            webDriver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(180));
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(180));
            webDriver.Navigate().GoToUrl(URL);
            Console.WriteLine("SRC:\n");
            int srcCount = 0;
            int legalSrc = 0;
            int illegalSrc = 0;
            double legalPercentage = 0.0;
            double illegalpercentage = 0.0;
            try
            {
                ReadOnlyCollection<IWebElement> links = webDriver.FindElements(By.XPath("//*[@src]"));
                //ReadOnlyCollection<IWebElement> links = webDriver.FindElements(By.Name("src"));
                foreach (IWebElement webElement in links)
                {

                    string link = webElement.GetAttribute("src");
                    string attributeURL = link;
                    Console.WriteLine(attributeURL);
                    string domain1 = extractDomainName("C:\\Users\\Rushikesh.Dharmadhik\\Documents\\extract_domain.py", attributeURL);
                    if (!domain1.Contains(domainName)) 
                        illegalSrc++;
                    else
                        legalSrc++;

                    srcCount++;
                }
                //Please take a look at code below
                if (srcCount != 0)
                {
                    illegalpercentage = 100 * illegalSrc / srcCount;
                    if (illegalpercentage < 22.0)
                        featureVector[9] = 1;
                    else if (illegalpercentage >= 22.0 && illegalpercentage <= 61.0)
                        featureVector[9] = 0;
                    else
                        featureVector[9] = -1;

                }
                else
                {
                    featureVector[9] = 1;
                }

            }
            catch (Exception exception) { Console.WriteLine(exception.Message); }
            int hrefCount = 0;
            int legalHref = 0;
            int illegalHref = 0;
            double legalPercentageH = 0.0;
            double illegalpercentageH = 0.0;
            Console.WriteLine("HREF:\n");
            try
            {
                ReadOnlyCollection<IWebElement> links = webDriver.FindElements(By.XPath("//*[@href]"));
                //ReadOnlyCollection<IWebElement> links = webDriver.FindElements(By.Name("src"));
                foreach (IWebElement webElement in links)
                {
                    string link = webElement.GetAttribute("href");
                    string attributeURL = link;
                    Console.WriteLine(attributeURL);
                    string domain1 = extractDomainName("C:\\Users\\Rushikesh.Dharmadhik\\Documents\\extract_domain.py", attributeURL);
                    if (!domain1.Contains(domainName)) 
                        illegalHref++;
                    else
                        legalHref++;

                    hrefCount++;
                }
                if (hrefCount != 0)
                {
                    illegalpercentageH = 100 * illegalHref / hrefCount;
                    if (illegalpercentageH < 31.0)
                        featureVector[10] = 1;
                    else if (illegalpercentageH >= 31.0 && illegalpercentageH <= 67.0)
                        featureVector[10] = 0;
                    else
                        featureVector[10] = -1;

                }
                else
                {
                    featureVector[10] = 1;
                }
            }
            catch (Exception exception) { Console.WriteLine(exception.Message); }
            //string  [] data = whois.Instantiate(URL.Trim());
            Console.WriteLine("Illegal Src Percentage: " + illegalpercentage + " Illegal src: "+ illegalSrc + " Total src: " + srcCount);
            Console.WriteLine("Illegal Href Percentage: " + illegalpercentageH + " Illegal href: " + illegalHref + " Total href: " + hrefCount);

            //feature 12 meta link script
            //meta
            int metaCounter = 0;
            int metaIllegal=  0;
            try
            {
                ReadOnlyCollection<IWebElement> metaTags = webDriver.FindElements(By.TagName("meta"));
                foreach (IWebElement metatag in metaTags)
                {
                    int startIndex = 0;
                    string content = metatag.GetAttribute("content");
                    if (content.Contains("http") || content.Contains("http"))
                    {
                        if (!content.Contains(domainName))
                            metaIllegal++;
                    }
                    metaCounter++;

                }
            }
            catch (Exception e3)
            {

                Console.WriteLine(e3.Message);
            }
            //Link
            int linkCounter = 0;
            int illegalLink = 0;
            try
            {
                ReadOnlyCollection<IWebElement> linkTags = webDriver.FindElements(By.TagName("link"));
                Console.WriteLine("In Link: ");
                foreach (IWebElement linktag in linkTags)
                {
                    
                    //int startIndex = 0;
                    string contentURL = linktag.GetAttribute("href");
                    Console.WriteLine(contentURL);
                    string domainLink = extractDomainName("C:\\Users\\Rushikesh.Dharmadhik\\Documents\\extract_domain.py", contentURL);
                    if (!domainLink.Contains(domainName))
                        illegalLink++;
                    linkCounter++;

                }
            }
            catch (Exception e3)
            {

                Console.WriteLine(e3.Message);
            }

            int scriptCounter = 0;
            int illegalScript = 0;
            try
            {
                ReadOnlyCollection<IWebElement> scriptTags = webDriver.FindElements(By.TagName("script"));
                Console.WriteLine("In Script");
                foreach (IWebElement scripttag in scriptTags)
                {
                    //int startIndex = 0;
                    string contentURL = null;
                    contentURL = scripttag.GetAttribute("src");
                    contentURL.Trim();
                    if(contentURL != null)
                        Console.WriteLine("ContentURL: " + contentURL);
                    string domainLink = "";
                    if (contentURL != "" || contentURL!= null)
                    {
                        domainLink = extractDomainName("C:\\Users\\Rushikesh.Dharmadhik\\Documents\\extract_domain2.py", contentURL);
                        Console.WriteLine("DomainLink: " + domainLink);
                        if(domainLink.Trim() != "Exception")
                            if (!domainLink.Contains(domainName))
                                illegalScript++;
                    }
                    scriptCounter++;

                }
            }
            catch (Exception e3)
            {

                Console.WriteLine(e3.Message);
            }
            int totalLinkMetaScript = linkCounter + metaCounter + scriptCounter;
            int totalIllegal = illegalLink + metaIllegal + illegalScript;
            double illegalPercentageLinkMetaScript = 0.0;
            if (totalLinkMetaScript != 0)
                illegalPercentageLinkMetaScript = totalIllegal * 100 / totalLinkMetaScript;
            Console.WriteLine("Total Link, Meta, Script: " + totalLinkMetaScript + " Illegal: " + totalIllegal + " "  + illegalPercentageLinkMetaScript);
            if (illegalPercentageLinkMetaScript < 17.0)
                featureVector[11] = 1;
            else if (illegalPercentageLinkMetaScript >= 17.0 && illegalPercentageLinkMetaScript <= 81.0)
                featureVector[11] = 0;
            else
                featureVector[11] = -1;
            try
            {
                ReadOnlyCollection<IWebElement> formTags = webDriver.FindElements(By.TagName("form"));
                foreach(IWebElement formElement in formTags)
                {
                    string action = formElement.GetAttribute("action");
                    if (action.Contains("about:blank"))
                    {
                        featureVector[12] = -1;
                        break;
                    }
                    else if (action.Contains("http") || action.Contains("https"))
                    {
                        if (!action.Contains(domainName))
                            featureVector[12] = 0;
                        break;
                    }
                    else
                        featureVector[12] = 1;

                        
                }
                
            }
            catch(Exception e6) { Console.WriteLine(e6.Message);
                featureVector[12] = 1;
            }
            //Console.WriteLine("Data: ");
            //for (int i = 0; i < data.Length; i++)
            //  Console.WriteLine(data[i]);
            //feature 14 mail mailto doesnt seem promising


            //directly iFrame
            int counterIframes = 0;
            try
            {
                ReadOnlyCollection<IWebElement> iframeTags = webDriver.FindElements(By.TagName("iframe"));
                foreach (IWebElement iframetag in iframeTags)
                {
                    int startIndex = 0;
                    string src = iframetag.GetAttribute("src");
                    if (src.Contains("http") || src.Contains("https"))
                    {
                        if (!src.Contains(domainName))
                            featureVector[13] = -1;
                        else
                            featureVector[13] = 1;
                    }
                    else
                        featureVector[13] = 1;
                }
            }
            catch (Exception exception5)
            {
                Console.WriteLine(exception5.Message);
                featureVector[13] = 1;
            }
            //feature age of domain
            string line1 = my_arr[0];
            string[] arr1 = line1.Split('-');
            
            
                //string[] arr1 = arr[1].Trim().Split('-');
                int year1 = Int32.Parse(arr1[0].Trim());
                int month = Int32.Parse(arr1[1].Trim());
                int currentYear1 = 2016;
                int currentMonth = 5;
            Console.WriteLine("In domain age: ");
                Console.WriteLine("Year: " + year1);
            Console.WriteLine("Month: " + month);
            int differenceyear = currentYear1 - year1;
            int differencemonth = currentMonth - month;
            int differencemonth1 = differenceyear * 12 + differencemonth;
            if (differencemonth1 >= 6)
                featureVector[14] = 1;
            else
                featureVector[14] = -1;
            Console.WriteLine("Features: ");
            for (int i = 0; i < featureVector.Length; i++)
                Console.WriteLine(featureVector[i]);
            //Console.WriteLine("FeatureVector:\n" + featureVector.ToString());
            Console.Read();
        }
        private static bool parse(string ipAddress)
        {
            try
            {
                // Create an instance of IPAddress for the specified address string (in 
                // dotted-quad, or colon-hexadecimal notation).
                IPAddress address = IPAddress.Parse(ipAddress.Trim());
                bool result = IPAddress.TryParse(address.ToString(), out address);
                return result;
                // Display the address in standard notation.
                //Console.WriteLine("Parsing your input string: " + "\"" + ipAddress + "\"" + " produces this address (shown in its standard notation): " + address.ToString());
            }

            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException caught!!!");
                Console.WriteLine("Source : " + e.Source);
                Console.WriteLine("Message : " + e.Message);
            }

            catch (FormatException e)
            {
                Console.WriteLine("FormatException caught!!!");
                Console.WriteLine("Source : " + e.Source);
                Console.WriteLine("Message : " + e.Message);
            }

            catch (Exception e)
            {
                Console.WriteLine("Exception caught!!!");
                Console.WriteLine("Source : " + e.Source);
                Console.WriteLine("Message : " + e.Message);
            }
            return false;
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
    public class whois
    {
        static string[] arr = new string[4];
        public static string [] getArr()
        {
            return arr;
        }
        public static void MyMethod(string value)
        {
            //////////////////////////
            // Fill in your details //
            //////////////////////////
            string username = "HowlNettle7404";
            string password = "iamapotterhead";
            string domain = value;
            string[] myArr = new string[4];

            /////////////////////////
            // Use a JSON resource //
            /////////////////////////
            string format = "JSON";
            string url = "http://www.whoisxmlapi.com/whoisserver/WhoisService?domainName=" + domain + "&username=" + username + "&password=" + password + "&outputFormat=" + format;

            // Create our JSON parser
            JavaScriptSerializer jsc = new JavaScriptSerializer();
            jsc.RegisterConverters(new JavaScriptConverter[] { new DynamicJsonConverter() });

            // Download and parse the JSON into a dynamic object
            dynamic result = jsc.Deserialize(new System.Net.WebClient().DownloadString(url), typeof(object)) as dynamic;

            // Print a nice informative string
            try
            {
                Console.WriteLine("JSON:\n");
                myArr= result.PrintPairs();
            }
            catch (Exception e)
            {
                try
                {
                    Console.WriteLine("JSON:\nErrorMessage:\n\t{0}", result.ErrorMessage.msg);
                }
                catch (Exception e2)
                {
                    Console.WriteLine("An unkown error has occurred!");
                }
            }

            /////////////////////////
            // Use an XML resource //
            /////////////////////////
            format = "XML";
            url = "http://www.whoisxmlapi.com/whoisserver/WhoisService?domainName=" + domain + "&username=" + username + "&password=" + password + "&outputFormat=" + format;

            var settings = new XmlReaderSettings();
            var reader = XmlReader.Create(url, settings);
            WhoisRecord record = new WhoisRecord();

            try
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(WhoisRecord));
                record = (WhoisRecord)serializer.Deserialize(reader);

                reader.Close();

                // Print a nice informative string
                Console.WriteLine("XML:");
                record.PrintToConsole();
            }
            catch (Exception e)
            {
                try
                {
                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(ErrorMessage));
                    ErrorMessage errorMessage = (ErrorMessage)serializer.Deserialize(reader);

                    reader.Close();

                    // Print a nice informative string
                    Console.WriteLine("XML:\nErrorMessage:\n\t{0}", errorMessage.msg);
                }
                catch (Exception e2)
                {
                    Console.WriteLine("XML:\nException: {0}", e2.Message);
                }
            }
            //Console.WriteLine("VIMP============");
            //for (int i = 0; i < myArr.Length; i++)
            //    Console.WriteLine(myArr[i]);
            // Prevent command window from automatically closing during debugging
            //Console.WriteLine("Press any key to continue...");
            //Console.ReadKey();
        }

        //////////////////
        // JSON Classes //
        //////////////////

        public class DynamicJsonObject : DynamicObject
        {
            private IDictionary<string, object> Dictionary { get; set; }

            public DynamicJsonObject(IDictionary<string, object> dictionary)
            {
                this.Dictionary = dictionary;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                result = this.Dictionary[binder.Name];

                if (result is IDictionary<string, object>)
                {
                    result = new DynamicJsonObject(result as IDictionary<string, object>);
                }
                else if (result is ArrayList && (result as ArrayList) is IDictionary<string, object>)
                {
                    result = new List<DynamicJsonObject>((result as ArrayList).ToArray().Select(x => new DynamicJsonObject(x as IDictionary<string, object>)));
                }
                else if (result is ArrayList)
                {
                    result = new List<object>((result as ArrayList).ToArray());
                }

                return this.Dictionary.ContainsKey(binder.Name);
            }

            public string [] PrintPairs()
            {
                if (File.Exists(@"C:\imp.txt"))
                    File.Delete(@"C:\imp.txt");


                string s;
               
                Console.WriteLine("-----------------------------------");
                //foreach (var pair1 in this.Dictionary) {
                //    Console.WriteLine("I am here");
                //    try
                //    {
                //        Console.WriteLine(pair1.Key);
                //        if (pair1.Key.ToString().Equals("createdDate"))
                //            arr[0] = pair1.Value.ToString();
                //        else if (pair1.Key.ToString().Equals("updatedDate"))
                //            arr[1] = pair1.Value.ToString();
                //        else if (pair1.Key.ToString().Equals("expiresDate"))
                //            arr[2] = pair1.Value.ToString();
                //        else if (pair1.Key.ToString().Equals("domainName"))
                //            arr[3] = pair1.Value.ToString();
                //    }
                //    catch (Exception e)
                //    {
                //        Console.WriteLine(e.Message);
                //    }
                // }
                //Console.WriteLine("----------------------------");
                //for (int i = 0; i < arr.Length; i++) {
                //    Console.WriteLine(arr[i]);
                //}
                Console.WriteLine("-----------------------------");

                foreach (var pair in this.Dictionary)
                {
                    try
                    {
                        Console.WriteLine("Key: " + pair.Key);
                        Console.WriteLine("Value: " + pair.Value);
                        s = ((string)pair.Value).Replace("\n", "");
                        s = pair.Key + ": " + s.Substring(0, (s.Length < 40 ? s.Length : 40)) + "\n";
                        
                        Console.Write(s);
                    }
                    catch (Exception e)
                    {
                        s = pair.Key + ":\n";
                        Console.Write(s);

                        foreach (var subpair in pair.Value as System.Collections.Generic.Dictionary<string, object>)
                        {
                            System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\imp.txt", true);

                            try
                            {
                                s = ((string)subpair.Value).Replace("\n", "");
                                s = "\t" + subpair.Key + ": " + s.Substring(0, (s.Length < 40 ? s.Length : 40)) + "\n";
                                Console.Write(s);
                                if (subpair.Key == "createdDate")
                                {
                                    Console.WriteLine("====Found====C: "+(string)subpair.Value);
                                    arr[0] = (string)subpair.Value;

                                    file.WriteLine("Created: " + (string)subpair.Value);
                                }
                                else if (subpair.Key == "updatedDate")
                                {
                                    try
                                    {
                                        Console.WriteLine("====Found====U:" + (string)subpair.Value);
                                        arr[1] = (string)subpair.Value;

                                        file.WriteLine("Updated: " + (string)subpair.Value);
                                    }
                                    catch (Exception e2) { Console.WriteLine("\n\nUpdated E: " + e2.Message); }
                                    }
                                else if (subpair.Key == "expiresDate")
                                {
                                    try {
                                        Console.WriteLine("====Found====E:" + (string)subpair.Value);
                                        arr[2] = (string)subpair.Value;
                                        file.WriteLine("Expires: " + (string)subpair.Value);
                                    }
                                    catch(Exception e4) { Console.WriteLine("\n\nExpires E:" + e4.Message); }
                                }
                                else if (subpair.Key == "domainName")
                                {
                                    Console.WriteLine("====Found====D:" + (string)subpair.Value);
                                    arr[3] = (string)subpair.Value;
                                    file.WriteLine("Domain Name: " + (string)subpair.Value);
                                  //  file.Close();
                                }
                                file.Close();
                            }
                            catch (Exception e2)
                            {
                                //file.Close();
                                System.IO.StreamWriter file1 = new System.IO.StreamWriter(@"C:\imp.txt", true);
                                Console.Write("\t" + subpair.Key + ":\n");

                                foreach (var subsubpair in subpair.Value as System.Collections.Generic.Dictionary<string, object>)
                                {
                                    s = subsubpair.Value.ToString().Replace("\n", "");
                                    s = "\t\t" + subsubpair.Key + ": " + s.Substring(0, (s.Length < 40 ? s.Length : 40)) + "\n";
                                    Console.Write(s);
                                    if (subsubpair.Key == "createdDate")
                                    {
                                        Console.WriteLine("====Found====C");
                                        arr[0] = (string)subsubpair.Value;
                                        file1.WriteLine("Created: " + (string)subsubpair.Value);
                                    }
                                    else if (subsubpair.Key == "updatedDate")
                                    {
                                        Console.WriteLine("====Found====U");
                                        arr[1] = (string)subsubpair.Value;
                                        file1.WriteLine("Updated: " + (string)subsubpair.Value);
                                    }
                                    else if (subsubpair.Key == "expiresDate")
                                    {
                                        Console.WriteLine("====Found====E");
                                        arr[2] = (string)subsubpair.Value;
                                        file1.WriteLine("Expiry: " + (string)subsubpair.Value);
                                    }
                                    else if (subsubpair.Key == "domainName")
                                    {
                                        Console.WriteLine("====Found====D:" + (string)subpair.Value);
                                        arr[3] = (string)subsubpair.Value;
                                        file1.WriteLine("Domain: " + (string)subsubpair.Value);
                                       file1.Close();
                                    }
                                    //file1.Close();
                                }
                                //file1.Close();
                            }
                            //file.Close();
                        }
                        
                    }
                }
                //file.Close();
                Console.WriteLine("=====================");
                for(int i = 0; i<arr.Length; i++)
                {
                    Console.WriteLine(arr[i]);
                }
                return arr;
            }
        }

        public class DynamicJsonConverter : JavaScriptConverter
        {
            public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
            {
                if (dictionary == null)
                    throw new ArgumentNullException("dictionary");

                if (type == typeof(object))
                {
                    return new DynamicJsonObject(dictionary);
                }

                return null;
            }

            public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<Type> SupportedTypes
            {
                get { return new System.Collections.ObjectModel.ReadOnlyCollection<Type>(new List<Type>(new Type[] { typeof(object) })); }
            }
        }

        /////////////////
        // XML Classes //
        /////////////////

        [Serializable()]
        public class ErrorMessage
        {
            [System.Xml.Serialization.XmlElement("msg")]
            public string msg { get; set; }
        }

        [Serializable()]
        public class WhoisRecord
        {
            [System.Xml.Serialization.XmlElement("createdDate")]
            public string createdDate { get; set; }
            [System.Xml.Serialization.XmlElement("updatedDate")]
            public string updatedDate { get; set; }
            [System.Xml.Serialization.XmlElement("expiresDate")]
            public string expiresDate { get; set; }
            [System.Xml.Serialization.XmlElement("registrant")]
            public WhoisRecordContact registrant { get; set; }
            [System.Xml.Serialization.XmlElement("administrativeContact")]
            public WhoisRecordContact administrativeContact { get; set; }
            [System.Xml.Serialization.XmlElement("billingContact")]
            public WhoisRecordContact billingContact { get; set; }
            [System.Xml.Serialization.XmlElement("technicalContact")]
            public WhoisRecordContact technicalContact { get; set; }
            [System.Xml.Serialization.XmlElement("zoneContact")]
            public WhoisRecordContact zoneContact { get; set; }
            [System.Xml.Serialization.XmlElement("domainName")]
            public string domainName { get; set; }
            [System.Xml.Serialization.XmlElement("nameServers")]
            public WhoisRecordNameServers nameServers { get; set; }
            [System.Xml.Serialization.XmlElement("rawText")]
            public string rawText { get; set; }
            [System.Xml.Serialization.XmlElement("header")]
            public string header { get; set; }
            [System.Xml.Serialization.XmlElement("strippedText")]
            public string strippedText { get; set; }
            [System.Xml.Serialization.XmlElement("footer")]
            public string footer { get; set; }
            [System.Xml.Serialization.XmlElement("audit")]
            public WhoisRecordAudit audit { get; set; }
            [System.Xml.Serialization.XmlElement("registrarName")]
            public string registrarName { get; set; }
            [System.Xml.Serialization.XmlElement("registryData")]
            public WhoisRecordRegistryData registryData { get; set; }
            [System.Xml.Serialization.XmlElement("domainAvailability")]
            public string domainAvailability { get; set; }
            [System.Xml.Serialization.XmlElement("contactEmail")]
            public string contactEmail { get; set; }
            [System.Xml.Serialization.XmlElement("domainNameExt")]
            public string domainNameExt { get; set; }

            public void PrintToConsole()
            {
                Console.WriteLine("WhoisRecord:");
                Console.WriteLine("\tcreatedDate: " + createdDate);
                Console.WriteLine("\texpdatedDate: " + updatedDate);
                Console.WriteLine("\texpiresDate: " + expiresDate);
                Console.WriteLine("\tregistrant:");
                registrant.PrintToConsole();
                Console.WriteLine("\tadministrativeContact:");
                administrativeContact.PrintToConsole();
                Console.WriteLine("\tbillingContact:");
                billingContact.PrintToConsole();
                Console.WriteLine("\ttechnicalContact:");
                technicalContact.PrintToConsole();
                Console.WriteLine("\ttechnicalContact:");
                technicalContact.PrintToConsole();
                Console.WriteLine("\tzoneContact:");
                zoneContact.PrintToConsole();
                Console.WriteLine("\tdomainName: " + domainName);
                Console.WriteLine("\tnameServers:");
                nameServers.PrintToConsole();
                Console.WriteLine("\trawText: " + rawText.Substring(0, rawText.Length < 40 ? rawText.Length : 40).Replace("\n", ""));
                Console.WriteLine("\theader: " + header.Substring(0, header.Length < 40 ? header.Length : 40).Replace("\n", ""));
                Console.WriteLine("\tstrippedText: " + strippedText.Substring(0, strippedText.Length < 40 ? strippedText.Length : 40).Replace("\n", ""));
                Console.WriteLine("\tfooter: " + footer.Substring(0, footer.Length < 40 ? footer.Length : 40).Replace("\n", ""));
                Console.WriteLine("\taudit:");
                audit.PrintToConsole();
                Console.WriteLine("\tregistrarName: " + registrarName);
                Console.WriteLine("\tregistryData:");
                registryData.PrintToConsole();
                Console.WriteLine("\tdomainAvailability: " + domainAvailability);
                Console.WriteLine("\tcontactEmail: " + contactEmail);
                Console.WriteLine("\tdomainNameExt: " + domainNameExt);
            }
        }

        [Serializable()]
        public class WhoisRecordContact
        {
            [System.Xml.Serialization.XmlElement("name")]
            public string name { get; set; }
            [System.Xml.Serialization.XmlElement("organization")]
            public string organization { get; set; }
            [System.Xml.Serialization.XmlElement("street1")]
            public string street1 { get; set; }
            [System.Xml.Serialization.XmlElement("street2")]
            public string street2 { get; set; }
            [System.Xml.Serialization.XmlElement("city")]
            public string city { get; set; }
            [System.Xml.Serialization.XmlElement("state")]
            public string state { get; set; }
            [System.Xml.Serialization.XmlElement("postalCode")]
            public string postalCode { get; set; }
            [System.Xml.Serialization.XmlElement("country")]
            public string country { get; set; }
            [System.Xml.Serialization.XmlElement("email")]
            public string email { get; set; }
            [System.Xml.Serialization.XmlElement("telephone")]
            public string telephone { get; set; }
            [System.Xml.Serialization.XmlElement("rawText")]
            public string rawText { get; set; }
            [System.Xml.Serialization.XmlElement("unparsable")]
            public string unparsable { get; set; }

            public void PrintToConsole()
            {
                Console.WriteLine("\t\t\tname: " + name);
                Console.WriteLine("\t\t\torganization: " + organization);
                Console.WriteLine("\t\t\tstreet1: " + street1);
                Console.WriteLine("\t\t\tstreet2: " + street2);
                Console.WriteLine("\t\t\tcity: " + city);
                Console.WriteLine("\t\t\tstate: " + state);
                Console.WriteLine("\t\t\tpostalCode: " + postalCode);
                Console.WriteLine("\t\t\tcountry: " + country);
                Console.WriteLine("\t\t\temail: " + email);
                Console.WriteLine("\t\t\ttelephone: " + telephone);
                Console.WriteLine("\t\t\trawText: " + rawText);
                Console.WriteLine("\t\t\tunparsable: " + unparsable);
            }
        }

        [Serializable()]
        public class WhoisRecordNameServers
        {
            [System.Xml.Serialization.XmlElement("rawText")]
            public string rawText { get; set; }
            [System.Xml.Serialization.XmlElement("Address")]
            public List<string> hostNames { get; set; }
            [System.Xml.Serialization.XmlElement("class")]
            public List<string> ips { get; set; }

            public void PrintToConsole()
            {
                Console.WriteLine("\t\trawText: " + rawText.Substring(0, (rawText.Length < 40 ? rawText.Length : 40)).Replace("\n", ""));
                Console.WriteLine("\t\thostNames:");
                foreach (string hostname in hostNames)
                    Console.WriteLine("\t\t\t" + hostname);
                Console.WriteLine("\t\tips:\n");
                foreach (string ip in ips)
                    Console.WriteLine("\t\t\t" + ip);
            }
        }

        [Serializable()]
        public class WhoisRecordAudit
        {
            [System.Xml.Serialization.XmlElement("createdDate")]
            public string createdDate { get; set; }
            [System.Xml.Serialization.XmlElement("updatedDate")]
            public string updatedDate { get; set; }

            public void PrintToConsole()
            {
                Console.WriteLine("\t\tcreatedDate: " + createdDate);
                Console.WriteLine("\t\tupdatedDate: " + updatedDate);
            }
        }

        [Serializable()]
        public class WhoisRecordRegistryData
        {
            [System.Xml.Serialization.XmlElement("createdDate")]
            public string createdDate { get; set; }
            [System.Xml.Serialization.XmlElement("updatedDate")]
            public string updatedDate { get; set; }
            [System.Xml.Serialization.XmlElement("expiresDate")]
            public string expiresDate { get; set; }
            [System.Xml.Serialization.XmlElement("registrant")]
            public WhoisRecordContact registrant { get; set; }
            [System.Xml.Serialization.XmlElement("administrativeContact")]
            public WhoisRecordContact administrativeContact { get; set; }
            [System.Xml.Serialization.XmlElement("billingContact")]
            public WhoisRecordContact billingContact { get; set; }
            [System.Xml.Serialization.XmlElement("technicalContact")]
            public WhoisRecordContact technicalContact { get; set; }
            [System.Xml.Serialization.XmlElement("zoneContact")]
            public WhoisRecordContact zoneContact { get; set; }
            [System.Xml.Serialization.XmlElement("domainName")]
            public string domainName { get; set; }
            [System.Xml.Serialization.XmlElement("nameServers")]
            public WhoisRecordNameServers nameServers { get; set; }
            [System.Xml.Serialization.XmlElement("status")]
            public string status { get; set; }
            [System.Xml.Serialization.XmlElement("rawText")]
            public string rawText { get; set; }
            [System.Xml.Serialization.XmlElement("header")]
            public string header { get; set; }
            [System.Xml.Serialization.XmlElement("strippedText")]
            public string strippedText { get; set; }
            [System.Xml.Serialization.XmlElement("footer")]
            public string footer { get; set; }
            [System.Xml.Serialization.XmlElement("audit")]
            public WhoisRecordAudit audit { get; set; }
            [System.Xml.Serialization.XmlElement("registrarName")]
            public string registrarName { get; set; }
            [System.Xml.Serialization.XmlElement("whoisServer")]
            public string whoisServer { get; set; }
            [System.Xml.Serialization.XmlElement("referralURL")]
            public string referralURL { get; set; }
            [System.Xml.Serialization.XmlElement("createdDateNormalized")]
            public string createdDateNormalized { get; set; }
            [System.Xml.Serialization.XmlElement("updatedDateNormalized")]
            public string updatedDateNormalized { get; set; }
            [System.Xml.Serialization.XmlElement("expiresDateNormalized")]
            public string expiresDateNormalized { get; set; }

            public void PrintToConsole()
            {
                Console.WriteLine("\t\tcreatedDate: " + createdDate);
                Console.WriteLine("\t\tupdatedDate: " + updatedDate);
                Console.WriteLine("\t\texpiresDate: " + expiresDate);
                Console.WriteLine("\t\tregistrant:");
                registrant.PrintToConsole();
                Console.WriteLine("\t\tadministrativeContact:");
                administrativeContact.PrintToConsole();
                Console.WriteLine("\t\tbillingContact:");
                billingContact.PrintToConsole();
                Console.WriteLine("\t\ttechnicalContact:");
                technicalContact.PrintToConsole();
                Console.WriteLine("\t\tzoneContact:");
                zoneContact.PrintToConsole();
                Console.WriteLine("\t\tdomainName: " + domainName);
                Console.WriteLine("\t\tnameServers:");
                nameServers.PrintToConsole();
                Console.WriteLine("\t\tstatus: " + status.Substring(0, (status.Length < 40 ? status.Length : 40)).Replace("\n", ""));
                Console.WriteLine("\t\trawText: " + rawText.Substring(0, (rawText.Length < 40 ? rawText.Length : 40)).Replace("\n", ""));
                Console.WriteLine("\t\theader: " + header.Substring(0, (header.Length < 40 ? header.Length : 40)).Replace("\n", ""));
                Console.WriteLine("\t\tstrippedText: " + strippedText.Substring(0, (strippedText.Length < 40 ? strippedText.Length : 40)).Replace("\n", ""));
                Console.WriteLine("\t\tfooter: " + footer.Substring(0, (footer.Length < 40 ? footer.Length : 40)).Replace("\n", ""));
                Console.WriteLine("\t\taudit:");
                audit.PrintToConsole();
                Console.WriteLine("\t\tregistrarName: " + registrarName);
                Console.WriteLine("\t\twhoisServer: " + whoisServer);
                Console.WriteLine("\t\treferralURL: " + referralURL);
                Console.WriteLine("\t\tcreatedDateNormalized: " + createdDateNormalized);
                Console.WriteLine("\t\tupdatedDateNormalized: " + updatedDateNormalized);
                Console.WriteLine("\t\texpiresDateNormalized: " + expiresDateNormalized);
            }
        }
    }

}
