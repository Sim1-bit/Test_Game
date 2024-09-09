using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test_Multiplayer_Game
{
    abstract class Online
    {
        protected const int port = 8888;

        public Online()
        {
            new Thread(Start).Start();
        }

        protected abstract void Start();
    }
}
