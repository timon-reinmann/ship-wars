public class Program
{
    static void Main(string[] args)
    {

        bool visible = true;
        var rnd = new Random();
        int color = 0;
        string blink = "no";
        var colorDecoration = ConsoleColor.Black;
        var intcolorCount = 0;
        var colorCount = 1;
        var star = "*";
        var star2 = "**";
        var starDecoration = "o*";
        var space = " ";
        var allColors = new Dictionary<int, int>();

        //Abfrage wie hoch der Weihnachtsbaum sein sollte.
        int height = GetHeight();

        //Abfrag ob Weihnachtsbaum Dekorat an oder ab;
        string answer = GetChrismastreeDecoration();

        if (answer == "yes")
        {
            //Abfrage ob Dekoration blinkt
            blink = GetBlink();
            if (blink == "no")
            {
                //Abfrage wie viele farben die Dekoratin haben soll
                colorCount = GetColorCount();

                if (colorCount == 1)
                {
                    //Abfrage welche farben die Dekoration haben soll
                    color = GetColor();

                    //Farbe von der Dekoration wird ausgewertet
                    colorDecoration = GetForegroundColor(color);
                }
                else
                {
                    for (int i = 0; i < colorCount; i++)
                    {
                        intcolorCount = GetIntColorCount(i);
                        allColors.Add(i, intcolorCount);
                    }
                }
            }
        }


        var treeIntent = height - 2;
        var stemIntent = height - 3;


        //Auswerting von Get ChrismastreeDecoration
        starDecoration = GetDecoration(answer, star2, starDecoration);

        //Auswertung/erstellung von GetBlink (Weihnachtsbaum wird gezeichnet)
        CreateBlink(visible, rnd, color, blink, colorDecoration, height, star, starDecoration, treeIntent, stemIntent, colorCount, space);

    }

    private static void CreateBlink(bool visible, Random rnd, int color, string blink, ConsoleColor colorDecoration, int height, string star, string starDecoration, int treeIntent, int stemIntent, int colorCount, string space)
    {
        if (blink == "yes")
        {
            color = 7;
            int delay = 500;
            while (true)
            {

                //Weihnachtsbaum wird gezeichnet
                DrawChristmastree(rnd, colorCount, height, color, colorDecoration, star, starDecoration, treeIntent, stemIntent, space, visible);
                System.Threading.Thread.Sleep(delay);
                Console.Clear();
                visible = !visible;
            }
        }
        else
        {
            DrawChristmastree(rnd, colorCount, height, color, colorDecoration, star, starDecoration, treeIntent, stemIntent, space, visible);
        }
    }

    private static int GetColorCount()
    {
        int colorCount = 0;
        Console.WriteLine("how many colors should your christmas tree have?: ");
        while (!int.TryParse(Console.ReadLine(), out colorCount))
        {
            Console.WriteLine("that isn't a number, enter a new number:");
        }

        return colorCount;
    }

    private static void DrawChristmastree(Random rnd, int colorCount, int height, int color, ConsoleColor colorDecoration, string star, string starDecoration, int treeIntent, int stemIntent, string space, bool visible)
    {
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
                    if (7 == color)
                    {
                        Console.ForegroundColor = (ConsoleColor)rnd.Next(9, 16);
                    }
                    else
                    {
                        if (colorCount > 1)
                        {

                        }
                        else
                        {
                            Console.ForegroundColor = colorDecoration;
                        }
                    }
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

    private static string GetDecoration(string answer, string star2, string starDecoration)
    {
        if (answer == "yes") { }
        else
        {
            starDecoration = star2;
        }

        return starDecoration;
    }

    private static string GetBlink()
    {
        string blink;
        Console.WriteLine("should your tree blink (you can then no longer select a color for the decoration)(yes/no):");
        blink = Console.ReadLine();

        return blink;
    }

    private static int GetColor()
    {
        int color;
        Console.WriteLine("what color should your Christmas tree be (1 = blue, 2 = green, 3 = red, 4 = yellow, 5 = pink, 6 = white, 7 = random):");
        while (!int.TryParse(Console.ReadLine(), out color))
        {
            Console.WriteLine("that isn't a number, enter a new number:");
        }

        return color;
    }

    private static int GetHeight()
    {
        return GetChrismasDecoration();
    }

    private static string GetChrismastreeDecoration()
    {
        Console.WriteLine("do you want decoration on your christmas tree (yes/no):");
        string answer = Console.ReadLine();
        return answer;
    }

    private static int GetChrismasDecoration()
    {
        int height;
        Console.WriteLine("enter the desired height of your christmas tree:");
        while (!int.TryParse(Console.ReadLine(), out height))
        {
            Console.WriteLine("that isn't a number, enter a new number:");
        }

        return height;
    }

    private static ConsoleColor GetForegroundColor(int color)
    {
        ConsoleColor colorDecoration = ConsoleColor.White;

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

        return colorDecoration;
    }

    private static int GetIntColorCount(int i)
    {
        int intcolorCount;
        Console.WriteLine("what color should your " + i +". Christmas tree be (1 = blue, 2 = green, 3 = red, 4 = yellow, 5 = pink, 6 = white):");
        while (!int.TryParse(Console.ReadLine(), out intcolorCount))
        {
            Console.WriteLine("that isn't a number, enter a new number:");
        }
        return intcolorCount;
    }
}






