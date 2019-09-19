using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Akka.Actor;
using SharedMess;

namespace Game
{
    public partial class GameForm : Form
    {
        IActorRef gameActor;
        Player player;
        string name;

        //own board
        int cellX;
        int cellY;
        //opponent board
        int opponentCellX;
        int opponentCellY;

        int currentShip;
        bool shipRotation; //true = horizontal
        bool[] shipDeployed = new bool[5];

        public GameForm()
        {
            
            InitializeComponent();

            List<Button> buttons = new List<Button> { btn_ship0, btn_ship1, btn_ship2, btn_ship3, btn_ship4, btn_rotate };

            player = new Player();
            name = GlobalContext.Name;

            Program.System = ActorSystem.Create("ClusterSystem");
            Props props = Props.Create(() => new GameActor(player, name, rtx_info, rtx_battleInfo, buttons)).WithDispatcher("akka.actor.synchronized-dispatcher");
            gameActor = Program.System.ActorOf(props);

            pic_shipDeploy.Image = GUI.gridImage;
            cellX = -1;
            cellY = -1;
            opponentCellX = -1;
            opponentCellY = -1;
            currentShip = -1;
            shipRotation = true;
        }

        //pic box -> mouse move event
        private void MouseMoveCoords(object sender, MouseEventArgs e)
        {
            if (currentShip != -1)
            {
                if (GUI.Coord(pic_shipDeploy, e.X) != -1 && GUI.Coord(pic_shipDeploy, e.Y) != -1)
                {
                    if (GUI.GetCell(GUI.Coord(pic_shipDeploy, e.X)) != cellX || GUI.GetCell(GUI.Coord(pic_shipDeploy, e.Y)) != cellY)
                    {
                        //rewrite mouse cell information
                        cellX = GUI.GetCell(GUI.Coord(pic_shipDeploy, e.X));
                        cellY = GUI.GetCell(GUI.Coord(pic_shipDeploy, e.Y));

                        //print coordinates
                        lbl_coords.Text = $"{e.X}, {e.Y}";
                        lbl_cells.Text = $"{cellX}, {cellY}";

                        pic_shipDeploy.Refresh();

                        if (shipRotation)
                        {
                            //deploy current ship with its color into the deck
                            for (int i = 0; i < GameLogic.shipLengths[currentShip]; i++)
                            {
                                //do not cross the boundaries
                                if (cellX + i <= 9)
                                {
                                    GUI.DrawInnerFrameCell(cellX + i, cellY, currentShip, this, pic_shipDeploy);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            //vertical rotation
                            for (int i = 0; i < GameLogic.shipLengths[currentShip]; i++)
                            {
                                if (cellY + i <= 9)
                                {
                                    GUI.DrawInnerFrameCell(cellX, cellY + i, currentShip, this, pic_shipDeploy);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    //out of boundaries
                    if (cellX != -1 || cellY != -1)
                    {
                        cellX = -1;
                        cellY = -1;

                        //redraw deck
                        pic_shipDeploy.Refresh();
                    }
                }
            }
        }

        //pic box -> mouse click event
        private void DeployShipPictureClick(object sender, EventArgs e)
        {
            if (currentShip != -1 && cellX != -1 && cellY != -1)
            {
                if (GameLogic.CanThereBeShip(currentShip, cellX, cellY, shipRotation, player.ShipSet))
                {
                    btn_rotate.Enabled = false;
                    //the current ship is being deployed
                    shipDeployed[currentShip] = true;

                    switch (currentShip)
                    {
                        case 0:
                            {
                                btn_ship0.Enabled = false;
                                break;
                            }
                        case 1:
                            {
                                btn_ship1.Enabled = false;
                                break;
                            }
                        case 2:
                            {
                                btn_ship2.Enabled = false;
                                break;
                            }
                        case 3:
                            {
                                btn_ship3.Enabled = false;
                                break;
                            }
                        case 4:
                            {
                                btn_ship4.Enabled = false;
                                break;
                            }
                    }

                    //deploy the current ship into the ship set
                    GameLogic.DeployShip(currentShip, cellX, cellY, shipRotation, player.ShipSet);

                    gameActor.Tell(new PlaceShip(cellX, cellY, currentShip));

                    pic_shipDeploy.Refresh();

                    //unselect ship
                    currentShip = -1;

                    //are all the ships already deployed
                    bool areAllShipsDeployed = true;
                    foreach (bool isDeployed in shipDeployed)
                    {
                        if (!isDeployed)
                        {
                            areAllShipsDeployed = false;
                        }
                    }

                    //if all the ships are deployed enable start button
                    if (areAllShipsDeployed)
                    {
                        btn_start.Enabled = true;
                    }
                }
            }
        }

        private void DrawShipPicture(object sender, PaintEventArgs e)
        {
            GUI.DrawShipSet(player.ShipSet, e);
        }

        private void btn_ship0_Click(object sender, EventArgs e)
        {
            currentShip = 0;
            btn_rotate.Enabled = true;
        }

        private void btn_ship1_Click(object sender, EventArgs e)
        {
            currentShip = 1;
            btn_rotate.Enabled = true;
        }

        private void btn_ship2_Click(object sender, EventArgs e)
        {
            currentShip = 2;
            btn_rotate.Enabled = true;
        }

        private void btn_ship3_Click(object sender, EventArgs e)
        {
            currentShip = 3;
            btn_rotate.Enabled = true;
        }

        private void btn_ship4_Click(object sender, EventArgs e)
        {
            currentShip = 4;
            btn_rotate.Enabled = true;
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            if (player.PlayerNumber == 1)
            {
                gameActor.Tell(new FinishedPlacingShips());
                btn_start.Enabled = false;
            }
                
            else
            {
                gameActor.Tell(new StartWar());
                player.MyTurn = true;
                btn_start.Enabled = false;
            }              
        }

        private void btn_rotate_Click(object sender, EventArgs e)
        {
            shipRotation = !shipRotation;
        }

        //SECOND STAGE -> WAR
        private void MouseMoveOpponentCoords(object sender, MouseEventArgs e)
        {
            if (player.MyTurn == true)
            {
                //are we on second grid
                if (GUI.Coord(pic_Opponent, e.X) != -1 && GUI.Coord(pic_Opponent, e.Y) != -1)
                {
                    //if cell is changed
                    if (GUI.GetCell(GUI.Coord(pic_Opponent, e.X)) != cellX || GUI.GetCell(GUI.Coord(pic_Opponent, e.Y)) != cellY)
                    {
                        opponentCellX = GUI.GetCell(GUI.Coord(pic_Opponent, e.X));
                        opponentCellY = GUI.GetCell(GUI.Coord(pic_Opponent, e.Y));

                        lbl_oppCoords.Text = $"{e.X}, {e.Y}";
                        lbl_oppCell.Text = $"{opponentCellX}, {opponentCellY}";

                        pic_Opponent.Refresh();

                        GUI.DrawInnerFrameCell(opponentCellX, opponentCellY, 5, this, pic_Opponent);
                    }
                }
                else
                {
                    opponentCellX = -1;
                    opponentCellY = -1;

                    pic_Opponent.Refresh();
                }
            }            
        }

        private void DrawOpponentHit(object sender, PaintEventArgs e)
        {
            GUI.DrawOpponentBoard(player.IsOpponentHit, player.OpponentBoard, e);
        }

        private void MouseClickOpponentGrid(object sender, EventArgs e)
        {
            if (opponentCellX != -1 && opponentCellY != -1)
            {
                gameActor.Tell(new TryingToHit(opponentCellX, opponentCellY));
            }
        }
    }
}
