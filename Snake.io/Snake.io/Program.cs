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

            snake.WriteSnake();
            snake.WriteBoard();
            Console.ReadKey();
        }
    }
}
