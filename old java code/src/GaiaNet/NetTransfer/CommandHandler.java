package GaiaNet.NetTransfer;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.Socket;
import java.nio.charset.StandardCharsets;
import java.util.Arrays;
import java.util.Scanner;
import java.util.logging.Logger;


public class CommandHandler {
    private Socket sock;
    DataOutputStream dos;
    DataInputStream dis;

    public CommandHandler(){
        this.sock = null;
    }

    public int sendCmd(String cmd) throws IOException {
        Command cmds = cmdParse(cmd);
        if (cmds.udp) {udpSendCmd(cmds); return 0;}
        this.sock = new Socket(cmds.to, 9190);
        this.dos = new DataOutputStream(this.sock.getOutputStream());
        this.dis = new DataInputStream(this.sock.getInputStream());

        this.dos.writeInt(GaiaType.Command.ordinal());  // 0 represent Command;
        this.dos.writeUTF(cmds.cmdStr);
        this.dos.flush();
        String response = this.dis.readUTF();
        System.out.println(response);
        return 0; // normal
    }


    public int udpSendCmd(Command cmd) throws IOException {
        DatagramSocket ds = new DatagramSocket();
        InetAddress serverIp = InetAddress.getByName(cmd.to);
//        this.dos.writeInt(GaiaType.Command.ordinal());  // 0 represent Command;
        DatagramPacket dpSent = new DatagramPacket(cmd.cmdStr.getBytes(StandardCharsets.UTF_8),cmd.cmdStr.length(),serverIp,9190);
        ds.send(dpSent);

        return 0; // normal
    }


    public void handle(Socket socket, Logger logger){
        try {
            DataInputStream dis = new DataInputStream(socket.getInputStream());
            DataOutputStream dos = new DataOutputStream(socket.getOutputStream());
            String cmd = dis.readUTF();
            logger.info("Command From: "+socket.getRemoteSocketAddress()+" "+cmd);
            Command command = cmdParse(cmd);
            String result = CommandExec.exec(command);
            dos.writeUTF(result);
        } catch (IOException e) {
            e.printStackTrace();
        }

    }


    public void getCommand(){
        try {
            Scanner sc = new Scanner(System.in);
            String cmdstr;
            while (true) {
                System.out.print("GaiaNet :> ");
                cmdstr = sc.nextLine();
                if ( !("".equals(cmdstr.strip())) )
                    sendCmd(cmdstr);
            }
        } catch (Exception e){

        }
    }

//  [to ip] command options, or [udp ip] command options
//  [to ip], If no, ip is set local.
//  command.cmdStr : string form of command.
//  long parameters : --A B ==> [A:B],  --A ==> [A:""]
//  short parameters: -cpt  ==> [c:"", p:"", t:""],  -cpt a ==> [c:"a", c:"a", c:"a"]
//  Position Parameters : command.argList
    private Command cmdParse(String cmd){
        Command command = new Command();
        String[] cmdary = cmd.split(" ");
        int index = 0;
        if ("udp".equalsIgnoreCase(cmdary[0])){
            command.udp = true;
            command.to = cmdary[1];
            command.action = cmdary[2];
            command.cmdStr = String.join(" ", Arrays.copyOfRange(cmdary,2,cmdary.length));
            index = 3;
        }else if ("to".equalsIgnoreCase(cmdary[0])){
            command.to = cmdary[1];
            command.action = cmdary[2];
            command.cmdStr = String.join(" ", Arrays.copyOfRange(cmdary,2,cmdary.length));
            index = 3;
        }else {
            command.action = cmdary[0];
            command.cmdStr = cmd;
            index = 1;
        }

        while (index < cmdary.length){
            if (cmdary[index].contains("--")){
                // Long Parameters Parse
                String key = cmdary[index].substring(2);
                String value = "";
                if ((index+1 < cmdary.length) && !cmdary[index+1].contains("-")){
                    value = cmdary[index+1];
                    index += 2;
                } else {
                    index += 1;
                }
                command.longOption.put(key, value);
            } else if (cmdary[index].contains("-")){
                // Short Parameters Parse
                char[] keys = cmdary[index].substring(1).toCharArray();  //maybe like this: -cpt
                String value = "";
                if ((index+1 < cmdary.length) && !cmdary[index+1].contains("-")){
                    value = cmdary[index+1];
                    index += 2;
                } else {
                    index += 1;
                }

                for (char key : keys) {
                    command.shortOption.put(String.valueOf(key), value);
                }
            } else {
                // Position Parameters Parse
                command.argList.add(cmdary[index]);
                index += 1;
            }
        }
        return command;
    }

}
