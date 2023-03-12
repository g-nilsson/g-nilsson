using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sudoku
{
    class Program
    {
        static bool[,,] possibleSpaces;
        static int[,] sudokuGrid;
        static void Main(string[] args)
        {
            sudokuGrid = new int[9, 9] {
                {  0,  0,  0,   0,  0,  0,   0,  0,  0 },
                {  0,  0,  0,   0,  0,  3,   0,  8,  5 },
                {  0,  0,  1,   0,  2,  0,   0,  0,  0 },

                {  0,  0,  0,   5,  0,  7,   0,  0,  0 },
                {  0,  0,  4,   0,  0,  0,   1,  0,  0 },
                {  0,  9,  0,   0,  0,  0,   0,  0,  0 },

                {  5,  0,  0,   0,  0,  0,   0,  7,  3 },
                {  0,  0,  2,   0,  1,  0,   0,  0,  0 },
                {  0,  0,  0,   0,  4,  0,   0,  0,  9 } };
            
            InputSudoku();
            SolveSudoku(sudokuGrid, false);
            DateTime startTime = DateTime.Now;
            SolveSudoku(sudokuGrid, true);
            TimeSpan solvingTime = DateTime.Now - startTime;

            //*: output
            Console.WriteLine($"Solving Complete. \nSolved in {solvingTime} seconds.");
            PrintOutput();
            Console.ReadKey();
        }

        static void InputSudoku()
        {
            Console.ForegroundColor = ConsoleColor.White;
            start:
            Console.WriteLine("do you want to input another sudoku? (y/n)");
            string input = Console.ReadLine();
            if (input.ToUpper() != "Y" && input.ToUpper() != "N")
            {
                Console.WriteLine("That is an incorrect input, please answer with a 'y' or an 'n'");
                Console.ReadKey();
                Console.Clear();
                goto start;
            }
            else if (input.ToUpper() == "Y")
            {
                for (int x = 0; x < 9; x++)
                    for (int y = 0; y < 9; y++)
                    {
                        insertNumber:
                        Console.WriteLine($"number at coordinate: x:{x} y:{y}");
                        input = Console.ReadLine();
                        if (Int32.TryParse(input, out int result))
                            sudokuGrid[x, y] = result;
                        else
                        {
                            Console.WriteLine("That is an incorrect input, please write a number (0-9)");
                            Console.ReadKey();
                            Console.Clear();
                            goto insertNumber;
                        }
                        Console.Clear();
                    }
            }
            Console.Clear();
        }

        static int[,] SolveSudoku(int[,] sudokuGrid, bool fastSolve)
        {
            possibleSpaces = new bool[9, 9, 9];
            bool solved = false;

            // set everything to possible
            for (int x = 0; x < 9; x++)
                for (int y = 0; y < 9; y++)
                    for (int z = 0; z < 9; z++)
                        possibleSpaces[x, y, z] = true;

            while (!solved)
            {                                
                if (!fastSolve)
                {
                    PrintOutputAdv();
                    Console.ReadKey();
                    Console.Clear();
                }

                UpdatePossibleSpaces();

                InsertNewNumbers();

                //*: Check if done
                solved = true;
                foreach (bool val in possibleSpaces)
                    if (val)
                        solved = false;
            }
            return sudokuGrid;
        }

        static void PrintOutputAdv()
        {
            Console.ForegroundColor = ConsoleColor.White;
            for (int i = 0; i < 9; i++)
            {
                Console.WriteLine("\n\n" + (i + 1));
                for (int x = 0; x < 9; x++)
                {
                    if (x % 3 == 0)
                        Console.WriteLine();
                    Console.WriteLine();
                    for (int y = 0; y < 9; y++)
                    {
                        if (y % 3 == 0)
                            Console.Write(" ");
                        Console.Write(Convert.ToInt32(possibleSpaces[i, x, y]) + " ");
                    }
                    Console.Write("   ");
                    for (int y = 0; y < 9; y++)
                    {
                        if (y % 3 == 0)
                            Console.Write(" ");
                        if (sudokuGrid[x, y] != -1)
                        {
                            if (sudokuGrid[x, y] == i + 1)
                                Console.ForegroundColor = ConsoleColor.Red;

                            Console.Write(sudokuGrid[x, y] + " ");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else
                            Console.Write(0 + " ");
                    }
                }
            }
        }

        static void PrintOutput()
        {
            Console.ForegroundColor = ConsoleColor.White;
            for (int x = 0; x < 9; x++)
            {
                if (x % 3 == 0)
                    Console.WriteLine();
                Console.WriteLine();
                Console.Write("   ");
                for (int y = 0; y < 9; y++)
                {
                    if (y % 3 == 0)
                        Console.Write(" ");
                    if (sudokuGrid[x, y] != -1)
                    {

                        Console.Write(sudokuGrid[x, y] + " ");
                    }
                    else
                        Console.Write(0 + " ");
                }
            }
        }

        static void UpdatePossibleSpaces()
        {
            for (int i = 0; i < 9; i++)
            {
                // Foreach space
                for (int x = 0; x < 9; x++)
                    for (int y = 0; y < 9; y++)
                    {
                        //*: If space already taken
                        if (sudokuGrid[x, y] != 0)
                            possibleSpaces[i, x, y] = false;

                        //*: Check column contains i
                        for (int a = 0; a < 9; a++)
                            if (sudokuGrid[x, a] == i + 1)
                                possibleSpaces[i, x, y] = false;

                        //*: Check row contains i
                        for (int a = 0; a < 9; a++)
                            if (sudokuGrid[a, y] == i + 1)
                                possibleSpaces[i, x, y] = false;

                        //*: Check square contains i
                        // get center of square (x % 3 == 0 ? 1 : ((x - 2) % 3 == 0 ? -1 : 0)) + x
                        for (int a = -1; a < 2; a++)
                            for (int b = -1; b < 2; b++)
                            {
                                int square_center_x = (x % 3 == 0 ? 1 : ((x - 2) % 3 == 0 ? -1 : 0)) + a;
                                int square_center_y = (y % 3 == 0 ? 1 : ((y - 2) % 3 == 0 ? -1 : 0)) + b;
                                if (sudokuGrid[x + square_center_x, y + square_center_y] == i + 1)
                                    possibleSpaces[i, x, y] = false;
                            }
                    }

                // foreach square
                for (int a = 0; a < 3; a++)
                    for (int b = 0; b < 3; b++)
                    {
                        // Check horizontal
                        int horizontalCertainVal = -1;
                        bool horizontalCertain = true;
                        for (int x = -1; x < 2; x++)
                        {
                            for (int y = -1; y < 2; y++)
                            {
                                if (possibleSpaces[i, 3 * a + 1 + x, 3 * b + 1 + y])
                                {
                                    if (horizontalCertainVal == -1)
                                        horizontalCertainVal = 3 * a + 1 + x;
                                    else if (horizontalCertainVal != 3 * a + 1 + x)
                                        horizontalCertain = false;
                                }
                                if (!horizontalCertain)
                                    break;
                            }
                            if (!horizontalCertain)
                                break;
                        }

                        if (horizontalCertain && horizontalCertainVal != -1)
                            for (int y = 0; y < 9; y++)
                                if (y != 3 * b && y != 3 * b + 1 && y != 3 * b + 2)
                                    possibleSpaces[i, horizontalCertainVal, y] = false;


                        // Check vertical
                        int verticalCertainVal = -1;
                        bool verticalCertain = true;
                        for (int x = -1; x < 2; x++)
                        {
                            for (int y = -1; y < 2; y++)
                            {
                                if (possibleSpaces[i, 3 * a + 1 + x, 3 * b + 1 + y])
                                {
                                    if (verticalCertainVal == -1)
                                        verticalCertainVal = 3 * b + 1 + y;
                                    else if (verticalCertainVal != 3 * b + 1 + y)
                                        verticalCertain = false;
                                }
                                if (!verticalCertain)
                                    break;
                            }
                            if (!verticalCertain)
                                break;
                        }

                        if (verticalCertain && verticalCertainVal != -1)
                            for (int x = 0; x < 9; x++)
                                if (x != 3 * a && x != 3 * a + 1 && x != 3 * a + 2)
                                    possibleSpaces[i, x, verticalCertainVal] = false;
                    }
            }
        }        

        static void InsertNewNumbers()
        {
            bool onlyPossible = true;

            for (int i = 0; i < 9; i++)
                for (int x = 0; x < 9; x++)
                    for (int y = 0; y < 9; y++)
                    {
                        //*: Check row
                        onlyPossible = true;
                        if (possibleSpaces[i, x, y])
                        {
                            for (int a = 0; a < 9; a++)
                                if (possibleSpaces[i, x, a] && a != y)
                                    onlyPossible = false;

                            if (onlyPossible)
                                sudokuGrid[x, y] = i + 1;
                        }

                        //*: Check column
                        onlyPossible = true;
                        if (possibleSpaces[i, x, y])
                        {
                            for (int a = 0; a < 9; a++)
                                if (possibleSpaces[i, a, y] && a != x)
                                    onlyPossible = false;

                            if (onlyPossible)
                                sudokuGrid[x, y] = i + 1;
                        }

                        //*: Check square
                        onlyPossible = true;
                        if (possibleSpaces[i, x, y])
                        {
                            for (int a = -1; a < 2; a++)
                                for (int b = -1; b < 2; b++)
                                {
                                    int square_center_x = (x % 3 == 0 ? 1 : ((x - 2) % 3 == 0 ? -1 : 0)) + a;
                                    int square_center_y = (y % 3 == 0 ? 1 : ((y - 2) % 3 == 0 ? -1 : 0)) + b;
                                    if (possibleSpaces[i, x + square_center_x, y + square_center_y] && (((x + square_center_x) != x) || ((y + square_center_y) != y)))
                                        onlyPossible = false;
                                }

                            if (onlyPossible)
                                sudokuGrid[x, y] = i + 1;
                        }

                        //*: Check if only i is possible
                        onlyPossible = true;
                        if (possibleSpaces[i, x, y])
                        {
                            for (int integer = 0; integer < 9; integer++)
                                if (possibleSpaces[integer, x, y] && integer != i)
                                    onlyPossible = false;

                            if (onlyPossible)
                                sudokuGrid[x, y] = i + 1;
                        }

                    }
        }
    }
}