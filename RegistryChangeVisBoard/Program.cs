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
                    RegistryKey regSecKey = regSec.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", true);
                    if (regSecKey != null) regSecKey.SetValue("EnableLUA", "0", RegistryValueKind.DWord);
                }
                using (var proc = new Process())
                {
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
