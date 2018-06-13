using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FTPClient
{
    public partial class Form1 : Form
    {
        private int BufferSize;
        private string IPA;
        private string FilePath;
        private int PortN;
        NetworkStream netstream = null;
        TcpClient client = null;
        public Form1()
        {
            IPA = "localhost";
            PortN = 21;
            FilePath = "C:\\Anime\\Initial_D_3rd_Stage.mkv";
            InitializeComponent();
            BufferSize = 4096;
            lblStatus.Text = "";
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            byte[] SendingBuffer = null;

            String filename = Path.GetFileName(FilePath);
            StreamWriter controlWriter = new StreamWriter(netstream);


            FileStream Fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            int NoOfPackets = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Fs.Length) / Convert.ToDouble(BufferSize)));
            progressBar1.Maximum = NoOfPackets;
            
            progressBar1.Step = 1;
            
            int TotalLength = (int)Fs.Length, CurrentPacketLength;
            int initialLength = TotalLength;
            logText.AppendText("File size: " + TotalLength + "\n");
            logText.AppendText("File size: " + Fs.Length + "\n");
            logText.AppendText("Buffer size: " + BufferSize + "\n");
            logText.AppendText("Number of packets: " + NoOfPackets + "\n");
            controlWriter.WriteLine("upload " + NoOfPackets + " " + filename);
            controlWriter.Flush();

            netstream.WriteTimeout = 100000;
            for (int i = 0; i < NoOfPackets; i++)
            {
                if (TotalLength > BufferSize)
                {
                    CurrentPacketLength = BufferSize;
                    TotalLength = TotalLength - CurrentPacketLength;
                }
                else
                {
                    CurrentPacketLength = TotalLength;
                }
                SendingBuffer = new byte[CurrentPacketLength];
                Fs.Read(SendingBuffer, 0, CurrentPacketLength);
                netstream.Write(SendingBuffer, 0, (int)SendingBuffer.Length);
                //netstream.Flush();
                if (progressBar1.Value >= progressBar1.Maximum)
                {
                    progressBar1.Value = progressBar1.Minimum;
                }
                
                progressBar1.PerformStep();
                logText.AppendText("Packet: " + i + " Send: " + (initialLength - TotalLength) + " Remains: " + TotalLength +  "\n");
            }

            lblStatus.Text = lblStatus.Text + "Sent " + Fs.Length.ToString() + " bytes to the server";
            Fs.Close();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            client = new TcpClient(IPA, PortN);
            lblStatus.Text = "Connecting...\n";
            netstream = client.GetStream();
            StreamReader controlReader = new StreamReader(netstream);
            lblStatus.Text = controlReader.ReadLine();
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            netstream.Close();
            client.Close();
            lblStatus.Text = "Disconnected";
        }
    }
}
