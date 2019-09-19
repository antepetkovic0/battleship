using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    static class GameLogic
    {
        public static int[] shipLengths = new int[5] { 2, 3, 3, 4, 5 };

        public static bool CanThereBeShip(int currentShip, int cellX, int cellY, bool isHorizontal, int[,] shipSet)
        {
            if (cellX < 0 || cellY < 0)
            {
                return false;
            }

            if (isHorizontal)
            {
                if (cellX + GameLogic.shipLengths[currentShip] - 1 <= 9)
                {
                    //searching for invalid layout on grid
                    for (int i = Math.Max(0, cellX - 1); i <= Math.Min(9, cellX + GameLogic.shipLengths[currentShip]); i++)
                    {
                        for (int j = Math.Max(0, cellY - 1); j <= Math.Min(9, cellY + 1); j++)
                        {
                            if (shipSet[i, j] != -1)
                            {
                                //invalid layout
                                return false;
                            }
                        }
                    }
                    return true;
                }
                else
                {
                    //out of the bounds
                    return false;
                }
            }
            else
            {
                //vertical
                if (cellY + GameLogic.shipLengths[currentShip] - 1 <= 9)
                {
                    for (int i = Math.Max(0, cellX - 1); i <= Math.Min(9, cellX + 1); i++)
                    {
                        for (int j = Math.Max(0, cellY - 1); j <= Math.Min(9, cellY + GameLogic.shipLengths[currentShip]); j++)
                        {
                            if (shipSet[i, j] != -1)
                            {
                                return false;
                            }
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        static public void DeployShip(int currentShip, int cellX, int cellY, bool isHorizontal, int[,] shipSet)
        {
            if (isHorizontal)
            {
                for (int i = 0; i < GameLogic.shipLengths[currentShip]; i++)
                {
                    //deploy into shipset
                    shipSet[cellX + i, cellY] = currentShip;
                }
            }
            else
            {
                for (int i = 0; i < GameLogic.shipLengths[currentShip]; i++)
                {
                    //deploy
                    shipSet[cellX, cellY + i] = currentShip;
                }
            }

        }
    }
}
