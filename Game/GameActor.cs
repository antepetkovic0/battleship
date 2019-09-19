using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster;
using System.Windows.Forms;
using SharedMess;
using System.Threading;

namespace Game
{
    class GameActor : ReceiveActor
    {
        public IActorRef opponent;
        public Player player { get; set; }
        private string username;
        RichTextBox rtxBox;
        RichTextBox rtxBox2;
        List<Button> listButtons;

        public GameActor(Player plyer, string name, RichTextBox rtx, RichTextBox rtx2, List<Button> buttons)
        {
            opponent = null;
            player = plyer;
            username = name;
            rtxBox = rtx;
            rtxBox2 = rtx2;
            listButtons = buttons;

            Receive<NewPlayer>(x => HandleSetup(x));

            //disable second player actions
            Receive<MyTurn>(x => HandleMyTurn());

            Receive<PlaceShip>(x => HandlePlaceShip(x));

            //tellin second player its his turn
            Receive<FinishedPlacingShips>(x => HandleFinishedPlacingShips());

            //disabling first player actions
            Receive<NewTurn>(x => HandleNewTurn());

            //start hitin opponent ships
            Receive<StartWar>(x => HandleStartWar());

            Receive<TryingToHit>(x => HandleTryingToHit(x));

            //check hit; if hit check if all ships sunk
            Receive<CheckHit>(x => HandleCheckHit(x));

            Receive<HitResponse>(x => HandleHitResponse(x));

            //tell opponent your ships are sunk 
            Receive<GameOver>(x => HandleGameOver(x));

            //quit game
            Receive<DeclareWinner>(x => HandleDeclareWinner(x));
            Receive<Quit>(x => HandleQuit(x));
        }

        private void HandleSetup(NewPlayer x)
        {
            opponent = Sender;
            if (x.PlayerNum == 1)
            {
                player.PlayerNumber = 1; //for start war later
                rtxBox.Text = $"Hello {username}, you're {x.PlayerNum}. player and you can deploy ships.";
                opponent.Tell(new MyTurn());
            }
            else
            {
                player.PlayerNumber = 2;
            }            
        }

        private void HandleMyTurn()
        {
            listButtons.ForEach(x => x.Enabled = false);
        }

        private void HandlePlaceShip(PlaceShip x)
        {
            rtxBox.Text = $"Ship {x.ShipIndex} deployed on coordinates {x.X},{x.Y}.";
        }

        private void HandleFinishedPlacingShips()
        {
            rtxBox.Text = "Wait other player to place ships.";
            opponent.Tell(new NewTurn());
        }

        private void HandleNewTurn()
        {
            rtxBox.Text = $"Hello {username}, you're {player.PlayerNumber}. player and it's your turn to deploy ships.";
            listButtons.ForEach(x => x.Enabled = true);
        }

        private void HandleStartWar()
        {
            if (player.MyTurn)
            {
                rtxBox.Text = $"War has begun, {player.Name} u can start hitting ships.";
            }            
        }

        private void HandleTryingToHit(TryingToHit x)
        {
            rtxBox2.Text += $"{username} targeting coordinates {x.X},{x.Y}.\n";
            player.MyTurn = false;

            var for_checkHit = new CheckHit(x.X, x.Y);
            opponent.Tell(for_checkHit);

            //for draw hit or miss image on opponent grid
            player.IsOpponentHit[x.X, x.Y] = true;
        }

        private void HandleCheckHit(CheckHit x)
        {
            player.MyTurn = true;
            //tell attacker if its hit
            var isHit = IsShipHitted(x.X, x.Y);
            var hitResponse = new HitResponse(x, isHit);
            if (isHit)
            {
                rtxBox2.Text += $"I have been attacked on {x.X},{x.Y}; HIT.\n";
                var isOver = IsGameOver(player.ShipSet);
                
                if (isOver == true)
                {
                    rtxBox2.Text += $"GAME OVER";
                    opponent.Tell(new GameOver(player.PlayerNumber, username));
                }
                else
                {
                    opponent.Tell(hitResponse);
                }
            }
            else
            {
                rtxBox2.Text += $"I have been attacked on {x.X},{x.Y}; MISS.\n";
                opponent.Tell(hitResponse);
            }
        }

        private bool IsShipHitted(int x, int y)
        {
            if (player.ShipSet[x, y] != -1)
            {
                player.ShipSet[x, y] = -1;
                return true;
            }
            else
            {
                return false;
            }
            
        }

        private bool IsGameOver(int[,] myboard)
        {
            var brojac = 0;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (myboard[i, j] == -1)
                        brojac++;
                }
            }
            if (brojac == 100)
                return true;
            else
                return false;
        }

        private void HandleHitResponse(HitResponse x)
        {
            if (x.isHit)
            {
                rtxBox2.Text += $"My shot on coordinate {x.CheckHit.X},{x.CheckHit.Y} was HIT.\n";
                player.OpponentBoard[x.CheckHit.X, x.CheckHit.Y] = 1;
            }               
            else
                rtxBox2.Text += $"My shot on coordinate {x.CheckHit.X},{x.CheckHit.Y} was MISS.\n";
        }

        private void HandleGameOver(GameOver x)
        {
            var winner = new DeclareWinner(x, player.PlayerNumber, username);
            opponent.Tell(winner);
        }

        private void HandleDeclareWinner(DeclareWinner x)
        {
            Thread.Sleep(1000);
            MessageBox.Show($"GAME OVER\n" +
                $"WINNER :> {x.WinnerNum}. player => {x.WinnerName}\n" +
                $"LOSER :> {x.GameOver.PlayerNum}. player => {x.GameOver.PlayerName}");

            var quit = new Quit(x);
            opponent.Tell(x);

            Application.Exit();
        }

        private void HandleQuit(Quit x)
        {
            MessageBox.Show($"GAME OVER\n" +
                $"WINNER :> {x.DeclareWinner.WinnerNum}. player => {x.DeclareWinner.WinnerName}\n" +
                $"LOSER :> {x.DeclareWinner.GameOver.PlayerNum}. player => {x.DeclareWinner.GameOver.PlayerName}");

            Application.Exit();
        }
    }
}
