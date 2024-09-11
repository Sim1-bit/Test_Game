using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test_Multiplayer_Game
{
    class Client : Player
    {
        TcpClient client;
        public Client(TcpClient client)
        {
            this.client = client;
        }

        protected void Initialize(int pX, int pY, int skin, int aX, int aY)
        {
            base.Initialize(skin);
            posMap = new Vector2i(pX, pY);
            animation = new Vector2i(aX, aY);
            sprite.Position = new Vector2f(PosX * Program.proportion + Program.proportion / 2, PosY * Program.proportion + Program.proportion / 2);
            Skin = skin;

            sprite.TextureRect = new IntRect(AnimationX * 16, (animation.Y + Skin * 4) * 32, 16, 32);
        }

        public void ManageClient()
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            try
            {
                // Avvia thread separati per ricezione e invio
                Thread ricezioneThread = new Thread(() => ReceiveMessage());
                ricezioneThread.Start();

                // Invia messaggi al client (può essere modificato per inviare dinamicamente)
                SendMessage();
            }
            catch (Exception e)
            {
                Console.WriteLine("Errore nel gestore del client: " + e.Message);
            }
            finally
            {
                /*stream.Close();
                client.Close();*/
            }
        }

        public void ReceiveMessage()
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int byteCount;

            try
            {
                while ((byteCount = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string messaggio = Encoding.UTF8.GetString(buffer, 0, byteCount);
                    Console.WriteLine("\nMessaggio ricevuto dal client: " + messaggio);
                    string[] message1 = messaggio.Split(';');

                    int pX = Convert.ToInt32(message1[0]);
                    int pY = Convert.ToInt32(message1[1]);
                    int skin = Convert.ToInt32(message1[2]);
                    int aX = Convert.ToInt32(message1[3]);
                    int aY = Convert.ToInt32(message1[4]);

                    if (!Program.players.Contains(this))
                    {
                        Initialize(pX, pY, skin, aX, aY);
                        Program.players.Add(this);
                    }
                    else
                    {
                        posMap = new Vector2i(pX, pY);
                        animation = new Vector2i(aX, aY);
                        sprite.Position = new Vector2f(PosX * Program.proportion + Program.proportion / 2, PosY * Program.proportion + Program.proportion / 2);
                        Skin = skin;

                        sprite.TextureRect = new IntRect(AnimationX * 16, (AnimationY + Skin * 4) * 32, 16, 32);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Errore nella ricezione dei messaggi dal client: " + e.Message);
            }
            finally
            {
                Program.players.Remove(this);
                stream.Close();
                client.Close();
            }
        }

        public void SendMessage()
        {
            string messaggio = Program.player.PosX.ToString() + ';' + Program.player.PosY.ToString() + ';' + Program.player.Skin.ToString()
                        + ';' + Program.player.AnimationX.ToString() + ';' + Program.player.AnimationY.ToString();
            Console.WriteLine("Dico: {0}", messaggio);
            byte[] data = Encoding.UTF8.GetBytes(messaggio);
            client.GetStream().Write(data, 0, data.Length);
        }

    }
}
