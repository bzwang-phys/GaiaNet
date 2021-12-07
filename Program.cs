using System;
using GaiaNet.BasicNet;
using GaiaNet.GaiaNets;
using GaiaNet.Command;
using CommandLine;
using System.Net;
using System.Linq;

namespace GaiaNet
{
    public class MainOptions{
        [Value(index:0, Required = false, HelpText = "type of server: server, test, shell")]
        public string ServerType { get; set;}

        [Option(shortName:'c', longName:"ctype", Required = false, HelpText = "NO")]
        public string ctype { get; set;}
    }



    class Program
    {
        static void Main(string[] args)
        {
            MainOptions mainOptions = null;
            Parser.Default.ParseArguments<MainOptions>(args).WithParsed<MainOptions>(o=>{ mainOptions = o; });

            String type = mainOptions.ServerType;
            if (type == null)
                type = "shell";
            DaemonServer nt = new DaemonServer(type);

            
        }
    }
}
