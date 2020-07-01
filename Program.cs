using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace noita_gif_converter
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.Title = "Noita gif to mp4 converter";


            Console.WriteLine("Welcome to Noita gif converter v2.43 beta!");
            Console.ReadKey();
            Console.WriteLine("Make sure your gifs to convert are in the 'gifs2convert' folder. \n Press any key to start the conversion.");
            Console.ReadLine();

            //create ffmpegcommands from filenames
            var ffmpegcommands = new List<string>();
            string fullpath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var directory = Path.GetDirectoryName(fullpath);

            string[] filestoconvert = Directory.GetFiles(directory + "/" + "gifs2convert", "*.gif");
            var filenames = new List<string>();

            foreach (string path in filestoconvert)
            {
                string filename = Path.GetFileName(path);
                char[] gifextensionremove = { 'f', 'i', 'g', '.' };
                string newfilename = filename.TrimEnd(gifextensionremove);
                filenames.Add(newfilename);
            }

            foreach (string filename in filenames)
            {
                ffmpegcommands.Add("ffmpeg -i gifs2convert/" + filename + ".gif -crf 10 -b:v 500K -movflags faststart -y -pix_fmt yuv420p -vf \"scale = trunc(iw / 2) * 2:trunc(ih / 2) * 2\" convertedgifs/" + filename + ".mp4");
            }

            foreach (string ffmpegcommand in ffmpegcommands)
            {
                Console.WriteLine(ffmpegcommand);
                Console.WriteLine("-------------------");

                using (Process process = new Process())
                {
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.WorkingDirectory = @"C:\";
                    process.StartInfo.FileName = Path.Combine(Environment.SystemDirectory, "cmd.exe");

                    process.StartInfo.RedirectStandardInput = true;

                    process.OutputDataReceived += ProcessOutputDataHandler;
                    process.ErrorDataReceived += ProcessErrorDataHandler;

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    process.StandardInput.WriteLine("cd " + directory);
                    process.StandardInput.WriteLine(ffmpegcommand);

                    process.WaitForExit(8000);


                }
            }

            Console.WriteLine("Conversion done!");
            Console.ReadKey();


        }

        public static void ProcessOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            Console.WriteLine(outLine.Data);
        }

        public static void ProcessErrorDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            Console.WriteLine(outLine.Data);
        }
    }
}
