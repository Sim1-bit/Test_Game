using System;
using System.IO;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Collections.Generic;
using System.Numerics;

namespace Test_Multiplayer_Game
{
    class Player
    {
        private int skin;

        public int Skin
        {
            get => skin;
            set
            {
                skin = value < 0 || value > 5 ? 0 : value;
            }
        }
        public int AnimationX
        {
            get => animation.X;
        }

        public int AnimationY
        {
            get => animation.Y;
        }

        private Vector2i animation;

        private int proportion;

        public int Proportion
        {
            get
            {
                return proportion;
            }
            set
            {
                if (value <= 0)
                {
                    proportion = 1;
                }
                else
                {
                    proportion = value;
                }
            }
        }

        private RenderWindow window;

        public readonly Sprite sprite;


        private Map map;

        private Vector2i posMap;

        public int PosX
        {
            get
            {
                return posMap.X;
            }
        }
        public int PosY
        {
            get
            {
                return posMap.Y;
            }
        }

        public Player(int proportion, RenderWindow window, Map map, int skin)
        {
            Proportion = proportion;

            this.window = window;

            Skin = skin;

            posMap = new Vector2i(5, 5);
            this.sprite = new Sprite(new Texture(@"..\..\..\FilePNG\Sprite\Sprite.png"), new IntRect(0, 0, 255, 255));
            
            sprite.Scale = new Vector2f(3 * proportion / 45, 3 * proportion / 45);
            sprite.Position = new Vector2f(PosX * proportion + proportion / 2, PosY * proportion + proportion / 2);
            sprite.TextureRect = new IntRect(0, Skin * 32 * 4, 16, 32);
            sprite.Origin = new Vector2f(sprite.GetLocalBounds().Left + sprite.GetLocalBounds().Width / 2, sprite.GetLocalBounds().Top + sprite.GetLocalBounds().Height / 2);

            animation = new Vector2i(0, Skin * 4 * 32);

            View view = new View(new FloatRect(0, 0, window.Size.X, window.Size.Y));
            view.Center = sprite.Position;
            view.Zoom(0.6f);
            window.SetView(view);

            this.map = map;
        }

        public Player(Player aux) : this(aux.proportion, aux.window, aux.map, aux.Skin)
        {

        }

        public void KeyPressed(KeyEventArgs e)
        {
            int frame = 4;

            switch(e.Code)
            {
                case Keyboard.Key.W:
                    animation.Y = 2;
                    if (map.IsWalkable(PosX, PosY - 1))
                    {
                        posMap.Y--;
                        for (int i = 0; i < frame; i++)
                        {
                            sprite.Position = new Vector2f(sprite.Position.X, sprite.Position.Y - Proportion / frame);
                        }
                    }
                    break;
                case Keyboard.Key.S:
                    animation.Y = 0;
                    if (map.IsWalkable(PosX, PosY + 1))
                    {
                        posMap.Y++;
                        sprite.Position = new Vector2f(sprite.Position.X, sprite.Position.Y + Proportion);
                    }
                    break;
                case Keyboard.Key.A:
                    animation.Y = 3;
                    if (map.IsWalkable(PosX - 1, PosY))
                    {
                        posMap.X--;
                        sprite.Position = new Vector2f(sprite.Position.X - Proportion, sprite.Position.Y);
                    }
                    break;
                case Keyboard.Key.D:
                    animation.Y = 1;
                    if (map.IsWalkable(PosX + 1, PosY))
                    {
                        posMap.X++;
                        sprite.Position = new Vector2f(sprite.Position.X + Proportion, sprite.Position.Y);
                    }
                    break;
            }
            animation.X = (animation.X + 1) % 4;
            sprite.TextureRect = new IntRect(animation.X * 16, (animation.Y + Skin * 4) * 32, 16, 32);
            if (Program.online is Client)
                (Program.online as Client).SendMessage();
            /*else
                (Program.online as Server).SendMessage();*/

        }

        public void DrawnPlayer()
        {
            window.Draw(sprite);
        }
    }
}
