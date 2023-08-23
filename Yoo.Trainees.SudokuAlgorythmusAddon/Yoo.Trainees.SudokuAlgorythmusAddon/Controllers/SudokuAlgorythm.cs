using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Yoo.Trainees.SudokuAlgorythmusAddon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SudokuController : ControllerBase
    {
        [HttpPost("solve")]
        public IActionResult Solve(string[][] board)
        {
            int[,] array = new int[9, 9];

            for (int i = 0; i < 9; i++)
            {
                /*string inputLine = board[i].Trim(' ', '{', '}', ',');
                string[] elements = inputLine.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                */
                for (int j = 0; j < 9; j++)
                {
                    array[i, j] = int.Parse(board[i][j]);
                }
            }
            if (SudokuLös(array))
            {
                int[][] result = new int[9][];
                for (int j = 0; j < 9; j++)
                {
                    result[j] = new int[9];
                    for (int k = 0; k < 9; k++)
                    {
                        result[j][k] = array[j, k];
                    }
                }
                return Ok(result);
            }
            else
            {
                return BadRequest(new int[,] { { -1} });
            }
        }

        private bool SudokuLös(int[,] sudokuTest)
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

                    numbers = PossibleNumbers(sudokuTest, x, y);
                    foreach (int number in numbers)
                    {
                        sudokuTest[y, x] = number;
                        if (SudokuLös(sudokuTest))
                        {
                            return true;
                        }
                        sudokuTest[y, x] = 0;
                    }
                    return false;
                }
            }
            return SudokuFertig(sudokuTest);
        }

        private bool SudokuFertig(int[,] sudokuTest)
        {
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    if (sudokuTest[y, x] == 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private List<int> PossibleNumbers(int[,] sudokuTest, int x, int y)
        {
            List<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            for (int i = 0; i < 9; i++)
            {
                if (numbers.Contains(sudokuTest[y, i]))
                {
                    numbers.Remove(sudokuTest[y, i]);
                }
                if (numbers.Contains(sudokuTest[i, x]))
                {
                    numbers.Remove(sudokuTest[i, x]);
                }
                if (numbers.Contains(sudokuTest[y - y % 3 + i / 3, x - x % 3 + i % 3]))
                {
                    numbers.Remove(sudokuTest[y - y % 3 + i / 3, x - x % 3 + i % 3]);
                }
            }
            return numbers;
        }
    }
}
