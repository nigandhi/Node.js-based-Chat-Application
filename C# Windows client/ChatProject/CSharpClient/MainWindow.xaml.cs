using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.ServiceModel;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Windows.Forms;

namespace CSharpClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {        
        Socket soc;
        System.Net.IPEndPoint remoteEP;
        System.Net.IPAddress ipAdd;
        TcpClient client;
        public MainWindow()
        {
            InitializeComponent();
            
            Thread t1 = new Thread(new ThreadStart(() => { ConnectToServer(); }));
            t1.IsBackground = true;
            t1.Start();
            Thread t2 = new Thread(new ThreadStart(() => { ConnectToServer_FileTransfer(); }));
            t2.IsBackground = true;
            t2.Start();
            Thread.Sleep(100);
            Thread t = new Thread(new ThreadStart(() => { ClientRecieverMain(); }));
            t.IsBackground = true;
            t.Start();
            Thread t3 = new Thread(new ThreadStart(() => { ClientRecieverMain_FileTransfer(); }));
            t3.IsBackground = true;
            t3.Start();
       
        }
        public void ConnectToServer()
        {
            soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //ipAdd = System.Net.IPAddress.Parse("10.1.70.50");
            ipAdd = System.Net.IPAddress.Parse("192.168.0.18");
            remoteEP = new System.Net.IPEndPoint(ipAdd, 1337);
            soc.Connect(remoteEP);
        }

        public void ConnectToServer_FileTransfer()
        {
            client = new TcpClient(AddressFamily.InterNetwork);
            //client.Connect("10.1.70.50", 1339);
            client.Connect("192.168.0.18", 1339);
        }

        private void Send_Data(object sender, RoutedEventArgs e)
        {            
            string str = Text.Text;            
            byte[] byData = System.Text.Encoding.ASCII.GetBytes(Text.Text+"\n");
            soc.Send(byData);
            Dispatcher.BeginInvoke(new Action(() => ChatScreen.Items.Add("Windows: "+str)));
            Text.Text = "";            
        }
        public void ClientRecieverMain()
        {           
            while (true)
            {
                byte[] buffer = new byte[1024];
                int iRx = soc.Receive(buffer);
                char[] chars = new char[iRx];
                System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                int charLen = d.GetChars(buffer, 0, iRx, chars, 0);
                System.String recv = new System.String(chars);
                Dispatcher.BeginInvoke(new Action(() => ChatScreen.Items.Add("Linux: "+recv)));
            }
        }
        public void ClientRecieverMain_FileTransfer()
        {
            if (client.Connected)
            {
                NetworkStream stream = client.GetStream();
                while(true)
                {  
                    byte[] dataBufer = new byte[1024];
                    int read=0;
                    int temp = 0;
                    Int32 read1=stream.Read(dataBufer, 0, dataBufer.Length);
                    string filename = System.Text.Encoding.ASCII.GetString(dataBufer,0,read1);
                    FileStream fs = File.Create(filename);
                    Dispatcher.BeginInvoke(new Action(() => ChatScreen.Items.Add("Receiving File: "+ filename)));
                    while ((read = stream.Read(dataBufer, 0, dataBufer.Length)) > 0)
                    {
                        temp += read;
                        fs.Write(dataBufer, 0, dataBufer.Length);
                        //read1 = stream.Read(dataBufer, 0, dataBufer.Length);
                        string end = System.Text.Encoding.ASCII.GetString(dataBufer, 0, read);
                        if (end.Contains("@End@")/*read < 1024*/)
                        {
                            Dispatcher.BeginInvoke(new Action(() => ChatScreen.Items.Add(temp)));
                            break;
                        }
                        //fs.Write(dataBufer, 0, dataBufer.Length);
                    }
                    fs.Close();
                    Dispatcher.BeginInvoke(new Action(() => ChatScreen.Items.Add("File Received")));
                }
            }
        }
        private void OpenFS(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Any Files|*.*";
            openFileDialog1.Title = "Select a Cursor File";
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                byte[] instr = System.Text.Encoding.ASCII.GetBytes(openFileDialog1.SafeFileName);
                byte[] endinstr = System.Text.Encoding.ASCII.GetBytes("End");
                byte[] buffer = File.ReadAllBytes(openFileDialog1.FileName);
                NetworkStream stream = client.GetStream();
                stream.Write(instr, 0, instr.Length);
                Thread.Sleep(100);
                stream.Write(buffer, 0, buffer.Length);
                Thread.Sleep(300);
                stream.Write(endinstr, 0, endinstr.Length);
                Thread.Sleep(10);
            }
        }

    }
}
