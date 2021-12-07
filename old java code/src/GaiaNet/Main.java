package GaiaNet;

import java.io.IOException;

import GaiaNet.NetTransfer.*;

public class Main {

    public static void main(String[] args) throws IOException, InterruptedException {
        String type = "server";
        if (args.length != 0)
            type = args[0];
        DaemonServer nt = new DaemonServer(type);
//        UdpClient udpClient = new UdpClient("114.214.202.245", 9392);
//        udpServer.run();

    }
}
