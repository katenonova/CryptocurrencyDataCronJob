using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace CurrencyDataCronJob
{
    class Program
    {

        private const string API_URL = @"https://coinmetrics.io/newdata/";
        static readonly string[] CURRENCY_LIST = { "btc", "bch", "bnb", "ltc", "bsv", "eth", "xrp" };
        private const string FOLDER_PATH = @"C:\Source\POC";
        static void Main(string[] args)
        {
            // create a new folder where to store the downloaded data 
            string folderNameCurrentDate = System.IO.Path.Combine(FOLDER_PATH, DateTime.Now.ToString("dd-MM-yyyy"));
            CreateNewFolder(folderNameCurrentDate);

            //clean cache data             
            RedisConnectorHelper.ClearRedisCache();

            try
            {
                //download the data and save it in the cache
                foreach (string currentCurrencyName in CURRENCY_LIST)
                {
                    using (WebClient webClient = new WebClient())
                    {
                        //download files from the source
                        Uri currentUrl = new Uri(API_URL + currentCurrencyName + ".csv");
                        string fileName = System.IO.Path.Combine(folderNameCurrentDate, currentCurrencyName + ".csv");
                        webClient.DownloadFile(currentUrl, fileName);
                        string headerKeys = File.ReadAllLines(fileName).First();

                        //read the downloaded data and save it in the cache
                        List<Cryptocurrency> metrics = File.ReadAllLines(fileName)
                                               .Skip(1)
                                               .Select(values => Cryptocurrency.CreateObjectFromCsv(headerKeys, values))
                                               .ToList();
                        RedisConnectorHelper.SaveDataInRedisCache(currentCurrencyName, JsonConvert.SerializeObject(metrics, Formatting.Indented));
                    }
                }
                //delete the folder that contains downloaded data from the previous date
                string folderNamePreviousDate = System.IO.Path.Combine(FOLDER_PATH, DateTime.Today.AddDays(-1).ToString("dd-MM-yyyy"));
                if (Directory.Exists(folderNamePreviousDate))
                {
                    Directory.Delete(folderNamePreviousDate, recursive: true);
                }
            }
            catch (InvalidCastException e)
            {
                if (e.Source != null)
                    Console.WriteLine("IOException source: {0}", e.Source);
                throw;
            }
        }

        /**
        * Create a new folder 
        * param: folderName - absolute path to the folder;
        */
        private static void CreateNewFolder(string folderName)
        {
            if (!System.IO.Directory.Exists(folderName))
            {
                System.IO.Directory.CreateDirectory(folderName);
            }
            else
            {
                Console.WriteLine("Folder \"{0}\" already exists.", folderName);
                return;
            }
        }
    }
}
