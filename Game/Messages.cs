using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class MyTurn { }

    public class PlaceShip
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int ShipIndex { get; set; }

        public PlaceShip(int x, int y, int shipIndex)
        {
            X = x;
            Y = y;
            ShipIndex = shipIndex;
        }
    }

    public class NewTurn { }

    public class FinishedPlacingShips { }

    public class StartWar { }

    public class TryingToHit
    {
        public int X { get; set; }
        public int Y { get; set; }
        public TryingToHit(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class CheckHit
    {
        public int X { get; set; }
        public int Y { get; set; }
        public CheckHit(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class HitResponse
    {
        public CheckHit CheckHit { get; set; }
        public bool isHit { get; set; }

        public HitResponse(CheckHit checkHit, bool isHit)
        {
            CheckHit = checkHit;
            this.isHit = isHit;
        }
    }

    public class GameOver
    {
        //player loser
        public int PlayerNum { get; set; }
        public string PlayerName { get; set; }

        public GameOver(int playerNum, string playerName)
        {
            PlayerNum = playerNum;
            PlayerName = playerName;
        }
    }

    public class DeclareWinner
    {
        public GameOver GameOver { get; set; }
        public int WinnerNum { get; set; }
        public string WinnerName { get; set; }

        public DeclareWinner(GameOver gameOver, int winnerNum, string winnerName)
        {
            GameOver = gameOver;
            WinnerNum = winnerNum;
            WinnerName = winnerName;
        }
    }

    public class Quit
    {
        public DeclareWinner DeclareWinner { get; set; }

        public Quit(DeclareWinner declareWinner)
        {
            DeclareWinner = declareWinner;
        }
    }

}
