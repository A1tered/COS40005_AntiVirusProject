using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsynchronousProgrammingPractice
{
    public class AsynchronousCaller
    {
        public AsynchronousCaller()
        {

        }
        public void startCall()
        {
            List<Task> outputList = new List<Task>();
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
                var result = Task.Run(() => (hasherInstance.ConvertHash(item))); // I believe this has the task run on a different thread.
                outputList.Add(result);
            }
            stopwatchItem.Stop();
            Console.WriteLine("Print out");
            Task.WaitAll(outputList.ToArray()); // Only continue once, all the tasks are done. 
            foreach (Task<string> output in outputList)
            {
                Console.WriteLine(output.Result);
            }
            Console.WriteLine("Asynchronous Example");
            Console.WriteLine($"Time taken for conversions: {stopwatchItem.Elapsed}");
        }
    }

   
}
