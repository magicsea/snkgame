using System;
using System.Collections.Generic;
////using System.Linq;
using System.Text;
////using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace GoNetClient
{
    public class LSCmdPack
    {
        private MemoryStream m_memStream;
        private BinaryReader m_reader;
        public List<ByteBuffer> m_bufferList;
        public LSCmdPack()
        {
            m_memStream = new MemoryStream();
            m_reader = new BinaryReader(m_memStream);
            m_bufferList = new List<ByteBuffer>();
        }

        public void Reset()
        {
            m_memStream.SetLength(0);
            m_memStream.Position = 0;
            m_bufferList.Clear();
        }
        /// <summary>
        /// 解析包
        /// 结构:
        /// ushort->下面包长度
        /// byte[]->包内容
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public bool OnReceive(byte[] bytes, int offset,int length)
        {
            bool haveCmd = false;
            m_memStream.Seek(0, SeekOrigin.End);
            m_memStream.Write(bytes, offset, length);
            //Reset to beginning
            m_memStream.Seek(0, SeekOrigin.Begin);
            while (RemainingBytes > 2)
            {
                ushort messageLen = m_reader.ReadUInt16();
                if (RemainingBytes >= messageLen)
                {
                    ByteBuffer buffer = new ByteBuffer(m_reader.ReadBytes(messageLen), 0, messageLen);
                    m_bufferList.Add(buffer);
                    haveCmd = true;
                    //MemoryStream ms = new MemoryStream();
                    //BinaryWriter writer = new BinaryWriter(ms);
                    //writer.Write(m_reader.ReadBytes(messageLen));
                    //ms.Seek(0, SeekOrigin.Begin);
                    //OnReceivedMessage(ms);
                }
                else
                {
                    //Back up the position two bytes,wait other bytes
                    m_memStream.Position = m_memStream.Position - 2;
                    break;
                }
            }
            //Create a new stream with any leftover bytes
            byte[] leftover = m_reader.ReadBytes((int)RemainingBytes);
            m_memStream.SetLength(0);     //Clear
            m_memStream.Write(leftover, 0, leftover.Length);

            return haveCmd;
        }

        public ByteBuffer GetPackBuffer()
        {
            if (m_bufferList.Count < 1)
                return null;
            ByteBuffer buffer = m_bufferList[0];
            m_bufferList.RemoveAt(0);
            return buffer;
        }

        /// <summary>
        /// 剩余的字节
        /// </summary>
        private long RemainingBytes
        {
            get
            {
                return m_memStream.Length - m_memStream.Position;
            }
        }

    }
}
