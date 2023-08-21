using System.Diagnostics;

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
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        if (SudokuLös(array))
        {
            Console.WriteLine("SUDOKU GELÖÖÖST");
            SudokuZeichnen(array);
        }
        else
        {
            Console.WriteLine("Sudoku Unlösbar");
        }

        stopwatch.Stop();

        TimeSpan ts = stopwatch.Elapsed;
        Console.WriteLine($"Verstrichene Zeit: {ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}");

    }

    private static bool SudokuLös(int[,] sudokuTest)
    {
        List<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        for (int y = 0; y < 9; y++)
        {

            for (int x = 0; x < 9; x++)
            {
                if (sudokuTest[y, x] != 0)
                {
                    continue;
                }
                numbers = PossibleNumbers(sudokuTest, x, y, numbers);
                foreach (int number in numbers)
                {
                    sudokuTest[y, x] = number;
                    Console.Clear();
                    SudokuZeichnen(sudokuTest);
                    Thread.Sleep(5);
                    if (SudokuLös(sudokuTest))
                    {
                        return true;
                    }
                    sudokuTest[y, x] = 0;
                }
                return false;
            }
        }
        return true;
    }

    private static List<int> PossibleNumbers(int[,] sudokuTest, int x, int y, List<int> numbers)
    {
        for (int i = 0; i < 9; i++)
        {
            if (numbers.Contains(sudokuTest[y, i]))
            {
                numbers.Remove(sudokuTest[y, i]);
            }
            else if (numbers.Contains(sudokuTest[i, x]))
            {
                numbers.Remove(sudokuTest[i, x]);
            }
            else if (numbers.Contains(sudokuTest[y - y % 3 + i / 3, x - x % 3 + i % 3]))
            {
                numbers.Remove(sudokuTest[y - y % 3 + i / 3, x - x % 3 + i % 3]);
            }
        }
        return numbers;
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
}