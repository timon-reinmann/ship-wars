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
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection;
using static Snake.io.SnakeIO;
using System.Threading.Tasks.Sources;

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
        private string body = "o";
        private string head = "^";

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

        private Random rnd = new Random();
        private string point = "%";

        private bool Points = false;

        private int score = 0;

        private int pointsX = 0;
        private int pointsY = 0;


        private int speed = 400;

        public void RunGame()
        {

            InitSnake();
            checkPoint();

            WriteBoard();


            var gameOver = false;
            while (!gameOver)
            {
                PrintSnake();

                Thread.Sleep(speed);

                // Todo: check for keys
                KeyEvent();

                // Todo: check for points
                EatPoints();

                // Todo: Show points
                Console.SetCursorPosition(61, 1);
                Console.Write("Score: " + score);

                // Todo: Check for collision
                if (!MoveSnake())
                {
                    gameOver = true;
                    Console.Clear();
                    Console.WriteLine("you lost");
                    Console.WriteLine("Your score was: " + score.ToString());

                }


            }
        }

        public void SpeedUp()
        {
            if (speed > 10)
            {
                if (speed > 50)
                {
                    if (score % 3 == 0)
                    {
                        speed -= 50;
                    }
                }
                else
                {
                    if (score % 3 == 0)
                    {
                        speed -= 10;
                    }
                }
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
                        if (direction != Direction.Bottom)
                        {
                            direction = Direction.Top;
                            head = "^";
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        if (direction != Direction.Left)
                        {
                            direction = Direction.Right;
                            head = ">";
                        }
                        break;
                    case ConsoleKey.DownArrow:

                        if (direction != Direction.Top)
                        {
                            direction = Direction.Bottom;
                            head = "v";
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                        if (direction != Direction.Right)
                        {
                            direction = Direction.Left;
                            head = "<";
                        }
                        break;
                }
            }
        }

        public void EatPoints()
        {

            if (wholeBody.First().Item1 == pointsX && wholeBody.First().Item2 == pointsY)
            {
                board[pointsX, pointsY] = " ";
                score++;

                checkPoint();
                Points = true;
            }
            else
            {
                Points = false;
            }
        }

        public bool MoveSnake()
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

            if (wholeBody.First().Item1 == 0 || wholeBody.First().Item2 == 0 || wholeBody.First().Item2 == Width - 2 || wholeBody.First().Item1 == Height)
            {
                return false;
            }

            for (int i = 1; i <= wholeBody.Count - 1; i++)
            {
                if (wholeBody.First().Item1 == wholeBody[i].Item1 && wholeBody.First().Item2 == wholeBody[i].Item2)
                {
                    return false;
                }
            }
            board[wholeBody.Last().Item1, wholeBody.Last().Item2] = null;

            // Save new body in list
            wholeBody.Insert(0, new Tuple<int, int>(startPositionHeadX, startPositionHeadY));

            if (!Points)
            {
                wholeBody.RemoveAt(wholeBody.Count - 1);
            }


            return true;

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
                        Console.SetCursorPosition(j, i);
                        Console.Write(' ');
                    }
                }
            }
        }

        public void InitSnake()
        {
            board[startPositionHeadX, startPositionHeadY] = head;

            board[startPositionBodyX, startPositionBodyY] = body;
            board[startPositionBodyX + 1, startPositionBodyY] = body;
            board[startPositionBodyX + 2, startPositionBodyY] = body;
            board[startPositionBodyX + 3, startPositionBodyY] = body;
            board[startPositionBodyX + 4, startPositionBodyY] = body;

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

        public void checkPoint()
        {
            pointsX = rnd.Next(1, 19);
            pointsY = rnd.Next(1, 59);

            bool test = false;

        again:
            while (!test)
            {
                for (int i = 0; i < wholeBody.Count; i++)
                {
                    if (pointsX == wholeBody[i].Item1 && pointsY == wholeBody[i].Item2 || pointsY % 2 != 0)
                    {
                        pointsX = rnd.Next(1, 19);
                        pointsY = rnd.Next(1, 59);
                        test = false;
                        goto again;
                    }
                    else
                    {
                        test = true;
                    }

                }
            }
            board[pointsX, pointsY] = point;
        }
    }
}


