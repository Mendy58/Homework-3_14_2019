using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesdbLibrary
{
    public class Game
    {
        public int Id { get; set; }
        public DateTime datetime { get; set; }
        public int MaxPlayers { get; set; }
        public int PLayerCnt { get; set; }
        public IEnumerable<Player> Players { get; set; }
    }
}
