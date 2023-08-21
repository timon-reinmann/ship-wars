using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake.io
{
    public class Program
    {
        static public void Main(String[] args)
        {
            SnakeIO snake = new SnakeIO();

            snake.WriteBoard();
            for (int i = 0; i < 5; i++)
            {
                snake.MoveSnake();
                if (i == 5)
                {
                    Console.Clear();
                    i = 0;
                }
            }
            Console.ReadKey();
        }
    }
}
