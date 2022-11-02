using System;
using CommandLine;
using System.Text;

namespace GaiaNet.Command
{
    class CommandTo
    {
        [Option(shortName:'u', longName:"udp", Required = false, HelpText = "Use udp connection to send command.")]
        public Boolean udp {get; set;}

        [Option('t', "to", Required = false, HelpText = "Where to execute the command")]
        public String to {get; set;}
        
        [Value(index:0, Required = true, HelpText = "Command String")]
        public String cmdStr {get; set;}

        public CommandTo()
        {
            this.to = "127.0.0.1";
            this.udp = false;
        }
    }





}
