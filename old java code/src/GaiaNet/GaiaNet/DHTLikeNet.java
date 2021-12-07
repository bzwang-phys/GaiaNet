package GaiaNet.GaiaNet;

import GaiaNet.NetTransfer.GaiaType;
import GaiaNet.NetTransfer.MultiThreadServer;

import java.io.*;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.ArrayList;
import java.util.logging.FileHandler;
import java.util.logging.Logger;
import java.util.logging.SimpleFormatter;

public class DHTLikeNet<toString> {
    private static final Logger logNet = Logger.getLogger( DHTLikeNet.class.getName() );
    public FileHandler fileHandler;  // For log file.
    public Node node = null;
    public ArrayList<String> masterNodesIp;
    public ArrayList<String> neighbours;

    public DHTLikeNet(Node node, Config config){
        this.node = node;
        this.masterNodesIp = (ArrayList<String>) config.masterNodesIp;
        this.neighbours = (ArrayList<String>) config.neighbours;

        try {
            File logDir = new File("./log/");
            if (!logDir.exists())   logDir.mkdir();
            this.fileHandler = new FileHandler("./log/Net.log");   // For log file.
            this.fileHandler.setFormatter(new SimpleFormatter());
            this.logNet.addHandler(fileHandler);
        } catch (IOException e) {
            e.printStackTrace();
        }

        logNet.info("net initialize: " + this.toString());
        joinNet();
    }

    public int joinNet(){
        for (String serverIP : this.masterNodesIp) {
            logNet.info("joinNet, send to " + serverIP);
            Socket sock = null;
            try {
                sock = new Socket(serverIP, 9190);
                DataOutputStream dos = new DataOutputStream(sock.getOutputStream());
                dos.writeInt(GaiaType.GaiaNet.ordinal());
                dos.writeInt(NetRequest.JoinNet.ordinal());

                ObjectOutputStream oos = new ObjectOutputStream(sock.getOutputStream());
                ObjectInputStream ois = new ObjectInputStream(sock.getInputStream());
                oos.writeObject(this.node);
                Node remoteNode = (Node) ois.readObject();
                logNet.info("master response: " + remoteNode.toString());

                boolean isMyneigh = isMyNeighbour(remoteNode);
                if (isMyneigh) joinMyNeighbourList(remoteNode);
            } catch (IOException | ClassNotFoundException e) {
                e.printStackTrace();
            }finally {
                try { if (sock != null) sock.close(); }
                catch (IOException e) { e.printStackTrace();}
            }
        }

        return 0; // normal
    }


    public void joinNetResponse(Socket socket, InputStream is, OutputStream os){
        try {
            ObjectOutputStream oos = new ObjectOutputStream(os);
            ObjectInputStream ois = new ObjectInputStream(is);
            Node remoteNode = (Node) ois.readObject();
            oos.writeObject(this.node);
            logNet.info("A node ask for joining net: " + remoteNode.toString());

            boolean isMyneigh = isMyNeighbour(remoteNode);
            if (isMyneigh) joinMyNeighbourList(remoteNode);
        } catch (IOException | ClassNotFoundException e) {
            e.printStackTrace();
        }finally {
            try { if (socket != null) socket.close(); }
            catch (IOException e) { e.printStackTrace();}
        }

    }


    public void Config(){ }

    public void handle(Socket socket, Logger logger){
        try {
            OutputStream os = socket.getOutputStream();
            InputStream is = socket.getInputStream();
            DataInputStream dis = new DataInputStream(is);
            int flag = dis.readInt();
            NetRequest type = NetRequest.values()[flag];

            if (type == NetRequest.JoinNet) joinNetResponse(socket, is, os);
            else if (type == NetRequest.LeaveNet);
            else if (type == NetRequest.NearestNodes);

            socket.close();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    private void joinMyNeighbourList(Node node) {
    }

    private boolean isMyNeighbour(Node node) {
        return false;
    }

    private String getMyNearestNodes() {
        return this.neighbours.toString();
    }


    public int distance(Node node1, Node node2){
        return 0;
    }

    public int leaveNet(){
        return 0;
    }

    public void nearest(Node node) {

    }

    @Override
    public String toString(){
        String s = "";
        s += "Node Name in DHTLikeNet: " + this.node.nodeName + "\n";
        s += "masterNodesIp: " + this.masterNodesIp + "\n";
        s += "neighbours: " + this.neighbours.toString() + "\n";
        return s;
    }
}
