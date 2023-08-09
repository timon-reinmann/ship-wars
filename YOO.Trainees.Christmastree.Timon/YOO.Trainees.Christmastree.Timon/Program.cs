using System.ComponentModel.Design;
using System.Reflection.Metadata.Ecma335;

public class Program
{
    static void Main(string[] args)
    {
        int height;
        int color;

        Console.WriteLine("enter the desired height of your christmas tree:");
        while (!int.TryParse(Console.ReadLine(), out height))
        {
            Console.WriteLine("that isn't a number, enter a new number:");
        }

        Console.WriteLine("do you want decoration on your christmas tree (yes/no):");
        string answer = Console.ReadLine();

        Console.WriteLine("what color should your Christmas tree be (1 = blue, 2 = green, 3 = red, 4 = yellow, 5 = pink, 6 = white, 7 = random):");
        while (!int.TryParse(Console.ReadLine(), out color))
        {
            Console.WriteLine("that isn't a number, enter a new number:");
        }


        var star = "*";
        var star2 = "**";
        var starDecoration = "o*";
        var treeIntent = height - 2;
        var stemIntent = height - 3;
        var space = " ";
        var colorDecoration = Console.ForegroundColor = ConsoleColor.White;
        var rnd = new Random();


        if (1 == color)
        {
            colorDecoration = ConsoleColor.Blue;
        }
        if (2 == color)
        {
            colorDecoration = ConsoleColor.Green;
        }
        if (3 == color)
        {
            colorDecoration = ConsoleColor.Red;
        }
        if (4 == color)
        {
            colorDecoration = ConsoleColor.Yellow;
        }
        if (5 == color)
        {
            colorDecoration = ConsoleColor.Magenta;
        }
        if (6 == color)
        {
            colorDecoration = ConsoleColor.White;
        }
        if (7 == color)
        {
            if (answer == "yes") { }
            else
            {
                starDecoration = star2;
            }

            for (int i = 0; i < height; i++)
            {
                for (int j = treeIntent; 0 <= j; j--)
                {
                    Console.Write(space);
                }

                treeIntent--;

                foreach (char c in star)
                {
                    if (c == '*')
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(c);
                        Console.ResetColor();
                    }
                    else if (c == 'o')
                    {
                        Console.ForegroundColor = (ConsoleColor)rnd.Next(9, 16);
                        Console.Write(c);
                        Console.ResetColor();

                    }
                }
                Console.WriteLine();
                star += starDecoration;
            }

            for (int i = stemIntent; 0 <= i; i--)
            {
                Console.Write(space);
            }
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("| |");
            Console.ResetColor();
        }
        else
        {
            if (answer == "yes") { }
            else
            {
                starDecoration = star2;
            }

            for (int i = 0; i < height; i++)
            {
                for (int j = treeIntent; 0 <= j; j--)
                {
                    Console.Write(space);
                }

                treeIntent--;

                foreach (char c in star)
                {
                    if (c == '*')
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(c);
                        Console.ResetColor();
                    }
                    else if (c == 'o')
                    {
                        Console.ForegroundColor = colorDecoration;
                        Console.Write(c);
                        Console.ResetColor();

                    }
                }
                Console.WriteLine();
                star += starDecoration;
            }

            for (int i = stemIntent; 0 <= i; i--)
            {
                Console.Write(space);
            }
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("| |");
            Console.ResetColor();
        }
    }
}
