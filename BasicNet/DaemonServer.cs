using GaiaNet.Command;
using GaiaNet.GaiaNets;
using System;
using GaiaNet.Relay;
using System.Threading;

namespace GaiaNet.BasicNet
{
    class DaemonServer
    {
        private int _portServer;
        private int _portRelay;
        private MultiThreadServer server = null;
        private UdpServer udpServer = null;
        private Net net = null;
        private LocalRelayListener relayListener = null;

        /*
        * Prepare a server listening on portServer,
        *
        */
        public DaemonServer(String type)
        {
            if ("server".Equals(type))
            {
                this.net = new Net();
                this._portServer = Config.serverPort;
                this._portRelay = this._portServer + 1;
                this.server = new MultiThreadServer(_portServer, this.net);
                this.udpServer = new UdpServer(_portServer);
                this.relayListener = new LocalRelayListener(_portRelay);
                new Thread(this.udpServer.run).Start();
                new Thread(this.relayListener.serverRun).Start();
                this.server.serverRun();
            }
            else if ("shell".Equals(type))
            {
                this.net = new Net();
                CommandHandler commandHandler = new CommandHandler(_portServer);
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
