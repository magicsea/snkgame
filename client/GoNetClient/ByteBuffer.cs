using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.IO;
namespace GoNetClient
{
    public enum NetObjetType
    {
        TUnknow = 0,
        TCustomObj,
        TORMObj,
        TARRAY,
        TDictionary,
        TList,
        TVector3,
        TByte,
        TInt,
        TLong,
        TShort,
        TFloat,
        TDouble,
        TString,
        
    }
    public class ByteBuffer
    {
        MemoryStream stream = null;
        BinaryWriter writer = null;
        BinaryReader reader = null;
        bool m_isWriteType = false;
        public ByteBuffer(bool writeType=false)
        {
            m_isWriteType = writeType;
            stream = new MemoryStream();
            writer = new BinaryWriter(stream);
        }

        public ByteBuffer(byte[] data, int offset,int count)
        {
            if (data != null)
            {
                stream = new MemoryStream(data, offset, count);
                //stream.Seek(offset, SeekOrigin.Begin);
                reader = new BinaryReader(stream);

            }
            else
            {
                stream = new MemoryStream();
                writer = new BinaryWriter(stream);
            }
        }

        public void Reset(byte[] data, int offset, int count)
        {
            if (data != null)
            {
                stream.SetLength(0);
                stream.Position = 0;
                writer.Write(data,offset,count);
            }
            else
            {
                stream.SetLength(0);
                stream.Position = 0;
            }
        }

        public void Close()
        {
            if (writer != null) writer.Close();
            if (reader != null) reader.Close();

            stream.Close();
            writer = null;
            reader = null;
            stream = null;
        }

        public void Write(object obj)
        {
            Type type = obj.GetType();
            if (type.Equals(typeof(Byte)))
	        {
		        WriteByte((byte)obj);
	        }
            else if (type.Equals(typeof(int)))
            {
                WriteInt((int)obj);
            }
            else if (type.Equals(typeof(long)))
            {
                WriteLong((long)obj);
            }
            else if (type.Equals(typeof(ushort)))
            {
                WriteShort((ushort)obj);
            }
            else if (type.Equals(typeof(float)))
            {
                WriteFloat((float)obj);
            }
            else if (type.Equals(typeof(double)))
            {
                WriteDouble((double)obj);
            }
            else if (type.Equals(typeof(string)))
            {
                WriteString((string)obj);
            }
        }

        public static NetObjetType GetNetObjectType(Type type)
        {
            if (type.Equals(typeof(Byte)))
            {
                return NetObjetType.TByte;
            }
            else if (type.Equals(typeof(int)))
            {
                return NetObjetType.TInt;
            }
            else if (type.Equals(typeof(long)))
            {
                return NetObjetType.TLong;
            }
            else if (type.Equals(typeof(ushort)))
            {
                return NetObjetType.TShort;
            }
            else if (type.Equals(typeof(float)))
            {
                return NetObjetType.TFloat;
            }
            else if (type.Equals(typeof(double)))
            {
                return NetObjetType.TDouble;
            }
            else if (type.Equals(typeof(string)))
            {
                return NetObjetType.TString;
            }

            return NetObjetType.TUnknow;
        }


        public void WriteByte(int v)
        {
            writer.Write((byte)v);
        }

        public void WriteInt(int v)
        {
            writer.Write((int)v);
        }

        public void WriteShort(ushort v)
        {
            writer.Write((ushort)v);
        }

        public void WriteLong(long v)
        {
            writer.Write((long)v);
        }

        public void WriteFloat(float v)
        {
            byte[] temp = BitConverter.GetBytes(v);
            Array.Reverse(temp);
            writer.Write(BitConverter.ToSingle(temp, 0));
        }

        public void WriteDouble(double v)
        {
            byte[] temp = BitConverter.GetBytes(v);
            Array.Reverse(temp);
            writer.Write(BitConverter.ToDouble(temp, 0));
        }

        public void WriteString(string v)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(v);
            writer.Write((ushort)bytes.Length);
            writer.Write(bytes);
        }


        public object Read(NetObjetType totype)
        {
            switch (totype)
            {
                case NetObjetType.TByte:
                    return ReadByte();
                case NetObjetType.TInt:
                    return ReadInt();
                case NetObjetType.TLong:
                    return ReadLong();
                case NetObjetType.TShort:
                    return ReadShort();
                case NetObjetType.TFloat:
                    return ReadFloat();
                case NetObjetType.TDouble:
                    return ReadDouble();
                case NetObjetType.TString:
                    return ReadString();
                default:
                    break;
            }

            return 0;
        }
        public int ReadByte()
        {
            return (int)reader.ReadByte();
        }

        public int ReadInt()
        {
            return (int)reader.ReadInt32();
        }

        public ushort ReadShort()
        {
            return (ushort)reader.ReadInt16();
        }

        public long ReadLong()
        {
            return (long)reader.ReadInt64();
        }

        public float ReadFloat()
        {
            byte[] temp = BitConverter.GetBytes(reader.ReadSingle());
            Array.Reverse(temp);
            return BitConverter.ToSingle(temp, 0);
        }

        public double ReadDouble()
        {
            byte[] temp = BitConverter.GetBytes(reader.ReadDouble());
            Array.Reverse(temp);
            return BitConverter.ToDouble(temp, 0);
        }

        public string ReadString()
        {
            ushort len = ReadShort();
            byte[] buffer = new byte[len];
            buffer = reader.ReadBytes(len);
            return Encoding.UTF8.GetString(buffer);
        }

        public byte[] ToBytes()
        {
            writer.Flush();
            return stream.ToArray();
        }

        public void Flush()
        {
            writer.Flush();
        }
    }

}
