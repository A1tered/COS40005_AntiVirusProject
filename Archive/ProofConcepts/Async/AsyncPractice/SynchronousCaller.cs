using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsynchronousProgrammingPractice
{
    public class SynchronousCaller
    {
        public SynchronousCaller()
        {

        }
        public void startCall()
        {
            List<string> outputList = new List<string>();
            Hasher hasherInstance = new();
            List<string> itemsToConvert = new List<string>();

            for (int i = 0; i < 100; i++)
            {
                //Console.WriteLine($"{itemsToConvert.Count()}");
                itemsToConvert.Add((i * i - 1).ToString());
            }
            Console.WriteLine($"{itemsToConvert.Count()}");

            Console.WriteLine("Conversion Operations Start Here:");

            Stopwatch stopwatchItem = new();
            stopwatchItem.Start();
            foreach (string item in itemsToConvert)
            { // Converts 100 items, hashes them a couple of times.
                outputList.Add(hasherInstance.ConvertHash(item));
            }
            stopwatchItem.Stop();
            Console.WriteLine("Print out");
            foreach (string output in outputList)
            {
                Console.WriteLine(output);
            }
            Console.WriteLine("Synchronous Example");
            Console.WriteLine($"Time taken for conversions: {stopwatchItem.Elapsed}");
        }
    }
}
