using System.Collections.Generic;

namespace Sudokusolver
{
    class Program
    {
        static void Main(string[] args)
        {
            int value = 1;
            Dictionary<int, int> tmp = new Dictionary<int, int>();
            bool exitOuterLoop = false;
            int[,] sudokuTest = {
                {5,3,0, 0,7,0, 0,0,0 },
                {6,0,0, 1,9,5, 0,0,0 },
                {0,9,8, 0,0,0, 0,6,0 },

                {8,0,0, 0,6,0, 0,0,3 },
                {4,0,0, 8,0,3, 0,0,1 },
                {7,0,0, 0,2,0, 0,0,6 },

                {0,6,0, 0,0,0, 2,8,0 },
                {0,0,0, 4,1,9, 0,0,5 },
                {0,0,0, 0,8,0, 0,7,9 }
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
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (!exitOuterLoop)
                        value = 1 + sudokuTest[i,j];
                    exitOuterLoop = false;
                    if (sudokuTest[i, j] != 0)
                    {
                        continue;
                    }
                    // Check if value is already there in 3x3?
                    ThreeByThree(ref value, ref exitOuterLoop, sudokuTest, i, ref j);
                    if (exitOuterLoop)
                    {
                        if (value == 9)
                            j--;
                        continue;
                    }
                    //Check if Value is alread there in up down and left to 
                    Xplus9(ref value, ref exitOuterLoop, sudokuTest, i, ref j);
                    if (exitOuterLoop)
                    {
                        if (value == 9)
                            j--;
                        continue;
                    }
                    Yplus9(ref value, ref exitOuterLoop, sudokuTest, i, ref j);
                    if (exitOuterLoop)
                    {
                        if (value == 9)
                            j--;
                        continue;
                    }
                    sudokuTest[i, j] = value;
                }
            }
            CorrectTest(sudokuTest, sudokuCompare);
        }

        private static void Yplus9(ref int value, ref bool exitOuterLoop, int[,] sudokuTest, int i, ref int j)
        {
            for (int l = 0; l < 9; l++)
            {
                if (sudokuTest[l, i] == value)
                {
                    exitOuterLoop = true;
                    value++;
                    j--;
                    break;
                }
            }
        }

        private static void Xplus9(ref int value, ref bool exitOuterLoop, int[,] sudokuTest, int i, ref int j)
        {
            for (int l = 0; l < 9; l++)
            {
                if (sudokuTest[i, l] == value)
                {
                    exitOuterLoop = true;
                    value++;
                    j--;
                    break;
                }
            }
        }

        private static void ThreeByThree(ref int value, ref bool exitOuterLoop, int[,] sudokuTest, int i, ref int j)
        {
            for (int k = i - (i % 3); k < k + 3; k++)
            {
                for (int l = j - (j % 3); l < l + 3; l++)
                {
                    if (sudokuTest[k, l] == value)
                    {
                        exitOuterLoop = true;
                        break;
                    }
                }
                if (exitOuterLoop)
                {
                    j--;
                    value++;
                    break;
                }
            }
        }

        private static void CorrectTest(int[,] sudokuTest, int[,] sudokuCompare)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (sudokuTest[i, j] != sudokuCompare[i, j])
                    {
                        Console.WriteLine("Code funktioniert noch nicht :(");
                        Thread.Sleep(10000);
                        Environment.Exit(0);
                    }
                }
            }
            Console.WriteLine("Code funktioniert :) !!!");
            Thread.Sleep(10000);
        }
    }
}