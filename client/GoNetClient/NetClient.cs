using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace GoNetClient
{
    public class NetClient
    {
        private NetworkStream outStream = null;
        private MemoryStream memStream;
        private BinaryReader reader;
        private const int MAX_READ = 8192;
        private byte[] byteBuffer = new byte[MAX_READ];

        protected TcpClient client = null;

        public bool IsConnect
        {
            get { return client!=null&&client.Connected; }
        }
        public bool ConnectTo(string serverIp, int port)
        {
            client = new TcpClient();
            client.SendTimeout = 1000;
            client.ReceiveTimeout = 1000;
            client.NoDelay = true;

            client.BeginConnect(serverIp, port, new AsyncCallback(OnConnect), null);
            return true;
        }

        void OnConnect(IAsyncResult asr)
        {
            outStream = client.GetStream();
            client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
        }
        void OnRead(IAsyncResult asr)
        {
            int bytesRead = 0;
            try
            {
                lock (client.GetStream())
                {         //读取字节流到缓冲区
                    bytesRead = client.GetStream().EndRead(asr);
                }
                if (bytesRead < 1)
                {                //包尺寸有问题，断线处理
                    Close("bytesRead < 1");
                    return;
                }
                OnReceive(byteBuffer, bytesRead);   //分析数据包内容，抛给逻辑层

                lock (client.GetStream())
                {         //分析完，再次监听服务器发过来的新消息
                    Array.Clear(byteBuffer, 0, byteBuffer.Length);   //清空数组
                    client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
                }
            }
            catch (Exception ex)
            {
                Exception("OnRead"+ex.ToString());
                Close(ex.Message);
            }
        }

        public void Close(string reason)
        {
            if (client != null)
            {
                if (client.Connected) client.Close();
                client = null;
            }
        }

        #region out interface

        LSCmdPack cmdpack = new LSCmdPack();
        void OnReceive(byte[] byteBuffer, int bytesRead)
        {
            Console.WriteLine("OnReceive:" + Encoding.UTF8.GetString(byteBuffer) + " size:" + bytesRead);
            //if (cmdpack.OnReceive(byteBuffer, 0, bytesRead))
            //{
            //    ByteBuffer buffer;
            //    while ((buffer = cmdpack.GetPackBuffer()) != null)
            //    {
            //        RecvBuffer(buffer);

            //    }
            //}
        }
        void RecvBuffer(ByteBuffer buffer)
        {
            //if (IsReadCmdMainThread)
            //{
            //    lock (waitCmdBuffers)
            //    {
            //        waitCmdBuffers.Add(buffer);
            //        Interlocked.Increment(ref waitCmdCounter);
            //    }
            //}
            //else
            //{
            //    PostBuffer(buffer);
            //}
        }
        void PostBuffer(ByteBuffer buffer)
        {
            //OperationRequest request = new OperationRequest(buffer);
            //listener.OnRecvRequest(request);
        }

        void Exception(string err)
        {
            Console.WriteLine("Exception:" + err);
        }

        public void WriteMessage(byte[] message)
        {
            Console.WriteLine("WriteMessage "+message.Length);
            MemoryStream ms = null;
            using (ms = new MemoryStream())
            {
                ms.Position = 0;
                BinaryWriter writer = new BinaryWriter(ms);
                ushort msglen = (ushort)message.Length;
                //writer.Write(msglen);
                writer.Write(message);
                writer.Flush();
                if (client != null && client.Connected)
                {
                    //NetworkStream stream = client.GetStream(); 
                    byte[] payload = ms.ToArray();
                    outStream.BeginWrite(payload, 0, payload.Length, new AsyncCallback(OnWrite), null);
                }
                else
                {
                    Exception("client.connected----->>false");
                }
            }
        }


        /// <summary>
        /// 向链接写入数据流
        /// </summary>
        void OnWrite(IAsyncResult r)
        {
            try
            {
                outStream.EndWrite(r);
            }
            catch (Exception ex)
            {
                Exception("OnWrite--->>>" + ex.Message);
            }
        }

        #endregion
    }
}
