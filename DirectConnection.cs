using System;
using System.IO;
using System.Data.SqlClient;
using System.Threading;

class DirectConnection{
    private static DirectConnection instance;
    private static Object lockObject = new Object();
    public string connetionString;
    private FileStream file;
    private StreamWriter writer;
    public int counterExecuted,counterFail; public double prom;

    private DirectConnection(){
        connetionString = "Data Source=127.0.0.1;Initial Catalog=Caso2Granel;User ID=fab;Password=fabianceciliano;Pooling=false;Connect Timeout=60";
        file = new FileStream("C:\\Users\\DELL\\Desktop\\C#\\Direct.txt", FileMode.Create);
        writer = new StreamWriter(file);
    }

    public static DirectConnection getInstance(){
        if(instance!=null){
            return instance;
        }else{
            lock(lockObject){
                if(instance==null){
                    instance=new DirectConnection();
                }
                return instance;
            }
        }
    }

    public void query(string queryString){
        SqlConnection conection = new SqlConnection(connetionString);
        SqlDataReader dataReader;
        DateTime frs;

        try
        {
            frs = DateTime.Now;
            conection.Open();
                Console.WriteLine("Start "+Thread.CurrentThread.Name);
            SqlCommand command = new SqlCommand(queryString,conection);
            dataReader = command.ExecuteReader();//command.ExecuteNonQuery();
            while (dataReader.Read()){
                //Console.WriteLine("Reading");
            }
            dataReader.Close();
                Console.WriteLine("Ready "+Thread.CurrentThread.Name);
            conection.Close();

            saveStatistics(DateTime.Now.Subtract(frs)," -> Read from Db");

        }
        catch (System.Exception x)
        {
            safeError(Thread.CurrentThread.Name+": Error");
            Console.WriteLine(x);
        }

        Thread.Sleep(0); 
    }

    private void safeError(string value){
        lock(writer){
            writer.WriteLine(value);
            writer.Flush();
        }
    }

    private void saveStatistics(string value){
        lock(writer){
            writer.WriteLine(value);
            writer.Flush();
            counterFail++;
        }
    }

    private void saveStatistics(TimeSpan difference,string value){
        lock(writer){
            writer.WriteLine(Thread.CurrentThread.Name+": "+difference.ToString()+value);
            writer.Flush();
            counterExecuted++;
            prom+=difference.TotalSeconds;
        }
    }
    public void endFile(){
        writer.WriteLine("\nAmount Executed: "+counterExecuted+"\nAmount Fail: "+counterFail+"\nExecuted sum (seg): "+prom+"\nExecuted prom (seg): "+prom/counterExecuted);
        writer.WriteLine();
        writer.Close();
        file.Close();
    }

}