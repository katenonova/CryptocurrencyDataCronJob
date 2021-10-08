using System;
using System.Collections.Generic;
using System.Text;

namespace CurrencyDataCronJob
{
    public class Cryptocurrency
    {
        public string date { get; set; }
        public Dictionary<string, string> metrics = new Dictionary<string, string>();

        /**
        * Transform csv info into a class
        * param: header - string with all columns' title separeted by "," 
        * param: csvValueline -string with all values from a csv line separeted by ","
        * return:  class Cryptocurrency 
        */
        public static Cryptocurrency CreateObjectFromCsv(string header, string csvValueLine)
        {            
            string[] keys = header.Split(',');
            string[] values = csvValueLine.Split(',');

            Cryptocurrency currentMetricsByDate = new Cryptocurrency();
            currentMetricsByDate.date = values[0];
            Dictionary<string, string> metrics = new Dictionary<string, string>();

            for (int i = 1; i < keys.Length - 1; i++)
            {
                currentMetricsByDate.metrics.Add(keys[i], (values[i] != "") ? values[i] : "-");
            }

            return currentMetricsByDate;
        }
    }
}
