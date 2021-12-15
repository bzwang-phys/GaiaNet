using GaiaNet.Command;
using GaiaNet.GaiaNets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GaiaNet.BasicNet
{
    class DaemonServer
    {
        private int _portServer = 9091;
        private MultiThreadServer server = null;
        private UdpServer udpServer = null;
        private Net net = null;

        /*
        * Prepare a server listening on portServer,
        *
        */
        public DaemonServer(String type)
        {
            if ("server".Equals(type))
            {
                this.net = new Net();
                this.server = new MultiThreadServer(_portServer, this.net);
                this.udpServer = new UdpServer(_portServer);
                new Thread(udpServer.run).Start();
                server.serverRun();
            }
            else if ("shell".Equals(type))
            {
                this.net = new Net();
                CommandHandler commandHandler = new CommandHandler();
                commandHandler.getCmdFromKey();
            }
            else if ("test".Equals(type))
            {
                this.udpServer = new UdpServer(_portServer);
                this.server = new MultiThreadServer(_portServer);
                this.net = new Net();
                new Thread(server.serverRun).Start();
                new Thread(udpServer.run).Start();
                CommandHandler commandHandler = new CommandHandler();
                commandHandler.getCmdFromKey();
            }
        }


    }
}
