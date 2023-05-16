using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace I_LOVE_PIPES
{
    class Program
    {
        static void Main()
        {
            HashSet<string> lastPipes = new HashSet<string>();
            bool started = false;
            while (true)
            {
                // Execute PowerShell command to get the list of named pipes
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = "-Command [System.IO.Directory]::GetFiles('\\\\.\\\\pipe\\\\\\')",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };

                Process process = Process.Start(psi);
                string output = process.StandardOutput.ReadToEnd().Trim();
                process.WaitForExit();

                // Split the string by newline characters to get individual pipe names
                HashSet<string> currentPipes = new HashSet<string>(
                    output.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                          .Select(pipe => pipe.Trim())
                );

                // Find the newly created pipes
                IEnumerable<string> newPipes = currentPipes.Except(lastPipes);
                foreach (string pipe in newPipes)
                {
                    if(pipe.Contains("powershell") || started == false)
                    {
                        break;
                    }
                    Console.WriteLine("New pipe created: " + pipe);
                }

                // Update the last seen pipes
                lastPipes = currentPipes;

                if(!started)
                {
                    Console.WriteLine("New pipes will be printed here. Made by Alawapr#4968");
                }
                started = true;

                // Delay for 1 second before running the loop again
                Thread.Sleep(1000);
            }
        }
    }
}
