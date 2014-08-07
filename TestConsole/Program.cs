using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Catsland.Editor;
using System.Threading;
using Catsland.Core;

namespace Catsland.Test {
    class Program {

        

        class DebugClient {

            UdpClient m_udpClient;
            IPEndPoint m_ipEndPoint;
            Thread m_listenningThread;
            bool m_exit = false;

            public void Start() {
                m_udpClient = new UdpClient(8818);
                m_ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8819);
                m_listenningThread = new Thread(ListenThread);
                m_listenningThread.Start();
            }

            public void WaitForInput() {
                while (true) {
                    string input = Console.ReadLine();
                    if (input == "exit client") {
                        break;
                    }
                    byte[] bytes = Encoding.Default.GetBytes(input);
                    m_udpClient.Send(bytes, bytes.Length, m_ipEndPoint);
                }
            }

            public void ListenThread() {
                while (!m_exit) {
                    try {
                        byte[] receive = m_udpClient.Receive(ref m_ipEndPoint);
                        if (receive.Length > 0) {
                            string rec_str = Encoding.Default.GetString(receive);
                            Console.Out.WriteLine(rec_str);
                        }
                    }
                    catch (SocketException) {
                    }
                }
            }

            public void Stop() {
                m_exit = true;
                m_udpClient.Close();
                //m_thread.Abort();
            }

        }

        static void Main(string[] args) {

            //System.Diagnostics.Process.Start("CatsEditor.exe", "test");

            DebugClient dc = new DebugClient();
            dc.Start();
            dc.WaitForInput();
            dc.Stop();

//             Thread gameEditorThread = new Thread(RunGameEditorThread);
//             gameEditorThread.Start();


//             UdpClient udpClient = new UdpClient(8818);
//             IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8819);
// 

            

            //TestEditScene.RunTest();
            //SendMessage("run ");
            //gameEditorThread.Abort();

        }

        static void RunGameEditorThread() {
            Entrance.Run(null);
        }
    }
}
