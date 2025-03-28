using System;
using System.Collections.Generic;
using System.Threading;

namespace Inocencio_P2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            { 
                Console.WriteLine("Enter input values separated by space in the following order:");
                Console.WriteLine("n t h d t1 t2");

                string input = Console.ReadLine();
                string[] tokens = input.Split(' ');

                if (tokens.Length != 6)
                {
                    Console.WriteLine("Please provide exactly 6 numbers separated by spaces.");
                    return;
                }

                int maxInstances = int.Parse(tokens[0]);
                int numTanks = int.Parse(tokens[1]);
                int numHealers = int.Parse(tokens[2]);
                int numDPS = int.Parse(tokens[3]);
                int minTime = int.Parse(tokens[4]);
                int maxTime = int.Parse(tokens[5]);

                if (maxInstances <= 0 || numTanks <= 0 || numHealers <= 0 || numDPS <= 0 || minTime <= 0 || maxTime <= 0)
                {
                    Console.WriteLine("All input values must be greater than zero.");
                    return;
                }

                if (maxTime < minTime)
                {
                    Console.WriteLine("The maximum clear time (t2) cannot be less than the minimum clear time (t1).");
                    return;
                }

                int totalParties = Math.Min(numTanks, Math.Min(numHealers, numDPS / 3));
                Console.WriteLine($"Total parties that can be formed: {totalParties}");

                DungeonQueue dungeonQueue = new DungeonQueue(maxInstances, minTime, maxTime);

                int playerId = 1;
                for (int i = 0; i < numTanks; i++)
                {
                    dungeonQueue.EnqueuePlayer(new Player(Role.Tank, playerId++));
                }
                for (int i = 0; i < numHealers; i++)
                {
                    dungeonQueue.EnqueuePlayer(new Player(Role.Healer, playerId++));
                }
                for (int i = 0; i < numDPS; i++)
                {
                    dungeonQueue.EnqueuePlayer(new Player(Role.DPS, playerId++));
                }

                CancellationTokenSource cts = new CancellationTokenSource();

                Task monitorTask = dungeonQueue.MonitorDungeonStatuses(cts.Token);

                Task processingTask = dungeonQueue.Start(totalParties);
                processingTask.Wait();

                cts.Cancel();
                monitorTask.Wait();

                Console.WriteLine("All processing complete.");
                Console.WriteLine(" ");

                Console.WriteLine("Final Dungeon Statistics:");
                foreach (var dungeon in dungeonQueue.Dungeons)
                {
                    Console.WriteLine($"Instance {dungeon.DungeonID} served for a total of {dungeon.TotalTimeServed} seconds and processed {dungeon.PartiesServed} parties.");
                }
                Console.WriteLine(" ");
                Console.WriteLine("Remaining players in queues:");
                Console.WriteLine($"Tanks: {dungeonQueue.RemainingTanks}");
                Console.WriteLine($"Healers: {dungeonQueue.RemainingHealers}");
                Console.WriteLine($"DPS: {dungeonQueue.RemainingDPS}");

                Console.ReadKey();
            }
            catch (FormatException fe)
            {
                Console.WriteLine("Input was not in the correct format. Please enter valid numbers.");
                Console.WriteLine(fe.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while processing inputs.");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
