using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inocencio_P2
{
    public class Player
    {
        public Role Role { get; set; }
        public int PlayerId { get; set; }

        public Player(Role role, int playerId)
        {
            Role = role;
            PlayerId = playerId;
        }
    }
}