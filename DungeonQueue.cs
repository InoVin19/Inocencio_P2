using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inocencio_P2
{
    internal class DungeonQueue
    {
        SemaphoreSlim s;
        private readonly object queueLock = new object();
        private int minTime;
        private int maxTime;
        private int lastUsedInstanceIndex = 0;

        private Queue<Player> tankQueue;
        private Queue<Player> healerQueue;
        private Queue<Player> dpsQueue;

        private List<Dungeon> dungeonInstances;
        public IReadOnlyList<Dungeon> Dungeons => dungeonInstances.AsReadOnly();

        public DungeonQueue(int maxInstances, int minTime, int maxTime)
        {
            s = new SemaphoreSlim(maxInstances);
            tankQueue = new Queue<Player>();
            healerQueue = new Queue<Player>();
            dpsQueue = new Queue<Player>();
            dungeonInstances = new List<Dungeon>();
            this.minTime = minTime;
            this.maxTime = maxTime;
            
            for (int i = 1; i <= maxInstances; i++)
            {
                dungeonInstances.Add(new Dungeon(i));
            }
        }

        public void EnqueuePlayer(Player player)
        {
            lock (queueLock)
            {
                switch (player.Role)
                {
                    case Role.Tank:
                        tankQueue.Enqueue(player);
                        break;
                    case Role.Healer:
                        healerQueue.Enqueue(player);
                        break;
                    case Role.DPS:
                        dpsQueue.Enqueue(player);
                        break;
                }
            }
        }

        public Task Start(int numParties)
        {
            return Task.Run(() => ProcessQueues(numParties));
        }

        private void ProcessQueues(int numParties)
        {
            int formedParties = 0;
            List<Task> tasks = new List<Task>();

            while (formedParties < numParties)
            {
                Party party = FormParty();
                if (party != null)
                {
                    formedParties++;
                    int currentPartyNumber = formedParties;
                    Task task = Task.Run(() => ProcessParty(party, currentPartyNumber));
                    tasks.Add(task);
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
            Task.WaitAll(tasks.ToArray());

            Console.WriteLine(" ");
            Console.WriteLine("All parties have been served.");
        }

        private Party FormParty()
        {
            lock (queueLock)
            {
                if (tankQueue.Count > 0 && healerQueue.Count > 0 && dpsQueue.Count >= 3)
                {
                    Player tank = tankQueue.Dequeue();
                    Player healer = healerQueue.Dequeue();
                    List<Player> dpsPlayers = new List<Player>();
                    for (int i = 0; i < 3; i++)
                    {
                        dpsPlayers.Add(dpsQueue.Dequeue());
                    }
                    return new Party(tank, healer, dpsPlayers);
                }
                else
                {
                    return null;
                }
            }
        }

        private void ProcessParty(Party party, int currentPartyNumber)
        {
            s.Wait();
            try
            {
                Dungeon instance = GetAvailableDungeonInstance();
                if (instance != null)
                {
                    instance.RunInstance(party, currentPartyNumber, minTime, maxTime);
                }
            }
            finally
            {
                s.Release();
            }
        }

        private Dungeon GetAvailableDungeonInstance()
        {
            lock (dungeonInstances)
            {
                int instanceCount = dungeonInstances.Count;
                for (int i = 0; i < instanceCount; i++)
                {
                    int index = (lastUsedInstanceIndex + i + 1) % instanceCount;
                    if (!dungeonInstances[index].IsActive)
                    {
                        lastUsedInstanceIndex = index;
                        return dungeonInstances[index];
                    }
                }
            }
            return null;
        }

        private void PrintStatuses()
        {
            lock (dungeonInstances)
            {
                Console.WriteLine(" ");
                Console.WriteLine($"Instance Statuses at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                foreach (var dungeon in dungeonInstances)
                {
                    Console.WriteLine($"Instance {dungeon.DungeonID}: {dungeon.Status}");
                }
                Console.WriteLine(" ");
            }
        }

        public async Task MonitorDungeonStatuses(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    PrintStatuses();
                    await Task.Delay(5000, token);
                }
            }
            catch (TaskCanceledException)
            {
                
            }
            finally
            {
                PrintStatuses();
            }
        }

        public int RemainingTanks
        {
            get { lock (queueLock) { return tankQueue.Count; } }
        }

        public int RemainingHealers
        {
            get { lock (queueLock) { return healerQueue.Count; } }
        }

        public int RemainingDPS
        {
            get { lock (queueLock) { return dpsQueue.Count; } }
        }
    }
}
