using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNetClient
{
    class Codec
    {

        LSCmdPack cmdpack = new LSCmdPack();
        void UnPackCmd(byte[] bytes, int count)
        {
            if (cmdpack.OnReceive(bytes, 0, count))
            {
                ByteBuffer buffer;
                while ((buffer = cmdpack.GetPackBuffer()) != null)
                {
                    RecvBuffer(buffer);

                }
            }
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

    }
}
