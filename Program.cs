using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Snek {
    class Program {
        //Keeps track of vital stats
        private static int score = 0;
        private static int length = 6;

        private struct Position {
            public int left;
            public int top;
        }
        //list to keep track of the snakes position
        private static List<Position> snek = new List<Position>();
        //explains where the snake is
        private static Position currentPosition = new Position();
        //explains what direction the snake should go in
        private static int _x;
        private static int _y;

        //track elements on screen
        private static List<Objects> Terraform = new List<Objects>();
        private struct Objects {
            public int left;
            public int top;
        }
        private static Objects Terrain = new Objects();
        //amount of terrain the player wants
        private static int TerrainCount = 3;

        //track food
        private static Objects Food = new Objects();

        //timer which the game plays in
        private static Timer snekMove;
        //randomizer
        private static Random rng = new Random();
        //dependensie for gameOver
        private static bool inPlay = true;

        //PlayingField Size
        private static int PlayingFieldWidthMin = 30;
        private static int PlayingFieldWidthMax = Console.WindowWidth -50;

        private static int PlayingFieldHeightMin = 5;
        private static int PlayingFieldHeightMax = Console.WindowHeight -5;
        //Skin
        class Skin {
            public bool Rainbow;
            public bool JohnCena;
            public int RainbowSkinCount = 0;
            public void RainbowDraw() {
                switch (RainbowSkinCount % 7) {
                    case 0: Draw(ConsoleColor.Red, currentPosition.left, currentPosition.top);  break;
                    case 1: Draw(ConsoleColor.DarkYellow, currentPosition.left, currentPosition.top); break;
                    case 2: Draw(ConsoleColor.Yellow, currentPosition.left, currentPosition.top); break;
                    case 3: Draw(ConsoleColor.Green, currentPosition.left, currentPosition.top); break;
                    case 4: Draw(ConsoleColor.Blue, currentPosition.left, currentPosition.top); break;
                    case 5: Draw(ConsoleColor.Magenta, currentPosition.left, currentPosition.top); break;
                    case 6: Draw(ConsoleColor.DarkMagenta, currentPosition.left, currentPosition.top); break;
                }
                RainbowSkinCount++;
            }
            public void JohnCenaDraw() {
                Draw(ConsoleColor.Black, currentPosition.left, currentPosition.top);
            }
            public void Norm() {
                Draw(ConsoleColor.DarkGreen, currentPosition.left, currentPosition.top);
            }
            public Skin() {
                Rainbow = false;
            }
        }
        private static Skin SnekSkin = new Skin();
        static void Main(string[] args) {
            Settup();
            // **************************************** // Gameplay
            start();
            //Generate map with a list
            DrawTerrain();
            //initiate game
            snekMove = new Timer(DrawPlayer, null, 0, 100);

            //init snake if terrain is greater than 0, write the head and then make it movable with only the head, when enter is pressed again start the game

            while (KeyInput(Console.ReadKey(true).Key) && inPlay) {
            }
            // **************************************** // Gameplay End
        }
        static int Beep = 0;
        private static bool KeyInput(ConsoleKey Key) {
            switch (Key) {
                //move inputs
                case ConsoleKey.LeftArrow: if (!snek.Any(Position => Position.top == currentPosition.top && Position.left == currentPosition.left -1)) { _x= -1; _y = 0; }; break;
                case ConsoleKey.RightArrow: if (!snek.Any(Position => Position.top == currentPosition.top && Position.left == currentPosition.left +1)) { _x= 1; _y = 0; }; break;
                case ConsoleKey.UpArrow: if (!snek.Any(Position => Position.top == currentPosition.top -1 && Position.left == currentPosition.left)) { _y= -1; _x = 0; }; break;
                case ConsoleKey.DownArrow: if (!snek.Any(Position => Position.top == currentPosition.top +1 && Position.left == currentPosition.left)) { _y= 1; _x = 0; }; break;

                //easteregg for Ahmed
                case ConsoleKey.A:
                Console.Beep() ;
                Console.SetCursorPosition(PlayingFieldWidthMax - PlayingFieldWidthMax / 3, PlayingFieldHeightMin - 2);
                Beep++;
                Console.Write("Amount of beeping: {0}", Beep);
                break;

                //ESC function
                case ConsoleKey.Escape: GameOver(); return false;
            }
            return true;

        }
        private static void DrawTerrain() {

            //Randomly generate blocks to a list of the game map
            while (true) {
                //validates if terrain is enabled
                if(TerrainCount > 0) {
                    //generates position
                    Terrain.top = rng.Next(PlayingFieldHeightMin + 1, PlayingFieldHeightMax);
                    Terrain.left = rng.Next(PlayingFieldWidthMin + 1, PlayingFieldWidthMax);
                    //Checks if position is valid
                    if (!Terraform.Any(Position => Position.left == Terrain.left && Position.top == Terrain.top)) {
                        //adds element to list
                        Terraform.Add(Terrain);
                        //draws the element
                        Draw(ConsoleColor.DarkCyan, Terrain.left, Terrain.top);
                    }
                    //checks if enough terrain is made
                    if (Terraform.Count() == TerrainCount) {
                        break;
                    }
                }else {
                break;
                }
            }
            //draw the essentials
            DrawFood();
            DrawScore();
        }
        private static void DrawFood() {
            while(true) {
                //mark where the food is located 
                Food.top = rng.Next(PlayingFieldHeightMin, PlayingFieldHeightMax);
                Food.left = rng.Next(PlayingFieldWidthMin, PlayingFieldWidthMax);

                //check if it is allowed to spawn there
                if (!snek.Any(position => position.left == Food.left && position.top == Food.top) ) {
                    if(!Terraform.Any(Position => Position.left == Terrain.left && Position.top == Terrain.top) || TerrainCount >0 ) {

                        //draws the Food
                        Draw(ConsoleColor.Red, Food.left, Food.top);

                        break;
                    }
                }
            }
        }
        private static void DrawPlayer(Object o) {
            //at start, move down
            if (currentPosition.top == snek.First().top && currentPosition.left == snek.First().left) {
                _y = 1;
            }

            //Terrain check
            if (snek.Count == 2 && TerrainCount > 0) {
                //pauses the timer
                snekMove.Dispose();
                //Writes the text
                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.SetCursorPosition(1, 1);
                Console.Write("Enter a direction to move in!");

                //Accepts the input of move direction
                KeyInput(Console.ReadKey(true).Key);

                //Removes the text
                for (int i = 1; i < 31; i++) {
                    Console.SetCursorPosition(i, 1);
                    Console.Write(" ");
                }

                //Resets stuff
                Console.ResetColor();
                snekMove = new Timer(DrawPlayer, null, 0, 100);
            }

            //acceptinput commad to keep the value until changed to another direction
            currentPosition.left += _x;
            currentPosition.top += _y;

            //check if direction is valid also keep track of if snek is alive
            CollisionDetection();

            //add element to list (after collision is checked to avoid instant death)
            snek.Add(currentPosition);

            //Draws the snek head // temp solution
            if (SnekSkin.Rainbow) {
                SnekSkin.RainbowDraw();
            } else if (SnekSkin.JohnCena) {
                SnekSkin.JohnCenaDraw();
            } else {
                SnekSkin.Norm();
            }

            if (snek.Count() > length) {
                Console.SetCursorPosition(snek.First().left, snek.First().top);
                Console.Write(" ");
                snek.Remove(snek.First());
            }
        }
        private static void Draw(ConsoleColor colour, int left, int top) {

            Console.BackgroundColor = colour;
            //Writes out element on screen
            Console.SetCursorPosition(left, top);
            Console.Write(" ");
            //Resets colour to white
            Console.BackgroundColor = ConsoleColor.Black;
        }
        private static void Draw(int left, int top, string mark) {
            Console.SetCursorPosition(left, top);
            Console.Write(mark);
        }
        private static void DrawScore() {
            //Sets position of score and draws it
            Draw(PlayingFieldWidthMax - PlayingFieldWidthMax / 3, PlayingFieldHeightMin - 1, "Score: " + score.ToString());
        }
        private static void CollisionDetection() {
            //off screen top & bottom
            if (currentPosition.top < PlayingFieldHeightMin) { currentPosition.top = PlayingFieldHeightMax; };
            if (currentPosition.top > PlayingFieldHeightMax) { currentPosition.top = PlayingFieldHeightMin; };
            //off screen right & left
            if (currentPosition.left < PlayingFieldWidthMin) { currentPosition.left = PlayingFieldWidthMax; };
            if (currentPosition.left > PlayingFieldWidthMax) { currentPosition.left = PlayingFieldWidthMin; };

            //self collision
            if (snek.Any(position => position.left == currentPosition.left && position.top == currentPosition.top)) {
                GameOver();
            }
            //collision with terrain
            if (Terraform.Any(Position => Position.left == currentPosition.left && Position.top == currentPosition.top)) {
                GameOver();
            }

            //food collision
            if (snek.Last().top == Food.top && snek.Last().left == Food.left) {
                score++;
                length++;
                DrawScore();
                DrawFood();
            }
        }
        private static void Settup() {
            Rules();
            while (true) {
                var Terrain = Console.ReadLine();
                if (int.TryParse(Terrain, out TerrainCount) && TerrainCount >= 0 && TerrainCount < ((PlayingFieldWidthMax - PlayingFieldWidthMin) * (PlayingFieldHeightMax - PlayingFieldHeightMin)) / 20   ) {
                    break;
                } else {
                    Console.Clear();
                    //class with all skins, constructor turns all to false but the one entered
                    new Skin();
                    switch (Terrain.ToString()) {
                        case "Rainbow": SnekSkin.Rainbow = true; break;
                        case "JohnCena": SnekSkin.JohnCena = true; break;
                        default: break;
                    }
                    Rules();
                }
            }
            Console.CursorVisible = false;
            Console.WriteLine("Press Enter to start");
            do {
            } while (Console.ReadKey(false).Key != ConsoleKey.Enter);
            Console.Clear();
        }
        private static void Rules() {
            Console.Write("Gamerules:\n" +
            "1) You will start in the center of the playfield\n" +
            "2) § represents a terrain block and will kill the player\n" +
            "3) O represents a food block and will increase your score and length\n" +
            "4) Press ESC to exit the game at any time\n" +
            "5) Amount of terrain: ");
        }
        private static void start() {
            //Sets startposition
            currentPosition.left = PlayingFieldWidthMax /2;
            currentPosition.top = PlayingFieldHeightMax /2;
            snek.Add(currentPosition);

            //draws the playing field
            for(int i = 0; i < Console.WindowWidth; i++) {
                for(int y = 0; y < Console.WindowHeight; y++) {
                    if(i < PlayingFieldWidthMin || i > PlayingFieldWidthMax || y > PlayingFieldHeightMax || y < PlayingFieldHeightMin) {
                        Console.SetCursorPosition(i, y);
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.Write(" ");
                    }
                }
            }
            //resets colours when field is drawn
            Console.ResetColor();

        }
        private static void GameOver() {
            inPlay = false;
            snekMove.Dispose();
            Console.Clear();
            Console.WriteLine("Game Over!\n\nYour Score: {0}\nPlaying area(x*y): {1} * {2}\nAmount of terrain: {3}", score, PlayingFieldWidthMax - PlayingFieldWidthMin, PlayingFieldHeightMax - PlayingFieldHeightMin, TerrainCount);
            Console.ReadLine();
        }
        
    }
}
