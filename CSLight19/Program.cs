using System;
using System.Text;
using System.Threading;
using System.IO;

namespace CSLight19
{
    class Program
    {
        static void Main(string[] args)
        {
            int playerX = 1, playerY = 1;
            int coinX, coinY;
            int bombX, bombY;
            bool isGameNotEnd = true;
            bool playerNotDraw = true;
            bool isBombPlanted = false;
            int coinsOnMap = 0;
            int coinsCollected = 0;
            int score = 0;
            int bombsLeft = 5;
            char coinSymbol = '$';
            char playerSymbol = '@';
            char bombSymbol = '*';
            char wallSymbol = '#';
            ConsoleColor playerColor = ConsoleColor.Green;
            ConsoleColor coinColor = ConsoleColor.Yellow;
            ConsoleColor bombColor = ConsoleColor.Red;

            char[,] map = ReadMap("map1");
            DrawMap(map);
            Console.CursorVisible = false;

            while (coinsOnMap < 10)
            {
                coinX = GetCoordinate(map.GetLength(0));
                coinY = GetCoordinate(map.GetLength(1));
                if (map[coinX, coinY] == ' ')
                {
                    map[coinX, coinY] = coinSymbol;
                    DrawElementOnMap(coinX, coinY, coinSymbol, coinColor);
                    coinsOnMap++;
                }
            }

            while (playerNotDraw)
            {
                playerX = GetCoordinate(map.GetLength(0));
                playerY = GetCoordinate(map.GetLength(1));
                if (map[playerX, playerY] == ' ' && map[playerX, playerY] != coinSymbol)
                {
                    DrawElementOnMap(playerX, playerY, playerSymbol, playerColor);
                    playerNotDraw = false;
                }
            }

            while (isGameNotEnd)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyPressed = Console.ReadKey(true);
                    switch (keyPressed.Key)
                    {
                        case ConsoleKey.UpArrow:
                            if (map[playerX - 1, playerY] != wallSymbol)
                            {
                                RemoveElement(playerX, playerY);
                                DrawElementOnMap(--playerX, playerY, playerSymbol, playerColor);
                            }
                            break;
                        case ConsoleKey.DownArrow:
                            if (map[playerX + 1, playerY] != wallSymbol)
                            {
                                RemoveElement(playerX, playerY);
                                DrawElementOnMap(++playerX, playerY, playerSymbol, playerColor);
                            }
                            break;
                        case ConsoleKey.LeftArrow:
                            if (map[playerX, playerY - 1] != wallSymbol)
                            {
                                RemoveElement(playerX, playerY);
                                DrawElementOnMap(playerX, --playerY, playerSymbol, playerColor);
                            }
                            break;
                        case ConsoleKey.RightArrow:
                            if (map[playerX, playerY + 1] != wallSymbol)
                            {
                                RemoveElement(playerX, playerY);
                                DrawElementOnMap(playerX, ++playerY, playerSymbol, playerColor);
                            }
                            break;
                        case ConsoleKey.Spacebar:
                            if (!isBombPlanted && bombsLeft > 0)
                            {
                                map[playerX, playerY] = bombSymbol;
                                isBombPlanted = true;
                                bombsLeft--;
                            }
                            break;
                        case ConsoleKey.Enter:
                            if (isBombPlanted)
                            {
                                GetCoordinate(map, bombSymbol, out bombX, out bombY);
                                for (int i = bombX - 1; i < bombX + 2; i++)
                                {
                                    for (int j = bombY - 1; j < bombY + 2; j++)
                                    {
                                        if (i < map.GetLength(0) - 1 && j < map.GetLength(1) - 1)
                                        {
                                            score += 50;
                                            RemoveElement(ref map, i, j);
                                            RemoveElement(i, j);
                                            if (playerX == i && playerY == j)
                                            {
                                                isGameNotEnd = false;
                                                GameResult(2);
                                            }
                                        }
                                    }
                                }
                                isBombPlanted = false;
                            }
                            break;
                        case ConsoleKey.Escape:
                            isGameNotEnd = false;
                            break;
                    }
                }

                if (isBombPlanted)
                {
                    GetCoordinate(map, bombSymbol, out bombX, out bombY);
                    DrawElementOnMap(bombX, bombY, bombSymbol, bombColor);
                }

                if (map[playerX, playerY] == coinSymbol)
                {
                    RemoveElement(ref map, playerX, playerY);
                    coinsCollected++;
                    score += 100;
                }

                if (coinsCollected == coinsOnMap)
                {
                    isGameNotEnd = false;
                    GameResult(1);
                }

                WriteScore(map, score, coinsCollected, bombsLeft);
            }
            Console.ReadKey();
        }

        static char[,] ReadMap(string mapName)
        {
            string[] newFile = File.ReadAllLines($"Maps/{mapName}.map");
            char[,] map = new char[newFile.Length, newFile[1].Length];

            for (int i = 0; i < map.GetLength(0); i++)
                for (int j = 0; j < map.GetLength(1); j++)
                    map[i, j] = newFile[i][j];

            return map;
        }
        static void DrawMap(char[,] map)
        {
            Console.Clear();
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                    Console.Write(map[i, j]);
                Console.WriteLine();
            }
        }

        static int GetCoordinate(int maxCoordinate)
        {
            Random coordinates = new Random();
            return coordinates.Next(1, maxCoordinate);
        }

        static void GetCoordinate(char[,] map, char symbol, out int coordinateX, out int coordinateY)
        {
            coordinateX = 1;
            coordinateY = 1;
            for (int i = 0; i < map.GetLength(0); i++)
                for (int j = 0; j < map.GetLength(1); j++)
                    if (map[i, j] == symbol)
                    {
                        coordinateX = i;
                        coordinateY = j;
                    }
        }

        static void DrawElementOnMap(int elementX, int elementY, char symbol, ConsoleColor elementColor = ConsoleColor.Gray)
        {
            ConsoleColor tempColor = Console.ForegroundColor;
            Console.ForegroundColor = elementColor;
            Console.SetCursorPosition(elementY, elementX);
            Console.Write(symbol);
            Console.ForegroundColor = tempColor;
            Console.SetCursorPosition(50, 0);
        }

        static void RemoveElement(ref char[,] map, int elementX, int elementY)
        {
            map[elementX, elementY] = ' ';
        }

        static void RemoveElement(int elementX, int elementY)
        {
            Console.SetCursorPosition(elementY, elementX);
            Console.Write(' ');
        }

        static void WriteScore(char[,] map, int scoreValue, int coinsCollected, int bombsLeft)
        {
            ConsoleColor tempColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.SetCursorPosition(0, map.GetLength(0) + 1);
            Console.Write($"Score: {scoreValue}\nCoins: {coinsCollected}\nBombs: {bombsLeft}");
            Console.ForegroundColor = tempColor;
            Console.WriteLine("\n\nДля перемещения используйте стрелки." +
                "\nСтены можно разрушить бомбой:" +
                "\n[Пробел] - установить бомбу" +
                "\n[Enter] - Взорвать бомбу" +
                "\nЗона поражения бомбы 3х3 клетки, игрок попавший под взрыв погибает" +
                "\n[Esc] - Выход из игры");
        }

        static void GameResult(int result)
        {
            Console.Clear();
            Console.SetCursorPosition(Console.WindowWidth / 2 - 3, Console.WindowHeight / 2);
            Console.ForegroundColor = ConsoleColor.Red;
            if (result == 1)
            {
                Console.WriteLine("!!!YOU WON!!!");
            }
            else
            {
                Console.WriteLine(":(((YOU LOSE:(((");
            }
        }
    }
}
