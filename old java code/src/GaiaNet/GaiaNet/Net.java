package GaiaNet.GaiaNet;

import com.google.gson.Gson;

import java.io.IOException;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.List;

public class Net {
    public DHTLikeNet net;
    public Node node;

    public Net(){
        Config config = readConfig();
        this.node = new Node("local", config);
        this.net = new DHTLikeNet(this.node, config);
    }




    public Config readConfig(){
        List<String> lines = null;
        try {
            lines = Files.readAllLines(Paths.get("Config.conf"), StandardCharsets.UTF_8);
            String jsonTxt = String.join(" ",lines);
            Gson gson = new Gson();
            Config config = gson.fromJson(jsonTxt, Config.class);
            return config;
        } catch (IOException e) {
            e.printStackTrace();
            System.out.println("Failed in reading the Config.conf.");
            return null;
        }
    }


}
