package GaiaNet.NetTransfer;

import java.io.BufferedReader;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.*;
import java.nio.charset.StandardCharsets;
import java.util.Enumeration;
import java.util.Scanner;

public class CommandExec {

    public static String exec(Command command) throws IOException {
        String res = "";
        if ("send".equalsIgnoreCase(command.action)){
            // send file.
            String ip = command.to;
            Integer port = 9190;
            String fname = command.argList.get(0);
            SendFileClient client = new SendFileClient(ip, port);
            client.sendFile(fname);
        } else if ("exit".equalsIgnoreCase(command.action)){
            System.exit(0);
            res = "System EXIT.";
        } else if ("pwd".equalsIgnoreCase(command.action)){
            res = System.getProperty("user.dir");
        } else if ("ip".equalsIgnoreCase(command.action)){
            res = getIP();
        } else if ("test".equalsIgnoreCase(command.action)){
            test();
        } else if (isLinux()){
            Process process = Runtime.getRuntime().exec(command.cmdStr);
            BufferedReader bfreader = new BufferedReader(new InputStreamReader(process.getInputStream(), StandardCharsets.UTF_8));
            String line = "";
            while ((line=bfreader.readLine()) != null) res += line + "\n";
        }
        else {
            res = "I don't know this command.";
        }
        return res;
    }

    public static void test(){
        try {
            while (true) {
                Socket socket = new Socket("114.214.205.186", 9190);
                DataOutputStream dos = new DataOutputStream(socket.getOutputStream());
                Scanner sc = new Scanner(System.in);
                dos.writeInt(100);   // Child thread to send file.
                dos.flush();

                Integer x = sc.nextInt();
                dos.writeLong(x);
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public static String getIP(){
        String res = "";
        try {
            Enumeration allNetInterfaces = null;
            allNetInterfaces = NetworkInterface.getNetworkInterfaces();
            InetAddress ip = null;
            while (allNetInterfaces.hasMoreElements())
            {
                NetworkInterface netInterface = (NetworkInterface) allNetInterfaces.nextElement();
                Enumeration addresses = netInterface.getInetAddresses();
                while (addresses.hasMoreElements())
                {
                    ip = (InetAddress) addresses.nextElement();
                    if (ip != null)  res += netInterface.getName() + "  " + ip.getHostAddress() + "\n";
                }
            }
        } catch (SocketException e) {
            e.printStackTrace();
        }

        return res;
    }

    public static boolean isLinux(){
        return System.getProperty("os.name").toLowerCase().contains("linux");
    }

    public static boolean isWindows(){
        return System.getProperty("os.name").toLowerCase().contains("windows");
    }

}
