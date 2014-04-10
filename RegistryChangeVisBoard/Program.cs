using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace RegistryChangeVisBoard
{
    static class Program
    {
        static void Main()
        {
            //This example reads from the file "VisBoards.txt" but it can be anything. This file contains a list of systems you want to run this reg change on. 
            int tempint = File.ReadAllLines("J:\\VisBoards.txt").Length;
            var system = new string[tempint];
            using (var sr = new StreamReader("J:\\VisBoards.txt"))
            {
                int r;
                for (r = 0; r < tempint; r++)
                {
                    system[r] = sr.ReadLine();
                }
            }
            int i;
            for (i = 0; i <tempint; i++)
            {
                ChangeReg(system[i]);
            }
        }

        static void ChangeReg(string system)
        {
            try
            {
                using (RegistryKey regSec = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, system))
                {
                    //For this example, I've chosen to change a value which disables UAC on a remote Windows Vista/7/8/8.1 workstation.  
                    RegistryKey regSecKey = regSec.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", true);
                    if (regSecKey != null) regSecKey.SetValue("EnableLUA", "0", RegistryValueKind.DWord);
                }
                using (var proc = new Process())
                {
                    //This will remotely reboot the remote workstation. 
                    proc.StartInfo.FileName = "shutdown.exe";
                    proc.StartInfo.Arguments = string.Format("-f -r -t 05 -m \\\\{0}", system);
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.Start();
                }
                Console.WriteLine(system + " is complete");
            }
            catch (Exception ex)
            {
                
               Console.WriteLine(ex.ToString());
            }
            
        }
    }
}
