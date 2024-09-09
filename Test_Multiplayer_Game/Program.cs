using SFML.Graphics;
using SFML.Window;
using System;
using System.IO;

namespace Test_Multiplayer_Game
{
    class Program
    {
        static RenderWindow window;
        const int proportion = 48;

        public static Map map;
        public static Player player;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            window = new RenderWindow(new VideoMode(1080, 640), "Oh shit");
            window.SetVerticalSyncEnabled(true);
            window.Closed += (sender, args) => window.Close();

            map = new Map(proportion);
            player = new Player(proportion, window, map);

            window.KeyPressed += KeyPressed;

            while (window.IsOpen)
            {
                window.DispatchEvents();
                window.Clear();

                map.Draw(window);
                player.DrawnPlayer();

                window.Display();
            }
        }

        public static void KeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.W || e.Code == Keyboard.Key.A || e.Code == Keyboard.Key.S || e.Code == Keyboard.Key.D || e.Code == Keyboard.Key.Enter)
            {
                player.KeyPressed(e);

                View view = new View(new FloatRect(0, 0, window.Size.X, window.Size.Y));
                view.Center = player.sprite.Position;
                view.Zoom(0.6f);
                window.SetView(view);
            }
        }
    }
}
