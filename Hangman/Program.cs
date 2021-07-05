using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Hangman
{
    class Program
    {
        public static List<String> ReadFile()
        {
            string directory = new DirectoryInfo(Environment.CurrentDirectory).FullName;
            string PATH = Path.Combine(directory, @"countries_and_capitals.txt");
            var Cities_with_countires= new List<String>();
            StreamReader file = File.OpenText(PATH);
            string line;
            while ((line = file.ReadLine()) != null)
            {
                Cities_with_countires.Add(line);
            }
            return Cities_with_countires;
        }

        public static void Write_all(List<String> Cities)
        {
            foreach (var s in Cities)
            {
                Console.WriteLine(s);
            }
        }

        public static int DrawString(List<String> Cities_with_Countries)
        {
            Random r = new Random();
            return r.Next(0, Cities_with_Countries.Count);
        }

        public static string[] GetCityNameAndCountryName()
        {
            var Cities_with_Countries = ReadFile();
            int index = DrawString(Cities_with_Countries);
            string City_with_Country=Cities_with_Countries[index];
            var City = new string[2];
            City[0]="";
            bool Country_passed = false;
            foreach (var character in City_with_Country)
            {
                if (Country_passed)
                {
                    City[0] += character;
                }
                else
                {
                    if (character == '|') Country_passed = true;
                    else City[1] += character;
                }
            }
            City[0]=City[0].Trim();
            City[1]=City[1].Trim();
            City[0]=City[0].ToUpper();
            City[1]=City[1].ToUpper();
            return City;
        }

        public static char GuessLetter(List<char> GuessedLetters)
        {
            char ch=' ';
            bool result=false;
            while (!result)
            {
                Console.WriteLine("Choose letter");
                result = Char.TryParse(Console.ReadLine(), out ch);
                if (!result)
                {
                    Console.WriteLine("Pick available letter!");
                }
                else
                {
                    ch=char.ToUpper(ch);
                    foreach (var character in GuessedLetters)
                    {
                        if (ch == character)
                        {
                            result = false;
                            Console.WriteLine("You have already picked this letter.");
                            Thread.Sleep(2000);
                            break;
                        }
                    }

                    if (result)
                    {
                        Console.Clear();
                        GuessedLetters.Add(ch);
                    }
                }
            }

            return ch;
        }

        public static string WriteCity(List<char> GuessedLetters, string[] City)
        {
            string ActualCity="";
            foreach (var ch in City[0])
            {
                if (ch == ' ') ActualCity += " ";
                else
                {
                    bool Guessed = false;
                    foreach (var va in GuessedLetters)
                    {
                        if (va == ch)
                        {
                            Guessed = true;
                            ActualCity += ch;
                        }
                    }

                    if (!Guessed)
                    {
                        ActualCity += "_";
                    }
                }
            }
            PrintCenter(ActualCity);
            return ActualCity;
        }

        public static void NewGame()
        {
            PrintCenter("Welcome to the hangman");
            PrintCenter("Press any key to draw your City to guess.");
            Console.ReadKey();
        }

        public static void Lose(int Letters,string City)
        {
            Console.Clear();
            Console.WriteLine("You lost");
            PrintStatsLost(Letters,City);
            StickmanDrawing(0);
            PrintHighScores();
        }

        public static void AddScore(int time, int tries, string word)
        {
            Console.WriteLine("Give your name");
            var name=Console.ReadLine();
            string fileName = @"scorelog.txt";
            using (FileStream fs = new FileStream(fileName,FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine("{0} | {1} | {2} | {3} |",name,time,tries,word);
            }
        }

        public class HighScoreEntry
        {
            public string PlayerName;
            public string Time;
            public string Letters;
            public string City;
            public HighScoreEntry(string playerName, string time, string letters, string city)
            {
                PlayerName = playerName;
                Time = time;
                Letters = letters;
                City = city;
            }
        }

        public static void PrintHighScores()
        {
            List<HighScoreEntry> AllScores = new List<HighScoreEntry>();
            using (System.IO.StreamReader sr = new StreamReader("scorelog.txt"))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string[] lineParts = line.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    AllScores.Add(new HighScoreEntry(lineParts[0], lineParts[1], lineParts[2], lineParts[3]));
                }
                List<HighScoreEntry> SortedList = AllScores.OrderBy(i => i.Time).Take(10).ToList<HighScoreEntry>();
                string HighScoreText = "Player | Score | Letters | City \n";
                foreach (HighScoreEntry item in SortedList)
                {
                    HighScoreText += item.PlayerName + "|" + item.Time + "|" + item.Letters + "|" + item.City + "\n";
                }
                Console.WriteLine(HighScoreText);
            }
        }
        public static void Win(int lives, int letters, int ElapsedTime, string City)
        {
            Console.Clear();
            PrintCenter("You won");
            PrintStatsWon(lives,letters, ElapsedTime);
            PrintCenter("Do you want to add your score to high score list?");
            string answer=Console.ReadLine();
            if (answer == "yes")
            {
                AddScore(ElapsedTime, letters, City);
            }
            PrintHighScores();
        }

        public static void PrintStatsWon(int lives, int letters, int ElapsedTime)
        {
            Console.WriteLine("Remaining lives {0}",lives);
            Console.WriteLine("Guessed letters: {0}", letters);
            Console.WriteLine("Time: {0} seconds", ElapsedTime);
        }
        public static void PrintStatsLost(int letters, string City)
        {
            Console.WriteLine("Guessed letters: {0}", letters);
            Console.WriteLine("The City was: {0}",City);
        }
        public static void PrintCenter(string s)
        {
            Console.SetCursorPosition((Console.WindowWidth - s.Length) / 2, Console.CursorTop);
            Console.WriteLine(s);
        }

        public static bool LetterOrCity()
        {
            Console.WriteLine("Choose if you want to type letter or city");
            string choose = Console.ReadLine();
            choose = choose.ToLower();
            choose = choose.Trim();
            switch (choose)
            {
                    case "letter":
                    {
                        return true;
                    }
                    case "city":
                    {
                        return false;
                    }
            }

            return LetterOrCity();
        }
        public static string GuessCity(string City)
        {
            string attempt;
            Console.WriteLine("Guess city");
            attempt = Console.ReadLine();
            Console.Clear();
            return attempt;
        }

        public static bool CheckCity(string attempt,string City)
        {
            attempt = attempt.ToUpper();
            attempt = attempt.Trim();
            if (attempt == City)
            {
                return true;
            }

            return false;
        }
        public static void Game()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            int lives = 5;
            NewGame();
            var City_with_country=GetCityNameAndCountryName();
            var Guessed_Letters = new List<char>();
            while (lives > 0)
            {
                Console.WriteLine("You have {0} lives", lives);
                if (lives == 2 || lives == 1) Console.WriteLine("Its capital of {0}", City_with_country[1]);
                if (IsWinner(WriteCity(Guessed_Letters, City_with_country)) == true)
                {
                    stopWatch.Stop();
                    TimeSpan ts = stopWatch.Elapsed;
                    int ElapsedTime = ts.Seconds;
                    Win(lives, Guessed_Letters.Count, ElapsedTime, City_with_country[0]);
                    break;
                }
                StickmanDrawing(lives);
                UsedLetters(Guessed_Letters);
                bool chosen_letter = LetterOrCity();
                if (chosen_letter)
                {
                    if (CheckLetter(GuessLetter(Guessed_Letters), City_with_country) == false)
                    {
                        lives--;
                    }
                }
                else
                {
                    if (CheckCity(GuessCity(City_with_country[0]), City_with_country[0]) == false)
                    {
                        Console.WriteLine("You lost 2 lives");
                        Thread.Sleep(2000);
                        Console.Clear();
                        lives -= 2;
                    }
                    else
                    {
                        stopWatch.Stop();
                        TimeSpan ts = stopWatch.Elapsed;
                        int ElapsedTime = ts.Seconds;
                        Win(lives, Guessed_Letters.Count, ElapsedTime, City_with_country[0]);
                        break;
                    }
                }
            }

            if (lives <= 0)
            {
                stopWatch.Stop();
                Lose(Guessed_Letters.Count,City_with_country[0]);
            }
        }

        public static bool IsWinner(string City)
        {
            if (City.Contains('_'))
            {
                return false;
            }

            return true;
        }

        public static bool CheckLetter(char ch, string[] City)
        {
            foreach (var VARIABLE in City[0])
            {
                if (VARIABLE == ch)
                {
                    return true;
                }
            }
            Console.WriteLine("You lost live");
            Thread.Sleep(2000);
            Console.Clear();
            return false;
        }

        public static void UsedLetters(List<char> Letters)
        {
            Console.WriteLine("Used Letters:");
            foreach (var ch in Letters)
            {
                Console.Write(ch+" ");
            }
            Console.WriteLine();
        }

        public static void StickmanDrawing(int lives)
        {
            Console.WriteLine();
            Console.WriteLine();
            if (lives <= 0)   Console.WriteLine("     0   ");
            if (lives <= 3)  Console.WriteLine(@"   /'I'\   ");
            else if(lives==4) Console.WriteLine("   /'I    ");
            if(lives<=1)     Console.WriteLine(@"    / \     ");
            else if(lives==2) Console.WriteLine("    /");
            Console.WriteLine();
            Console.WriteLine();
        }
        static void Main(string[] args)
        {
            Game();
            bool newgame = false;
            PrintCenter("Do you want to start new game?");
            string answer = Console.ReadLine();
            if (answer == "yes") newgame = true;
            if (answer == "no") newgame = false;
            while (newgame)
            {
                Console.Clear();
                Game();
            }
        }
    }
}
