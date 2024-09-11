using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Test_Multiplayer_Game
{
    class Guest : Online
    {
        private TcpClient client;
        private NetworkStream stream;

        public Guest()
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

                Client client1 = new Client(client);

                // Avvia thread separati per l'invio e la ricezione
                Thread invioThread = new Thread(client1.SendMessage);
                Thread ricezioneThread = new Thread(client1.ReceiveMessage);

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
    }
}
