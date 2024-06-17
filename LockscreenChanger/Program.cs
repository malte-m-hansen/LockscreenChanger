using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace LockscreenChanger
{
    class Program
    {
        static void Main(string[] args)
        {
            string source = Directory.GetCurrentDirectory() + @"\pictures";
            string active = Directory.GetCurrentDirectory() + @"\active";
            string lockscreen = active + @"\lockscreen.jpg.";
            string lastactive = Directory.GetCurrentDirectory() + @"\lastactive.txt";

            //****BEGIN PRECHECK****
            if (!File.Exists(lastactive))
            {
                using (var s = new StreamWriter(lastactive, true))
                {
                    s.Write("0");
                }
            }
            if (!Directory.Exists(source))
            {
                Directory.CreateDirectory(source);
                Log("Generated folder: " + source);
            }
            if (!Directory.Exists(active))
            {
                //
                Directory.CreateDirectory(active);
                Log("Generated folder: " + active);
            }   
            if (!Directory.EnumerateFileSystemEntries(source).Any())
            {
                Log("Error. No files pictures found in " + source);
                Thread.Sleep(3000);
                Environment.ExitCode = 1;
                Environment.Exit(1);
            }
            //****END PRECHECK****
            bool b = true;
            while (b)
            {
                List<FileInfo> list = new DirectoryInfo(source).GetFiles().ToList();
                var rnd = new Random();
                int index = rnd.Next(list.Count);
                string selected = list[index].ToString();
                Thread.Sleep(500);
                Log("Selected = " + selected);
                string last = File.ReadAllText(lastactive);
                Log("Last = " + last);
                File.WriteAllText(lastactive, selected);
                if (selected != last && selected != "Thumbs.db")
                {
                    if (File.Exists(lockscreen))
                    {
                        File.Delete(lockscreen);
                        Log("Deleted current lockscreen from active folder");
                    }
                    try
                    {
                        File.Copy(source + @"\" + selected, lockscreen);
                        Log("Sucessfully copied: " + selected);
                        Thread.Sleep(2000);
                        b = false;
                    }
                    catch (Exception e)
                    {
                        Log("Could not copy selected to active folder: "+ e);
                        Thread.Sleep(2000);
                        Environment.Exit(2);
                    }
                    
                }
                else
                {
                    Log("ERROR: Dupe selection or thumbs.db: " + selected);
                }

            }


            Thread.Sleep(3000);
        }
        private static void Log(string t)
        {
            t = DateTime.Now.ToString() + " " + t;
            Console.WriteLine(t);
            using (var s = new StreamWriter(Directory.GetCurrentDirectory()+@"\log.log", true))
            {
                s.WriteLine(t);
            }
        }
    }
}
