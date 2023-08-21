using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Snake.io
{
    public class SnakeIO
    {
        //Höhe / Breite des Spielfeldes
        private int Height = 20;
        private int Width = 60;

        //Schlange
        private string x = "x";
        private string head = "o";

        //Bereich in der die Schlange erstellt wird
        private string[,] board = new string[20, 60];

        private int startPositionHeadX = 10;
        private int startPositionHeadY = 30;

        private int startPositionBodyX = 11;
        private int startPositionBodyY = 30;
        public void WriteBoard()
        {

            for (int i = 0; i <= (Width); i++)
            {
                Console.SetCursorPosition(i + 1, 1);
                Console.Write("-");
            }

            for (int i = 0; i <= (Width); i++)
            {
                Console.SetCursorPosition(i + 1, (Height + 1));
                Console.Write("-");
            }

            for (int i = 0; i <= (Height); i++)
            {
                Console.SetCursorPosition(1, i + 1);
                Console.Write("|");
            }

            for (int i = 0; i <= (Height); i++)
            {
                Console.SetCursorPosition((Width + 1), i + 1);
                Console.Write("|");
            }
        }
        public void MoveSnake()
        {
            startPositionBodyX--;
            startPositionHeadX--;
            board[startPositionHeadX, startPositionHeadY] = head;
            board[startPositionBodyX, startPositionBodyY] = x;

            for (int i = 1; i < Height; i++)
            {
                for (int j = 1; j < Width; j++)
                {
                    if (board[i, j] != null)
                    {
                        foreach (char k in board[i, j])
                        {
                            if (k == 'o')
                            {
                                Console.Write(board[i, j]);
                                board[i, j] = null;
                            }
                            if (k == 'x')
                            {
                                Console.SetCursorPosition(j, i);
                                Console.Write(board[i, j]);
                            }
                        }
                    }
                }
            }
        }

    }
}

