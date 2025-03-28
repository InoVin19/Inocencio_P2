using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inocencio_P2
{
    class Party
    {
        public Player Tank { get; set; }
        public Player Healer { get; set; }
        public List<Player> DPS { get; set; }

        public Party(Player tank, Player healer, List<Player> dpsPlayers)
        {
            if (dpsPlayers.Count != 3)
            {
                throw new ArgumentException("A party must have exactly 3 DPS players.");
            }
            Tank = tank;
            Healer = healer;
            DPS = new List<Player>(dpsPlayers);

        }
    }
}
