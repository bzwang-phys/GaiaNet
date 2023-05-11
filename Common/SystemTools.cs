using System;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GaiaNet.Common
{
public class SystemTools{

    /// <summary>  
    /// Execute the shell command and return the result.  
    /// </summary>  
    /// <param name="shellcommand"> Shell Command String </param>  
    /// <param name="milliseconds"> Time to wait(milliseconds), the default value is 0，which means always wait.</param>  
    /// <returns> Return the result of command execution </returns>  
    public static string Execute(string command, int seconds = 0)
    {
        string output = "";  
        if (command != null && !command.Equals(""))
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "powershell.exe" : "/bin/bash";
            startInfo.Arguments = "/C " + command;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardInput = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.CreateNoWindow = true;
            startInfo.WorkingDirectory = "./";
            process.StartInfo = startInfo;
            try
            {
                if (process.Start())
                {
                    if (seconds == 0) process.WaitForExit();
                    else process.WaitForExit(seconds);
                    output = process.StandardOutput.ReadToEnd();
                }
            }
            catch(Exception ex){ Console.WriteLine(ex.Message); }
            finally{ if (process != null) process.Close(); }
        }
        return output;
    }  


}
}