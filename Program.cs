using System.Threading;
using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Connection
{
    class Program
    {
        
        /*public void runTask(){
            int amount = 10;
            Task [] threadArray = new Task [amount];

            for (int i = 0; i < amount; i++){
                threadArray[i]=Task.Factory.StartNew(()=>DbQuery.getInstance("Pool").query("waitfor delay '00:00:04'"));
            }

            Task.WaitAll(threadArray);

            Console.WriteLine("Hola");
        }*/

        static void Main(string[] args)
        {
            int amount = 28;
            string type = "Cache";//Cache//Direct//Pool
            Thread [] threadArray = new Thread [amount];

            for (int i = 0; i < amount; i++){
                if(type=="Cache"){
                    //En este caso se usa siempre el mismo query para que el programa haga cache la primer vez 
                    //que se busca en la bd y las siguientes no requiera ir de nuevo a la base de datos
                    threadArray[i]= new Thread(()=>DbQuery.getInstance(type).query("select * from Users"));
                }else{
                    //se genera un query con rangos, con la ayuda de la variable del ciclo, para que asi 
                    //siempre se ejecute un query diferente y sql no haga cache internamente
                    string queryStr = "select * from movements where movementsid >= "+((i+1)*30000-29999)+" and movementsid < "+((i+1)*30000);
                    threadArray[i]= new Thread(()=>DbQuery.getInstance(type).query(queryStr));
                }
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