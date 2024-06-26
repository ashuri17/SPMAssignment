﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NgeeAnnCity
{
    internal class Board
    {
        private int size;
        private char[,] grid;
        private Dictionary<Point, char> buildingDict;
        const int expansionSize = 5;

        public Board(int size)
        {
            this.size = size;
            grid = new char[size, size];
            buildingDict = new Dictionary<Point, char>();
        }

        internal void Initialize()
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    grid[i, j] = '.';
                }
            }
        }

        internal void Display()
        {
            int horizontalPadding = size.ToString().Length + 1;

            // get grid labels
            char[][] gridLabels = GetGridLabels();

            // print top grid labels
            for (int i = 0; i < gridLabels.Length; i++)
            {
                Console.Write(new String(' ', horizontalPadding + 2));
                Console.WriteLine(string.Join(" ", gridLabels[i]));
            }

            Console.WriteLine();

            // print grid
            for (int i = 0; i < size; i++)
            {
                // grid label on the left
                Console.Write($"{i + 1}".PadLeft(horizontalPadding) + "  ");

                for (int j = 0; j < size; j++)
                {
                    Console.Write(grid[i, j] + " ");
                }

                Console.WriteLine();
            }
            Console.WriteLine("\n\n");
        }

        private char[][] GetGridLabels()
        {
            // Get each number to print
            int[] numbers = Enumerable.Range(1, size).ToArray();

            // Convert each number to a string
            string[] numberStrings = numbers.Select(i => i.ToString()).ToArray();

            // Max number of digits for each number => number of digits the largest number has => number of digits the width has
            int maxLength = size.ToString().Length;

            // Number of rows the array should have
            char[][] rows = new char[maxLength][];

            // Number of columns (digits) each row should have
            for (int i = 0; i < maxLength; i++)
            {
                rows[i] = new char[size];
            }

            // Iterate through each number
            for (int col = 0; col < size; col++)
            {
                string num = numberStrings[col];
                int numLen = num.Length;

                // Iterate through each digit
                for (int row = 0; row < maxLength; row++)
                {
                    // check if num has a digit in the row (99 doesn't have a digit in the first row if the width is 100) 
                    // e.g.
                    //
                    //        1
                    //   9    0
                    //   9    0

                    if (row < numLen)
                    {
                        // If width is 2 digits, there will be 2 rows.
                        // e.g.
                        // {...} <- stores the digit in the "tens" place
                        // {...} <- stores the digit in the "ones" place
                        //
                        // If num is 5, numLen is 1.
                        // During the first iteration of the for loop, the if statement will evaluate to (0 < 1).
                        // Since it is true, 5 will be added to the last row.
                        //
                        // If it is 25, 5 will be added to the last row.
                        // If it is 125, 5 will be added to the last row.
                        //
                        //
                        // Subsequent iterations will check the other digits and if possible, add them to the other rows.

                        rows[maxLength - row - 1][col] = num[numLen - row - 1];
                    }
                    else
                    {
                        // If there is no digits, represent as whitespace
                        rows[maxLength - row - 1][col] = ' ';
                    }
                }
            }
            return rows;
        }

        internal void PlaceBuilding(char building, bool freeplay = true)
        {
            int x, y;

            // runs until a building is placed
            while (true)
            {
                // get row from user
                while (true)
                {
                    Console.Write($"Row (1-{size}): ");

                    // check if user enters a number that falls within the width of the board
                    if (!int.TryParse(Console.ReadLine(), out x) || x < 1 || x > size)
                    {
                        Console.WriteLine("Invalid row.\n");
                        continue;
                    }
                    break;
                }

                // get column from user 
                while (true)
                {
                    Console.Write($"Column (1-{size}): ");

                    // check if user enters a number that falls within the height of the board
                    if (!int.TryParse(Console.ReadLine(), out y) || y < 1 || y > size)
                    {
                        Console.WriteLine("Invalid column.\n");
                        continue;
                    }
                    break;
                }

                x--;
                y--;

                // check if spot is taken
                if (grid[x, y] != '.')
                {
                    Console.WriteLine("Spot taken.\n");
                }
                else
                {
                    grid[x, y] = building;
                    StoreBuilding(building, x, y);
                }

                if (freeplay)
                {
                    if (TouchingBorder(x, y))
                    {
                        ExpandGrid();
                    }
                }
                break;
            }
        }

        internal char GetBuilding(int row, int column)
        {
            Console.WriteLine($"{row}, {column}");
            return grid[row, column];
        }

        internal void StoreBuilding(char building, int row, int column)
        {
            buildingDict.Add(new Point(row, column), building);
        }

        internal bool TouchingBorder(int row, int column)
        {
            int maxIndex = size - 1;
            return (row == 0 || row == maxIndex || column == 0 || column == maxIndex);
        }

        internal void ExpandGrid()
        {
            size += expansionSize * 2;
            grid = new char[size, size];
            Initialize();
            foreach (var buildingInfo in buildingDict.ToList())
            {
                Point coords = buildingInfo.Key;
                char building = buildingInfo.Value;
                coords.X += 5;
                coords.Y += 5;
                grid[coords.X, coords.Y] = building;
                buildingDict.Remove(new Point(coords.X - 5, coords.Y - 5));
                buildingDict.Add(new Point(coords.X, coords.Y), building);
            }
        }

        internal int GetSize()
        {
            return size;
        }
    }
}
