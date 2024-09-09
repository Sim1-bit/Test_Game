using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.IO;

namespace Test_Multiplayer_Game
{
    class Program
    {
        static Online online;

        static RenderWindow window;
        const int proportion = 48;

        public static Map map;
        public static Player player;

        public static List<Player> players;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            window = new RenderWindow(new VideoMode(1080, 640), "Oh shit");
            window.SetVerticalSyncEnabled(true);
            window.Closed += (sender, args) => window.Close();

            map = new Map(proportion);
            player = new Player(proportion, window, map, 1);

            players = new List<Player>();

            players.Add(player);

            window.KeyPressed += KeyPressed;

            while (window.IsOpen)
            {
                window.DispatchEvents();
                window.Clear();

                map.Draw(window);

                foreach (var player in players)
                {
                    player.DrawnPlayer();
                }

                window.Display();
            }
        }

        public static void KeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.W || e.Code == Keyboard.Key.A || e.Code == Keyboard.Key.S || e.Code == Keyboard.Key.D)
            {
                player.KeyPressed(e);

                View view = new View(new FloatRect(0, 0, window.Size.X, window.Size.Y));
                view.Center = player.sprite.Position;
                view.Zoom(0.6f);
                window.SetView(view);
            }
            else if (e.Code == Keyboard.Key.H)
                online = new Server();
            else if(e.Code == Keyboard.Key.Enter)
                online = new Client();
        }
    }
}
