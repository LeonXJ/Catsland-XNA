using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Collections;
using System.Threading;
using System.Reflection;

namespace Catsland.Core {
    public class UDPConsolePanel : IConsolePanel{

#region Properties

        UdpClient m_udpServer;
        IPEndPoint m_ipEndPoint;
        Queue m_inputMsg;
        Thread m_thread;
        CatConsole m_console;
        bool m_exit = false;

#endregion

        public UDPConsolePanel(CatConsole _console) {
            m_console = _console;

        }

        

        public void Start() {
            if (m_thread == null) {
                m_thread = new Thread(Do);
                m_exit = false;
                m_thread.Start();
            }
            else {
                Console.Out.WriteLine("The thread has been running.");
            }
        }

        void Do() {
            m_inputMsg = Queue.Synchronized(new Queue());
            m_udpServer = new UdpClient(8819);
            m_ipEndPoint = new IPEndPoint(IPAddress.Any, 8818);

            //m_udpServer.Client.ReceiveTimeout = 5000;
            //m_udpServer.Client.Blocking = false;
            while (!m_exit) {
                try {
                    byte[] receive = m_udpServer.Receive(ref m_ipEndPoint);
                    if (receive.Length > 0) {
                        string rec_str = Encoding.Default.GetString(receive);
                        Console.Out.WriteLine(rec_str);
                        m_inputMsg.Enqueue(rec_str);
                    }
                }
                catch (SocketException) {
                }
            }
        }

        public void Update() {
            while (m_inputMsg.Count > 0) {
                String str = m_inputMsg.Dequeue() as String;
                IConsoleCommand commend = CatConsole.IntepreteCommendString(str);
                if (commend != null) {
                    m_console.PushCommend(commend, this);
                }
            }
        }

        public void GetResult(object _result) {
            if (_result.GetType() == typeof(String)) {
                byte[] bytes = Encoding.Default.GetBytes(_result as String);
                m_udpServer.Send(bytes, bytes.Length, m_ipEndPoint);
            }
        }

        public void Stop() {
            m_exit = true;
            m_udpServer.Close();
            //m_thread.Abort();
        }
    }
}
