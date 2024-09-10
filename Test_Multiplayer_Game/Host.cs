using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Test_Multiplayer_Game
{
    class Host : Online
    {
        private TcpListener server;

        public Host()
        {
            //new Thread(Start).Start();
        }

        protected override void Start()
        {
            try
            {
                server = new TcpListener(IPAddress.Any, port);
                server.Start();
                Console.WriteLine("Server avviato. In ascolto su porta " + port);

                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connessione accettata da un client.");

                    // Avvia un nuovo thread per gestire ciascun client
                    Client client1 = new Client(client);
                    Thread clientThread = new Thread(() => client1.ManageClient());
                    clientThread.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Errore nel server: " + e.Message);
            }
        }

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

                sprite.TextureRect = new IntRect(aX * 16, aY * Skin * 32 * 4, 16, 32);
            }

            public void ManageClient()
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];

                try
                {
                    // Avvia thread separati per ricezione e invio
                    Thread ricezioneThread = new Thread(() => ReceiveMessage(stream));
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
                    stream.Close();
                    client.Close();
                }
            }

            protected void ReceiveMessage(NetworkStream stream)
            {
                byte[] buffer = new byte[1024];
                int byteCount;

                try
                {
                    while ((byteCount = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        string messaggio = Encoding.UTF8.GetString(buffer, 0, byteCount);
                        Console.WriteLine("\nMessaggio ricevuto dal client: " + messaggio);
                        string[] message1 = messaggio.Split(';');
                        Initialize(Convert.ToInt32(message1[0]), Convert.ToInt32(message1[1]), Convert.ToInt32(message1[2]), Convert.ToInt32(message1[3]), Convert.ToInt32(message1[4]));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Errore nella ricezione dei messaggi dal client: " + e.Message);

                }
            }

            public void SendMessage()
            {
                string messaggio = Program.player.PosX.ToString() + ';' + Program.player.PosY.ToString() + ';' + Program.player.Skin.ToString()
                            + ';' + Program.player.AnimationX.ToString() + ';' + (Program.player.AnimationY / 4 / 32).ToString();
                byte[] data = Encoding.UTF8.GetBytes(messaggio);
                client.GetStream().Write(data, 0, data.Length);
            }

        }

        private void ManageClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            try
            {
                // Avvia thread separati per ricezione e invio
                Thread ricezioneThread = new Thread(() => ReceiveMessage(client, stream));
                ricezioneThread.Start();

                // Invia messaggi al client (può essere modificato per inviare dinamicamente)
                while (true)
                {
                    string messaggio = Program.player.PosX.ToString() + ';' + Program.player.PosY.ToString() + ';' + Program.player.Skin.ToString()
                        + ';' + Program.player.AnimationX.ToString() + ';' + (Program.player.AnimationY / 4 / 32).ToString();
                    byte[] data = Encoding.UTF8.GetBytes(messaggio);
                    stream.Write(data, 0, data.Length);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Errore nel gestore del client: " + e.Message);
            }
            finally
            {
                stream.Close();
                client.Close();
            }
        }

        protected void ReceiveMessage(TcpClient client, NetworkStream stream)
        {
            byte[] buffer = new byte[1024];
            int byteCount;

            try
            {
                while ((byteCount = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string messaggio = Encoding.UTF8.GetString(buffer, 0, byteCount);
                    Console.WriteLine("\nMessaggio ricevuto dal client: " + messaggio);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Errore nella ricezione dei messaggi dal client: " + e.Message);

            }
        }

        public void SendMessage()
        {
            throw new NotImplementedException();
        }
    }
}
