package GaiaNet.NetTransfer;

//import GaiaNet.NetTransfer.*;

import GaiaNet.GaiaNet.Net;
import GaiaNet.BasicNet.UdpServer;

public class DaemonServer {
    private int portServer = 9190;
    private MultiThreadServer server = null;
    private UdpServer udpServer = null;
    private Net net = null;

    /*
     * Prepare a server listening on portServer,
     *
     */
    public DaemonServer(String type){
        if ("server".equals(type)){
            this.net = new Net();
            this.server = new MultiThreadServer(portServer, this.net);
            this.udpServer = new UdpServer(portServer);
            new Thread(() -> this.udpServer.run()).start();
            this.server.serverRun();
        } else if ("shell".equals(type)){
            this.net = new Net();
            CommandHandler commandHandler = new CommandHandler();
            commandHandler.getCommand();
        } else if ("test".equals(type)){
            this.udpServer = new UdpServer(portServer);
            this.server = new MultiThreadServer(portServer);
            this.net = new Net();
            new Thread(() -> this.server.serverRun()).start();
            new Thread(() -> this.udpServer.run()).start();
            CommandHandler commandHandler = new CommandHandler();
            commandHandler.getCommand();
        }
    }




}

