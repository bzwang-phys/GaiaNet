package GaiaNet.NetTransfer;

import java.util.ArrayList;
import java.util.HashMap;

public class Command {
    public boolean udp = false;
    public String to;
    public String action;
    public String cmdStr;
    public ArrayList<String> argList = new ArrayList<>();
    public HashMap<String, String> shortOption = new HashMap<>();
    public HashMap<String, String> longOption = new HashMap<>();


    public Command(){
        this.to = "127.0.0.1";
        this.action = "";
    }

    @Override
    public String toString(){
        StringBuilder s = new StringBuilder("To: " + this.to);
        s.append("\nAction: ").append(this.action);
        s.append("\nPosition Parameters: ").append(this.argList.toString());
        for (HashMap.Entry<String,String> entry : this.shortOption.entrySet()) {
            s.append("\n").append(entry.getKey()).append(": ").append(entry.getValue());
        }
        for (HashMap.Entry<String,String> entry : this.longOption.entrySet()) {
            s.append("\n").append(entry.getKey()).append(": ").append(entry.getValue());
        }
        return s.toString();
    }
}
