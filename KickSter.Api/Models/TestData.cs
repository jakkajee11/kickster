using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KickSter.Models
{
    public class TestData
    {
        public static IEnumerable<Player> Players()
        {
            var players = new List<Player>();
            players.Add(new Player
            {
                Name = "Player1",                
            });
            players.Add(new Player
            {
                Name = "Player2",
            });
            players.Add(new Player
            {
                Name = "Player3",
            });
            players.Add(new Player
            {
                Name = "Player4",
            });
            players.Add(new Player
            {
                Name = "Player5",
            });

            return players;
        }
    }
}