using System.Text.RegularExpressions;
using RegularExpressionDataGenerator;

namespace TurnitTest.Methods
{
    public class Methods
    {
        
        /// <summary>
        /// Reads and parses .txt file
        /// </summary>
        /// <returns>
        /// List with Break(int startTime(HHmm), int endTime(HHmm)) values
        /// </returns>
        /// <param name="filePath"> .txt file path </param>
        public static List<Break> ParseInitialEntries(string filePath)
        {
            var entries = new List<Break>();
            var lines = File.ReadLines(filePath);
            foreach (var line in lines)
            {
                if (ValidEntry(line))
                {
                    // split line into start and end values
                    string firstHalf = line.Substring(0, line.Length / 2);
                    string secondHalf = line.Substring(line.Length / 2, line.Length - line.Length / 2);

                    // remove ":", parse into integer values for sorting
                    int.TryParse(firstHalf.Remove(2, 1), out int startTime);
                    int.TryParse(secondHalf.Remove(2, 1), out int endTime);
                    Break entry = new(startTime, endTime);
                    entries.Add(entry);
                }
                else
                {
                    Console.WriteLine("bad entry ignored: " + line);
                }
            }
            return entries;
        }


        /// <summary>
        /// Parses manual entry
        /// </summary>
        /// <returns>
        /// Break object
        /// </returns>
        /// <param name="input">  </param>
        /// <returns></returns>
        public static Break ParseEntry(string input) 
        {
            // split line into start and end values
            string firstHalf = input.Substring(0, input.Length / 2);
            string secondHalf = input.Substring(input.Length / 2, input.Length - input.Length / 2);

            // remove ":", parse into integer values for sorting
            int.TryParse(firstHalf.Remove(2, 1), out int startTime);
            int.TryParse(secondHalf.Remove(2, 1), out int endTime);
            Break entry = new(startTime, endTime);
            return entry;
        }


        /// <summary>
        /// checks if entry matches regular expression
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool ValidEntry(string input)
        {
            string regexPattern = @"^(2[0-3]|[01]{1}[0-9]):([0-5]{1}[0-9])(2[0-3]|[01]{1}[0-9]):([0-5]{1}[0-9])$";

            if (Regex.IsMatch(input, regexPattern))
            {
                return true;
            }
            return false;
        }


        /// Code adapted from github by Goran Šiška
        /// source: https://github.com/GoranSiska/rxrdg
        /// <summary>
        /// Generates random data in correct format HH:mmHH:mm and writes it into a file
        /// </summary>
        /// <param name="filePath">full file path</param>
        /// <param name="amount">amount of data generated</param>
        public static void GenerateData(string filePath, int amount)
        {
            var param = @"(2[0-3]|[01]{1}[0-9]):([0-5]{1}[0-9])(2[0-3]|[01]{1}[0-9]):([0-5]{1}[0-9])";
            var rxrdg = new RegExpDataGenerator(param);

            using (StreamWriter writer = new StreamWriter(filePath)) 
            {
                for (var i = 0; i < amount; i++)
                {
                    writer.WriteLine(rxrdg.Next());
                }
            }
            
        }

        /// <summary>
        /// calculates and displays busiest time
        /// </summary>
        /// <param name="breakList"> List of breaks </param>
        public static void GetBusiest(List<Break> breakList)
        {
            Count(breakList);
        }

        public static void Count(List<Break> breakList) 
        {
            // sorted asc. by endTime
            var sortedList = breakList.OrderBy(o => o.endTime).ToList();
            var runnerCap = sortedList.Last().endTime; 

            // sorted asc. by startTime
            var startTimeCheckList = breakList.OrderBy(o => o.startTime).ToList();
            var runnerStart = startTimeCheckList.First().startTime;

            var concurrentBreaks = new List<Break>();
            var mostConcurrentBreaks = new List<Break>();

            for (int i = runnerStart; i < runnerCap;)
            {
                for (int j = 0; j < sortedList.Count; j++)
                {

                    if (concurrentBreaks.Contains(sortedList[j]))
                    {
                        // out of range breakTime handling
                        if (i > sortedList[j].endTime)
                        {
                            concurrentBreaks.Remove(sortedList[j]);

                            // remove breaks from iterable since they can't be a part of a time again
                            sortedList.RemoveAt(j);
                            startTimeCheckList.Remove(sortedList[j]);
                        }
                        continue;
                    }
                    else if (sortedList[j].startTime <= i && i <= sortedList[j].endTime)
                    {
                        concurrentBreaks.Add(sortedList[j]);
                    }
                }

                // update busiest period
                if (concurrentBreaks.Count > mostConcurrentBreaks.Count)
                {
                    mostConcurrentBreaks.Clear();
                    mostConcurrentBreaks = concurrentBreaks.ToList();
                }
                
                // update iterator i. Picks lowest value from remaining startTimes and endTimes
                if (i < sortedList.Last().endTime)
                {
                    var lowestEndTime = sortedList.Find(o => o.endTime > i).endTime;

                    if (i < startTimeCheckList.Last().startTime)
                    {
                        var lowestStartTime = startTimeCheckList.Find(o => o.startTime > i).startTime;
                        if (lowestStartTime <= lowestEndTime) { i = lowestStartTime; }
                        else { i = lowestEndTime; }

                    } else{ i = lowestEndTime; }
                }
            }

            // Max startTime and Min endTime in list contain the busiest period
            var busiestStartTime = mostConcurrentBreaks.MaxBy(o => o.startTime).startTime;
            var busiestEndTime = mostConcurrentBreaks.MinBy(o => o.endTime).endTime;
            
            Display(busiestStartTime, busiestEndTime, mostConcurrentBreaks.Count);
        }


        public static void Display(int startTime, int endTime, int count)
        {
            string start = startTime.ToString();
            string end = endTime.ToString();

            start = start.Insert(start.Length - 2, ":");
            end = end.Insert(end.Length - 2, ":");

            Console.WriteLine($"\nBusiest period: {start} - {end} , with {count} drivers taking a break." );

        }
    }
}
