using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Input;
using System.Runtime.InteropServices;

namespace Snake.io
{
    public class SnakeIO
    {
        public enum Direction
        {
            Top,
            Right,
            Bottom,
            Left
        }

        //Höhe / Breite des Spielfeldes
        private int Height = 19;
        private int Width = 59;

        //Schlange
        private string body = "x";
        private string head = "o";

        //Bereich in der die Schlange erstellt wird
        private string[,] board = new string[19, 59];

        private int startPositionHeadX = 10;
        private int startPositionHeadY = 30;

        private int startPositionBodyX = 11;
        private int startPositionBodyY = 30;

        //Hier werden die Kordinaten der Schlange gespeichert
        private List<Tuple<int, int>> wholeBody = new List<Tuple<int, int>>();

        //Gibt die Richtung an in die die Schlange schluss entlich geht
        private Direction direction = Direction.Top;


        public void RunGame()
        {

            WriteBoard();
            InitSnake();

            var gameOver = false;
            while (!gameOver)
            {
                PrintSnake();

                Thread.Sleep(300);

                // Todo: check for keys
                KeyEvent();
                // Todo: Check for collision

                MoveSnake();
            }
        }

        public void KeyEvent()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        direction = Direction.Top;
                        break;
                    case ConsoleKey.RightArrow:
                        direction = Direction.Right;
                        break;
                    case ConsoleKey.DownArrow:
                        direction = Direction.Bottom;
                        break;
                    case ConsoleKey.LeftArrow:
                        direction = Direction.Left;
                        break;
                }
            }
        }

        public void CheckCollision()
        {

        }

        public void MoveSnake()
        {
            // Last round head = body
            board[startPositionHeadX, startPositionHeadY] = body;

            startPositionBodyX = startPositionHeadX;
            startPositionBodyY = startPositionHeadY;

            
            //  Move head into correct direction
            if (direction == Direction.Top)
            {
                startPositionHeadX--;
            }
            else if (direction == Direction.Right)
            {
                startPositionHeadY += 2;
            }
            else if (direction == Direction.Bottom)
            {
                startPositionHeadX++;
            }
            else if (direction == Direction.Left)
            {
                startPositionHeadY -= 2;
            }

            board[startPositionHeadX, startPositionHeadY] = head;

            board[wholeBody.Last().Item1, wholeBody.Last().Item2] = null;

            // Save new body in list
            wholeBody.Insert(0, new Tuple<int, int>(startPositionHeadX, startPositionHeadY));
            wholeBody.RemoveAt(wholeBody.Count - 1);

        }
        public void PrintSnake()
        {
            for (int i = 1; i < Height; i++)
            {
                for (int j = 1; j < Width; j++)
                {
                    if (board[i, j] != null)
                    {
                        Console.SetCursorPosition(j, i);
                        Console.Write(board[i, j]);
                    }
                    else
                    {
                        Console.SetCursorPosition(j - 1, i - 1);
                        Console.Write(' ');
                    }
                }
            }
        }

        public void InitSnake()
        {
            board[startPositionHeadX, startPositionHeadY] = head;

            board[startPositionBodyX, startPositionBodyY] = body;
            board[startPositionBodyX+1, startPositionBodyY] = body;
            board[startPositionBodyX+2, startPositionBodyY] = body;
            board[startPositionBodyX+3, startPositionBodyY] = body;
            board[startPositionBodyX+4, startPositionBodyY] = body;

            wholeBody.Add(new Tuple<int, int>(startPositionHeadX, startPositionHeadY));
            wholeBody.Add(new Tuple<int, int>(startPositionBodyX, startPositionBodyY));
            wholeBody.Add(new Tuple<int, int>(startPositionBodyX + 1, startPositionBodyY));
            wholeBody.Add(new Tuple<int, int>(startPositionBodyX + 2, startPositionBodyY));
            wholeBody.Add(new Tuple<int, int>(startPositionBodyX + 3, startPositionBodyY));
            wholeBody.Add(new Tuple<int, int>(startPositionBodyX + 4, startPositionBodyY));
        }

        public void WriteBoard()
        {
            for (int i = 0; i <= (Width); i++)
            {
                Console.SetCursorPosition(i, 0);
                Console.Write("-");
            }

            for (int i = 0; i <= (Width); i++)
            {
                Console.SetCursorPosition(i, (Height));
                Console.Write("-");
            }

            for (int i = 0; i <= (Height); i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write("|");
            }

            for (int i = 0; i <= (Height); i++)
            {
                Console.SetCursorPosition((Width), i);
                Console.Write("|");
            }
        }
    }
}


