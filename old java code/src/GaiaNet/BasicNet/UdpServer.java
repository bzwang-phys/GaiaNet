package GaiaNet.BasicNet;

//TODO:
// 1. Potential safety loophole: should check the data that would be written in.

import GaiaNet.NetTransfer.MultiThreadServer;

import java.io.BufferedWriter;
import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.nio.charset.StandardCharsets;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.logging.FileHandler;
import java.util.logging.Logger;
import java.util.logging.SimpleFormatter;

public class UdpServer {
    private static final Logger logServer = Logger.getLogger( UdpServer.class.getName() );
    public FileHandler fileHandler;  // For log file.
    private int port;

    public UdpServer(int port){
        this.port = port;

        try {
            this.fileHandler = new FileHandler("./log/UdpReceived.log");   // For log file.
            this.fileHandler.setFormatter(new SimpleFormatter());
            this.logServer.addHandler(this.fileHandler);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    private void writeFile(String s) throws IOException {
        logServer.info(s);
    }


//    receive the data and write in log.
    public void run(){
        try {
            SimpleDateFormat ft = new SimpleDateFormat("yyyy-MM-dd hh:mm:ss");
            byte[] buf = new byte[1024];
            DatagramSocket ds = new DatagramSocket(this.port);
            DatagramPacket dpReceive = new DatagramPacket(buf, buf.length);
//            System.out.println("server is on.");
            while (true){
                ds.receive(dpReceive);
                String clientInfo = dpReceive.getAddress().toString() +":" + dpReceive.getPort();
                Date dNow = new Date();
                String timeNow = "[ " + ft.format(dNow) + " ] ";
                String dataReceive = new String(dpReceive.getData(), 0, dpReceive.getLength(), StandardCharsets.UTF_8);
                writeFile("From: " + clientInfo + "  Data: " + dataReceive + System.lineSeparator());
            }
        } catch (Exception e){
            e.printStackTrace();
        }

    }
}
