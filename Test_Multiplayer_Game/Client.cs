using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test_Multiplayer_Game
{
    class Client : Online
    {
        private TcpClient client;
        private NetworkStream stream;

        public Client()
        {
            //new Thread(Start).Start();
        }

        protected override void Start()
        {
            try
            {
                client = new TcpClient();
                client.Connect(IPAddress.Parse("127.0.0.1"), port);
                Console.WriteLine("Connesso al server.");

                stream = client.GetStream();

                // Avvia thread separati per l'invio e la ricezione
                Thread invioThread = new Thread(SendMessage);
                Thread ricezioneThread = new Thread(ReceiveMessage);

                invioThread.Start();
                ricezioneThread.Start();

                // Assicurati che il client attenda la chiusura dei thread
                invioThread.Join();
                ricezioneThread.Join();
            }
            catch (Exception e)
            {
                Console.WriteLine("Errore nel client: " + e.Message);
            }

        }

        protected void ReceiveMessage()
        {
            byte[] buffer = new byte[1024];
            int byteCount;

            try
            {
                while ((byteCount = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string messaggio = Encoding.UTF8.GetString(buffer, 0, byteCount);
                    Console.WriteLine("\nMessaggio ricevuto dal server: " + messaggio);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Errore nella ricezione dei messaggi: " + e.Message);
            }
            finally
            {
                stream.Close();
                client.Close();
            }
        }

        public void SendMessage()
        {
            try
            {
                string messaggio = Program.player.PosX.ToString() + ';' + Program.player.PosY.ToString() + ';' + Program.player.Skin.ToString()
                       + ';' + Program.player.AnimationX.ToString() + ';' + (Program.player.AnimationY / 4 / 32).ToString();
                byte[] data = Encoding.UTF8.GetBytes(messaggio);
                stream.Write(data, 0, data.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine("Errore nell'invio dei messaggi: " + e.Message);
            }
        }
    }
}
