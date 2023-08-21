class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Bitte geben Sie das 2D-Array im folgenden Format ein:");
        Console.WriteLine("{1,3,0, 0,0,5, 0,4,0 },");
        Console.WriteLine("...");
        Console.WriteLine("{6,0,0, 0,0,0, 0,0,5 }");

        int[,] array = new int[9, 9];

        for (int i = 0; i < 9; i++)
        {
            string inputLine = Console.ReadLine().Trim(' ', '{', '}', ',');
            string[] elements = inputLine.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            for (int j = 0; j < elements.Length; j++)
            {
                array[i, j] = int.Parse(elements[j]);
            }
        }

        int[,] sudokuTest = {
                {1,3,0, 0,0,5, 0,4,0 },
                {5,0,0, 7,3,0, 0,0,0 },
                {7,0,0, 0,1,9, 0,0,0 },

                {0,0,6, 0,0,0, 1,0,9 },
                {0,0,9, 8,0,0, 0,0,2 },
                {8,7,0, 0,0,0, 0,0,0 },

                {0,0,0, 0,0,8, 0,0,0 },
                {0,0,0, 3,4,0, 0,6,0 },
                {6,0,0, 0,0,0, 0,0,5 }
            };
        if (SudokuLös(array))
        {
            Console.WriteLine("SUDOKU GELÖÖÖST");
        }else
        {
            Console.WriteLine("Sudoku Unlösbar");
        }
            
    }

    private static bool SudokuLös(int[,] sudokuTest)
    {
        for(int y = 0; y < 9; y++)
        {
            for(int x = 0; x < 9; x++)
            {
                if (sudokuTest[y, x] != 0)
                {
                    continue;
                }
                for (int wert = 1; wert <= 9; wert++)
                {
                    if (FeldFrei(sudokuTest, y, x, wert))
                    {
                        sudokuTest[y, x] = wert;
                        Console.Clear();
                        SudokuZeichnen(sudokuTest);
                        Thread.Sleep(5);
                        if (SudokuLös(sudokuTest))
                        {
                            return true;
                        }
                        sudokuTest[y, x] = 0;
                    }
                }
                return false;
            }
        }
        return true;
    }

    private static void SudokuZeichnen(int[,] sudokuTest)
    {
        for (int y = 0; y < 9; y++)
        {
            if (y != 0 && y % 3 == 0)
            { 
                Console.WriteLine(new string('-', 19));
            }

            for (int x = 0; x < 9; x++)
            {
                if (x != 0 && x % 3 == 0)
                    Console.Write("|");

                Console.Write(sudokuTest[y, x]);

                if (x < 8) 
                    Console.Write(" ");
            }
            Console.WriteLine();
        }
    }

    private static bool FeldFrei(int[,] sudokuTest, int y, int x, int wert)
    {
        for(int i =  0; i < 9; i++)
        {
            if (sudokuTest[y, i] == wert || sudokuTest[i, x] == wert || sudokuTest[y - y % 3 + i  / 3, x - x % 3 + i % 3] == wert)
            {
                return false;
            }
        }
        return true;
    }
}