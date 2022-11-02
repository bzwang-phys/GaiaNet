using System;
using CommandLine;
using GaiaNet.BasicNet;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "./config/log4net.conf")]
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
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().ReflectedType);
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
