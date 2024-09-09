using System;
using System.IO;
using System.Collections.Generic;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Test_Multiplayer_Game
{
    class Map
    {
        private Sprite sprite;

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

        public Dictionary<int, IntRect> book = new Dictionary<int, IntRect>();

        private int[,] map;

        public int this[int x, int y]
        {
            get
            {
                return map[x, y];
            }
        }

        public Map(int proportion)
        {
            FileStream stream = File.Open(@"..\..\..\map\map.mp", FileMode.Open);
            BinaryReader br = new BinaryReader(stream);

            this.proportion = proportion;
            this.map = new int[br.ReadInt32(), br.ReadInt32()];
            for(int i = 0; i < map.GetLength(0); i++)
            {
                for(int j = 0; j < map.GetLength(1); j++)
                {
                    this.map[i, j] = br.ReadInt32();
                }
            }

            stream.Close();

            stream = File.Open(@"..\..\..\map\map.bk", FileMode.Open);
            br = new BinaryReader(stream);

            for (int i = 0, j = br.ReadInt32(); i < j; i++)
            {
                book[br.ReadInt32()] = new IntRect(br.ReadInt32(), br.ReadInt32(), br.ReadInt32(), br.ReadInt32());
            }

            stream.Close();

            sprite = new Sprite();
            sprite.Texture = new Texture(@"..\..\..\FilePNG\Map\Overworld.png");
            sprite.Scale = new Vector2f(3 * proportion / 45, 3 * proportion / 45);
        }

        public bool IsWalkable(int x, int y)
        {
            try
            {
                return map[x, y] % 2 == 0 ? true : false;
            }
            catch (IndexOutOfRangeException Error)
            {
                Console.WriteLine(Error.Message);
                return false;
            }
        }

        public void Draw(RenderWindow window)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    sprite.Position = new Vector2f(i * proportion, j * proportion);
                    sprite.TextureRect = book[map[i, j]];
                    window.Draw(sprite);
                }
            }
        }
    }
}
