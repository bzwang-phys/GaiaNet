package GaiaNet.BasicNet;

import java.io.IOException;
import java.net.*;
import java.nio.charset.StandardCharsets;
import java.util.Scanner;

public class UdpClient {
    private static final int TIMEOUT = 5000;
    private static final int MAXNUM = 5;
    private int port;
    private String ip;

    public UdpClient(String ip, int port){
        this.ip = ip;
        this.port = port;
    }

    public UdpClient(String ip){
        this.ip = ip;
        this.port = 9091;
    }

    public void senIP() throws IOException {
        Scanner sc = new Scanner(System.in);
        String strSend = InetAddress.getLocalHost().getHostAddress();
        DatagramSocket ds = new DatagramSocket();
        InetAddress serverIp = InetAddress.getByName(this.ip);
        DatagramPacket dpSent = new DatagramPacket(strSend.getBytes(StandardCharsets.UTF_8),strSend.length(),serverIp,this.port);
//        byte[]  buf = new byte[1024];
//        DatagramPacket dpReceive = new DatagramPacket(buf, 1024);
        ds.send(dpSent);
//        }
    }

    public void sendMessa() throws IOException {
        Scanner sc = new Scanner(System.in);
        String strSend = sc.nextLine();
        DatagramSocket ds = new DatagramSocket();
        InetAddress serverIp = InetAddress.getByName(this.ip);
        DatagramPacket dpSent = new DatagramPacket(strSend.getBytes(StandardCharsets.UTF_8),strSend.length(),serverIp,this.port);
        ds.send(dpSent);
    }


}
