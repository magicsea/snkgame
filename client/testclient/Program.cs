using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace testclient
{
    using GoNetClient;
    using Newtonsoft.Json;
    public class AddReq 
    {
	    public int A, B;
    }

    public class AddRsp 
    {
        public int C;
    }

    public class jsonOut
    {
        public string t;

        public AddReq m;
    }

    class Program
    {
        static void Main(string[] args)
        {
            NetClient client = new NetClient();
            client.ConnectTo("127.0.0.1", 3200);

            Thread.Sleep(1000);
            for(int i=0;i<3;i++)
            {
                if (client.IsConnect)
	            {
                    AddReq req = new AddReq {A=i,B=i};
                    jsonOut js = new jsonOut { t = "main/"+req.GetType().Name, m = req };
                    string str = JsonConvert.SerializeObject(js);
                    Console.WriteLine("send:" + str);
                    client.WriteMessage(Encoding.UTF8.GetBytes(str));
	            }
                Thread.Sleep(1000);
            }
            Console.ReadKey();
        }
    }
}
