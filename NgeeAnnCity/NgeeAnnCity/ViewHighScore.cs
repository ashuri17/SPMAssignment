using System;
using System.Collections.Generic;

namespace NgeeAnnCity
{
    public class HighScores
    {
        public static void Start()
        {
            ViewArcade();
            ViewFreePlay();
        }
        public static void ViewArcade()
        {
            string[] highScoreFile = File.ReadAllLines("arcadehighscores.csv");
            int nameColumnWidth = 9;
            int scoreColumnWidth = 11;
            int totalWidth = nameColumnWidth + scoreColumnWidth + 3; // +3 for the spaces and borders

            // Top border

            Console.WriteLine(new string('-', totalWidth + 1));

            // Title inside the box
            Console.WriteLine("|{0," + (totalWidth - 2) + "} |", "Game Mode: Arcade");
            Console.WriteLine("|{0," + (totalWidth - 2) + "} |", "Top 10 Highest Scores");
            Console.WriteLine(new string('-', totalWidth + 1));


            // Header
            Console.WriteLine("|{0,-" + nameColumnWidth + "} {1," + scoreColumnWidth + "} |", "Name", "High Score");

            // Scores
            for (int i = 0; i < highScoreFile.Length; i++)
            {
                string[] highScore = highScoreFile[i].Split(',');
                Console.WriteLine("|{0,-" + nameColumnWidth + "} {1," + scoreColumnWidth + "} |", highScore[0], highScore[1]);
            }

            // Bottom border
            Console.WriteLine(new string('-', totalWidth));
        }
        public static void ViewFreePlay()
        {
            string[] highScoreFile = File.ReadAllLines("freeplayhighscores.csv");
            int nameColumnWidth = 9;
            int scoreColumnWidth = 11;
            int totalWidth = nameColumnWidth + scoreColumnWidth + 3; // +3 for the spaces and borders

            // Top border
            Console.WriteLine(new string('-', totalWidth + 1));

            // Title inside the box
            Console.WriteLine("|{0," + (totalWidth - 2) + "} |", "Game Mode: Free Play");
            Console.WriteLine("|{0," + (totalWidth - 2) + "} |", "Top 10 Highest Scores");
            Console.WriteLine(new string('-', totalWidth + 1));


            // Header
            Console.WriteLine("|{0,-" + nameColumnWidth + "} {1," + scoreColumnWidth + "} |", "Name", "High Score");

            // Scores
            for (int i = 0; i < highScoreFile.Length; i++)
            {
                string[] highScore = highScoreFile[i].Split(',');
                Console.WriteLine("|{0,-" + nameColumnWidth + "} {1," + scoreColumnWidth + "} |", highScore[0], highScore[1]);
            }
            // Bottom border
            Console.WriteLine(new string('-', totalWidth + 1));
        }
    }
}

