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
        private string[ , ] board = new string[20, 60];

        public void WriteSnake()
        {
            board[10, 30] = head;
            board[11, 30] = x;
            board[12, 30] = x;
            board[13, 30] = x;
            board[14, 30] = x;
        }

        public void WriteBoard()
        {
            Console.Clear();

            for (int i = 0; i < Height; i ++)
            {
                for (int j = 0; j < Width; j ++)
                {
                    if (board[i, j] != null)
                    {
                        Console.SetCursorPosition(j, i);
                        Console.Write(board[i, j]);
                    }
                }
            }
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
                 
        }
    }
}

