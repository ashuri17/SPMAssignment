using System;
using System.Collections.Generic;

namespace NgeeAnnCity
{
    public class HighScores
    {
        public static void Start()
        {
            ViewArcade();
            Console.WriteLine("\n\n\n");
            
            ViewFreePlay();
            Console.WriteLine("\n\n\n");
            Console.Write("Press any key to exit... ");
            Console.ReadKey();
        }
        public static void ViewArcade()
        {
            List<String> name = [];
            List<String> score = [];

            foreach (string s in File.ReadAllLines("arcadehighscores.csv").Skip(1).ToArray())
            {
                string[] sData = s.Split(',');
                name.Add(sData[0]);
                score.Add(sData[1]);
            }
            int nameColumnWidth = name.Max(i => i.Length);
            int scoreColumnWidth = "Highscore".Length;
            int totalWidth = nameColumnWidth + scoreColumnWidth + 10 < 28 ? 28 : nameColumnWidth + scoreColumnWidth + 10; // for the spaces and borders

            // Top border
            Console.WriteLine(new string('-', totalWidth));

            // Title inside the box
            Console.WriteLine("|" + CenterString("ARCADE LEADERBOARD", totalWidth - 2) + "|");

            // Middle border
            Console.WriteLine(new string('-', totalWidth));


            // Header
            Console.WriteLine("|{0,-" + (nameColumnWidth + 4) + "} {1," + (scoreColumnWidth + 3) + "}|", " Name", "Score ");

            for (int i = 0; i < name.Count; i++)
            {
                Console.WriteLine("|{2," + 2 + "}. {0,-" + nameColumnWidth + "} {1," + (scoreColumnWidth + 2) + "} |", name[i], score[i], i + 1);
            }

            // Bottom border
            Console.WriteLine(new string('-', totalWidth));
        }
        public static void ViewFreePlay()
        {
            List<String> name = [];
            List<String> score = [];

            foreach (string s in File.ReadAllLines("freeplayhighscores.csv").Skip(1).ToArray())
            {
                string[] sData = s.Split(',');
                name.Add(sData[0]);
                score.Add(sData[1]);
            }
            int nameColumnWidth = name.Max(i => i.Length);
            int scoreColumnWidth = score.Max(i => i.Length) > "Highscore".Length ? score.Max(i => i.Length) : "Highscore".Length;
            int totalWidth = nameColumnWidth + scoreColumnWidth + 10 < 30 ? 30 : nameColumnWidth + scoreColumnWidth + 10; // for the spaces and borders

            // Top border
            Console.WriteLine(new string('-', totalWidth));

            // Title inside the box
            Console.WriteLine("|" + CenterString("FREEPLAY LEADERBOARD", totalWidth - 2) + "|");

            // Middle border
            Console.WriteLine(new string('-', totalWidth));


            // Header
            Console.WriteLine("|{0,-" + (nameColumnWidth + 4) + "} {1," + (scoreColumnWidth + 3) + "}|", " Name", "Score ");

            for (int i = 0; i < name.Count; i++)
            {
                Console.WriteLine("|{2," + 2 + "}. {0,-" + nameColumnWidth + "} {1," + (scoreColumnWidth + 2) + "} |", name[i], score[i], i + 1);
            }

            // Bottom border
            Console.WriteLine(new string('-', totalWidth));
        }

        private static string CenterString(String s, int width)
        {
            string padding = new string(' ', (int)Math.Floor((double)(width - s.Length) / 2));
            return padding + s + padding;
        }
    }
}