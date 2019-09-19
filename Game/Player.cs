using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class Player
    {
        public string Name { get; set; }
        public int PlayerNumber { get; set; }
        public int[,] ShipSet { get; set; }

        public bool MyTurn { get; set; }

        //for drawing hits on opponent grid
        public int[,] OpponentBoard { get; set; }
        public bool[,] IsOpponentHit { get; set; }

        public Player()
        {
            MyTurn = false;

            ShipSet = new int[10, 10];
            OpponentBoard = new int[10, 10];
            IsOpponentHit = new bool[10, 10];

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    ShipSet[i, j] = -1;
                    OpponentBoard[i, j] = -1;
                    IsOpponentHit[i, j] = false;
                }
            }
        }
    }
}
