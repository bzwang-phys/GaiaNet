package GaiaNet.GaiaNet;

import GaiaNet.Common.ByteTools;
import GaiaNet.Encrypt.Digest;

import java.io.IOException;
import java.net.*;
import java.nio.charset.StandardCharsets;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.util.ArrayList;
import java.util.Properties;
import java.util.logging.FileHandler;
import java.util.logging.Logger;
import java.util.logging.SimpleFormatter;

public class Node {
    public int uid;
    public int gid;
    public ArrayList<String> macArray;
    public ArrayList<String> serverIPList;
    public String innerIp;
    public String outerIp;
    public String ip6;
    public String nodeName;
    public String hostName;
    public String osName;
    private static final Logger logNode = Logger.getLogger( Node.class.getName() );
    public FileHandler fileHandler;  // For writing in log file.

    // Three methods to build node.
    // 1.local  2.from txt info. 3. null
    public Node(String s, Config conf){
        if ("local".equalsIgnoreCase(s)) {
            try {
                this.nodeName = conf.nodeName;
                this.getIPAndHost();
                this.getMacArray();
                this.osName = getOsName();
                this.uid = getUid();
                this.gid = getGid();

                fileHandler = new FileHandler("./log/GaiaNet.log");   // For log file.
                fileHandler.setFormatter(new SimpleFormatter());
                logNode.addHandler(fileHandler);
                logNode.info("Initialize Node: \n" + this.getInfo());
            } catch (IOException e) {
                e.printStackTrace();
            }
        } else {
            buildFromTxt(s);
        }
    }

    public Node(String s){
        if ("local".equalsIgnoreCase(s)) {
            try {
                this.getIPAndHost();
                this.getMacArray();
                this.osName = getOsName();
                this.uid = getUid();
                this.gid = getGid();

                fileHandler = new FileHandler("./log/GaiaNet.log");   // For log file.
                fileHandler.setFormatter(new SimpleFormatter());
                logNode.addHandler(fileHandler);
                logNode.info("Initialize Node: \n" + this.getInfo());
            } catch (IOException e) {
                e.printStackTrace();
            }
        } else {
            buildFromTxt(s);
        }
    }

    public Node(){}

    public void Config(){ }

    private void getMacArray(){
        try {
            ArrayList<String> tmpMacList = new ArrayList<>();
            java.util.Enumeration<NetworkInterface> netInterfaceEnum = NetworkInterface.getNetworkInterfaces();
            while (netInterfaceEnum.hasMoreElements()){
                NetworkInterface netIface = netInterfaceEnum.nextElement();
                byte[] mac = netIface.getHardwareAddress();
                if (mac == null) continue;
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < mac.length; i++) {
                    sb.append(String.format("%02X%s", mac[i], (i < mac.length - 1) ? "-" : ""));
                }
                tmpMacList.add(sb.toString());
                this.macArray = tmpMacList;
            }

        } catch (Exception e) {
            e.printStackTrace();
        }
    }


    private String getOsName(){
        Properties props=System.getProperties(); //?????????????????????
        return props.getProperty("os.name");
    }

    private void getIPAndHost(){
        try {
            InetAddress inetAddress = InetAddress.getLocalHost();
            this.innerIp = inetAddress.getHostAddress();
            this.hostName = inetAddress.getHostName();
            this.ip6 = Inet6Address.getLocalHost().getHostAddress();
        } catch (UnknownHostException e) {
            e.printStackTrace();
        }
    }

    private int getUid(){
        int i, res=0;
        try {
            int bitSize = 16;
            String mac = this.macArray.get(0);
            byte[] macHash = Digest.PearsonHash(mac.getBytes(StandardCharsets.UTF_8), bitSize);
            for (i=0; i<macHash.length; i++){
                res += Byte.toUnsignedInt(macHash[i]) * Math.pow(2, 8*(macHash.length-1-i));
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
        return res;
    }


    private int getGid(){
        return 0;
    }

    public String getInfo(){
        String s = "";
        s += "NodeName: " + this.nodeName + "\n";
        s += "Uid: " + this.uid + "\n";
        s += "Gid: " + this.gid + "\n";
        s += "Mac: " + this.macArray.toString() + "\n";
        s += "HostName: " + this.hostName + "\n";
        s += "InnerIP: " + this.innerIp + "\n";
        s += "IP6Addr: " + this.ip6 + "\n";
        s += "OuterIP: " + this.outerIp + "\n";
        s += "osName: " + this.osName + "\n";
        return s;
    }

    @Override
    public String toString(){
        return getInfo();
    }

    public String toTxtForm(){
        String res = String.format("%d  %s  %s%n", this.uid, this.nodeName, this.outerIp);
        return res;
    }

    public void buildFromTxt(String txt){

    }



}
