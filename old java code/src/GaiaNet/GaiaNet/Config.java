package GaiaNet.GaiaNet;

import java.util.List;
import java.util.Map;

public class Config {
    public String nodeName;
    public List<String> masterNodesIp;
    public List<String> neighbours;


    @Override
    public String toString() {
        return "Config{" +
            "nodeName='" + nodeName + '\'' +
            ", masterNodesIp='" + masterNodesIp + '\'' +
            ", neighbours='" + neighbours + '\'' +
            '}';
    }

//    private String neighboursSting(){
//        String s = "";
//        for (Map.Entry<String, String> entry : this.neighbours.entrySet())
//            s += entry.getKey() + ":" + entry.getValue() + ", ";
//        return s;
//    }
}
