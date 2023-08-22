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
            replay:
            SnakeIO snake = new SnakeIO();
            snake.RunGame();

            Console.Write("Press enter to replay:");
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey();
                    if (key.Key == ConsoleKey.Enter)
                    {
                        goto replay;
                    }
                }
            }
        }
    }
}
