using System.Threading;
using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Connection
{
    class Program
    {
        
        public void runTask(){
            int amount = 10;
            Task [] threadArray = new Task [amount];

            for (int i = 0; i < amount; i++){
                threadArray[i]=Task.Factory.StartNew(()=>DbQuery.getInstance("Pool").query("waitfor delay '00:00:04'"));
            }

            Task.WaitAll(threadArray);

            Console.WriteLine("Hola");
        }

        static void Main(string[] args)
        {
            int amount = 11;
            string type = "Pool";
            Thread [] threadArray = new Thread [amount];

            for (int i = 0; i < amount; i++){
                threadArray[i]= new Thread(()=>DbQuery.getInstance(type).query("waitfor delay '00:00:04'"));//Cache-Pool-Direct
                threadArray[i].Name="Thread# "+i;
            }

            for (int i = 0; i < amount; i++){
                threadArray[i].Start();
            }

            foreach (Thread thread  in threadArray)
            {
                thread.Join();
            }

            DbQuery.getInstance(type).endFile();

        }
    }
}