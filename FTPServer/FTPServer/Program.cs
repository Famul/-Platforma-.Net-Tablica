using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FTPServer
{

    public class FtpServer
    {
        int BufferSize;
        private TcpListener _listener;
        NetworkStream stream;
        TcpClient client;

        public FtpServer()
        {
            BufferSize = 4096;

        }

        public void Start()
        {
            _listener = new TcpListener(IPAddress.Any, 21);
            _listener.Start();

            _listener.BeginAcceptTcpClient(HandleAcceptTcpClient, _listener);
        }

        public void Stop()
        {
            if (_listener != null)
            {
                _listener.Stop();
            }
        }

        private void HandleAcceptTcpClient(IAsyncResult result)
        {
            client = _listener.EndAcceptTcpClient(result);
            _listener.BeginAcceptTcpClient(HandleAcceptTcpClient, _listener);

            ThreadPool.QueueUserWorkItem(handleClient, client);


        }

        private void handleClient(Object obj)
        {

            stream = client.GetStream();


            using (StreamWriter writer = new StreamWriter(stream, Encoding.ASCII))
            using (StreamReader reader = new StreamReader(stream, Encoding.ASCII))
            {
                writer.AutoFlush = true;
                writer.WriteLine("Ready!");

                string line = reader.ReadLine();
                
                String[] command = line.Split(' ');
                Console.WriteLine(line);
                Console.WriteLine(command[1]);
                string cmd = command[0].ToUpperInvariant();
                int packets = int.Parse(command[1]);
                string arguments = command.Length > 1 ? line.Substring(command[0].Length + command[1].Length + 2) : null;
                if (string.IsNullOrWhiteSpace(arguments))
                    arguments = string.Empty;
                if (cmd == "UPLOAD")
                    uploadFile(arguments, packets);
            }
        }

        private void uploadFile(string arguments, int packets)
        {



            string SaveFileName = string.Empty;
            byte[] RecData = new byte[BufferSize];
            int RecBytes;
            {
                SaveFileName = arguments;
            }
            Console.WriteLine("File name: " + SaveFileName);
            if (SaveFileName != string.Empty)
            {
                int totalrecbytes = 0;
                SaveFileName = "C:\\FTP\\" + SaveFileName;
                //FileStream Fs = new FileStream(SaveFileName, FileMode.OpenOrCreate, FileAccess.Write);
                stream.ReadTimeout = 100000;
                
                using (var outputStream = File.OpenWrite(SaveFileName))
                {

                    for(int i = 0; i < packets; i++)
                    {
                        RecBytes = stream.Read(RecData, 0, RecData.Length);
                        outputStream.Write(RecData, 0, RecBytes);
                        totalrecbytes += RecBytes;
                        //stream.Flush();
                    }/*
                    while ((RecBytes = stream.Read(RecData, 0, RecData.Length)) > 0)
                    {
                        outputStream.Write(RecData, 0, RecBytes);
                        totalrecbytes += RecBytes;
                        stream.Flush();
                    }*/
                    outputStream.Flush();
                }
               // Fs.Close();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            FtpServer server = new FtpServer();
            server.Start();
            Console.WriteLine("Server started");
            string text = Console.ReadLine();
            while (text != "exit") { text = Console.ReadLine(); }
            server.Stop();
            Console.WriteLine("Server stopped");
            Console.ReadKey();
        }
    }
}
