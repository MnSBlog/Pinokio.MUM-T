using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace Pinokio.Core
{

    public static class ListMethods
    {

        public static List<int> FindAllIndexOf<T>(this IEnumerable<T> values, T val)
        {
            return values.Select((b, i) => object.Equals(b, val) ? i : -1).Where(i => i != -1).ToList();
        }

        public static void PrintAll<T>(this IEnumerable<T> values)
        {
            string valueString = values.ToString<T>();
            Console.WriteLine(valueString);
        }

        public static string ToString<T>(this IEnumerable<T> values)
        {
            string valueString = "";
            for (int i = 0; i < values.Count() - 1; i++)
            {
                valueString += values.ElementAt(i).ToString() + ", ";
            }
            if (values.Count() > 0)
                valueString += values.ElementAt(values.Count() - 1).ToString();
            
            return valueString;
        }

        public static void MergeWithoutDuplicate<T>(this List<T> values, IEnumerable<T> other)
        {
            for (int i = 0; i < other.Count(); i++)
            {
                if (!values.Contains<T>(other.ElementAt(i)))
                    values.Add(other.ElementAt(i));
            }
        }

        public static (double mean, double variance) MeanAndVariance(this List<double> values)
        {
            double mean = values.Average();
            double sum = 0;
            values.ForEach(x => sum += Math.Pow(x - mean, 2));
            double variance = sum / (values.Count - 1);

            return (mean, variance);
        }
    }
}
