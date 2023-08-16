class Program
{
    static void Main(string[] args)
    {
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
        int[,] sudokuCompare = {
                {5,3,4, 6,7,8, 9,1,2 },
                {6,7,2, 1,9,5, 3,4,8 },
                {1,9,8, 3,4,2, 5,6,7 },

                {8,5,9, 7,6,1, 4,2,3 },
                {4,2,6, 8,5,3, 7,9,1 },
                {7,1,3, 9,2,4, 8,5,6 },

                {9,6,1, 5,3,7, 2,8,4 },
                {2,8,7, 4,1,9, 6,3,5 },
                {3,4,5, 2,8,6, 1,7,9 }
            };
        if (SudokuLös(sudokuTest))
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