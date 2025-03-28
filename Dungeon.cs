using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inocencio_P2
{
    internal class Dungeon
    {
        public int DungeonID { get; set; }
        public bool IsActive { get; set; }
        public int PartiesServed { get; set; }
        public int TotalTimeServed { get; set; }

        public Dungeon(int dungeonID) 
        {
            DungeonID = dungeonID;
            IsActive = false;
            PartiesServed = 0;
            TotalTimeServed = 0;
        }

        public string Status => IsActive ? "Active" : "Empty";

        public void RunInstance(Party party, int currentPartyNumber, int minTime, int maxTime)
        {
            IsActive = true;
            PartiesServed++;
            int partyNumber = PartiesServed;

            Random rand = new Random();
            int duration = rand.Next(minTime, maxTime + 1);

            Console.WriteLine($"Instance {DungeonID} is now serving party {currentPartyNumber}.");

            Thread.Sleep(duration * 1000);

            TotalTimeServed += duration;

            IsActive = false;
            Console.WriteLine($"Instance {DungeonID}: Party {currentPartyNumber} has completed the dungeon in {duration} seconds.");
        }
    }
}
