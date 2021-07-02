using System;
using System.Collections.Generic;
using System.IO;
using System;
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
            Console.ReadKey();
        }

        public static void Win(int lives, int letters)
        {
            Console.Clear();
            PrintCenter("You won");
            PrintStatsWon(lives,letters);
            Console.ReadKey();
        }

        public static void PrintStatsWon(int lives, int letters)
        {
            Console.WriteLine("Remaining lives {0}",lives);
            Console.WriteLine("Guessed letters: {0}", letters);
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
        public static void Game()
        {
            int lives = 5;
            NewGame();
            var City_with_country=GetCityNameAndCountryName();
            var Guessed_Letters = new List<char>();
            while (lives != 0)
            {
                Console.WriteLine("You have {0} lives",lives);
                if(lives==2 || lives==1) Console.WriteLine("Its capital of {0}", City_with_country[1]);
                if (IsWinner(WriteCity(Guessed_Letters, City_with_country)) == true)
                {
                    Win(lives,Guessed_Letters.Count);
                    break;
                };
                if (CheckLetter(GuessLetter(Guessed_Letters), City_with_country) == false)
                {
                    lives--;
                }
            }

            if (lives == 0)
            {
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
        static void Main(string[] args)
        {
            Game();
        }
    }
}
