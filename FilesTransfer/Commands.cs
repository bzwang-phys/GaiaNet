using System;
using CommandLine;
using System.Text;

namespace GaiaNet.FilesTransfer
{
    class CmdSendFile
    {
        [Option('d', "dest", Required = true, HelpText = "Which host to communicate")]
        public String dest {get; set;}

        [Option('f', "from", Required = true, HelpText = "Where the files come from ?")]
        public String from {get; set;}

        [Option('t', "to", Required = false, HelpText = "Where the files are transformed to ?")]
        public String to {get; set;}
        
        [Value(index:0, Required = true, HelpText = "The command string, main operation.")]
        public String cmd {get; set;}

        public CmdSendFile()
        {
            this.to = "./";
        }
    }


    class CmdGetFiles
    {
        [Option('d', "dest", Required = true, HelpText = "Which host to communicate")]
        public String dest {get; set;}

        [Option('f', "from", Required = true, HelpText = "Where the files come from ?")]
        public String from {get; set;}

        [Option('t', "to", Required = false, HelpText = "Where the files are transformed to ?")]
        public String to {get; set;}
        
        [Value(index:0, Required = true, HelpText = "The command string, main operation.")]
        public String cmd {get; set;}

        public CmdGetFiles()
        {
            this.to = "./";
        }
    }

    class FileCmdHandler
    {
        public static int handler(string cmdStr){
            string[] cmdList = cmdStr.Split(null);
            CmdSendFile cmd = null;
            Parser.Default.ParseArguments<CmdSendFile>(cmdList).WithParsed<CmdSendFile>(o=>{ cmd = o; });

            SendFiles sendFiles = new SendFiles(cmd.dest, 9091);
            sendFiles.SendFile(cmd.from);
            return 0;
        }

    }


}
