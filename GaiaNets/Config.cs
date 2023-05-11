using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace GaiaNet.GaiaNets
{
    public class Config {
        public static String nodeName { get; set; }
        public static String relayNode { get; set; }
        public static int serverPort { get; set; }
        public static Dictionary<String,String> masterNodesIp { get; set; }
        public static Dictionary<String,String> neighbours { get; set; }
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().ReflectedType);

        public static string toString() {
            return "Config{" +
                "nodeName=" + Config.nodeName + ", relayDest=" + Config.relayNode + 
                ", serverPort=" + Config.serverPort.ToString() +
                ", \n masterNodesIp=" + String.Join(", ",Config.masterNodesIp.Select(x=>x.Key +" = "+ x.Value).ToArray()) +
                ", \n neighbours=" + String.Join(", ",Config.neighbours.Select(x=>x.Key +" = "+ x.Value).ToArray()) +
                '}';
        }
        public static void ReadConfig() {
            try {
                // var options = new JsonSerializerOptions { WriteIndented = true };
                // string jsonString = JsonSerializer.Serialize(weatherForecast, options);
                if (!File.Exists("./config/net.conf")){
                    Console.WriteLine("File ./config/net.conf does not exists.");
                    System.Environment.Exit(1);
                }
                string confString = File.ReadAllText("./config/net.conf", System.Text.Encoding.UTF8);
                ConfigJson config = JsonSerializer.Deserialize<ConfigJson>(confString);
                Config.nodeName = config.nodeName;
                Config.relayNode = config.relayNode;
                Config.serverPort = config.serverPort;
                Config.masterNodesIp = config.masterNodesIp;
                Config.neighbours = config.neighbours;
                log.Info("Get configuration: " + toString());
                Console.WriteLine("Get configuration: " + toString());
            } catch (IOException e) {
                Console.WriteLine("Failed in getting the net.conf.");
                log.Info("Failed in getting the net.conf.");
                log.Error(e);
            }
        }
    }

    public class ConfigJson {
        public String nodeName { get; set; }
        public String relayNode { get; set; }
        public int serverPort { get; set; }
        public Dictionary<String, String> masterNodesIp { get; set; }
        public  Dictionary<String, String> neighbours { get; set; }
    }
}