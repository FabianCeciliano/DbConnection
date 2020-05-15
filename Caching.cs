using System;
using System.IO;
using System.Data.SqlClient;
using System.Threading;
using System.Runtime.Caching;

class Caching{
    private static Caching instance;
    private string connetionString;
    private ObjectCache cache = MemoryCache.Default;
    private static Object lockObject = new Object();
    private FileStream file;
    private StreamWriter writer;
    public int counterExecuted,counterFail; public double prom;
    private Caching(){
        connetionString = "Data Source=127.0.0.1;Initial Catalog=Caso2Granel;User ID=fab;Password=fabianceciliano;Pooling=false;Connect Timeout=60";
        this.file = new FileStream("C:\\Users\\DELL\\Desktop\\C#\\Cache.txt", FileMode.Create);
        writer = new StreamWriter(file);
    }

    public static Caching getInstance(){
        if(instance!=null){
            return instance;
        }else{
            lock(lockObject){
                if(instance==null){
                    instance=new Caching();
                }
                return instance;
            }
        }
    }

    private void queryToDb(string queryString){
        DateTime frs = DateTime.Now;
        SqlConnection conection = new SqlConnection(connetionString);
        try
        {
            conection.Open();
                Console.WriteLine("Start "+Thread.CurrentThread.Name);
            SqlCommand command = new SqlCommand(queryString,conection);
            SqlDataReader dataReader = command.ExecuteReader();//command.ExecuteNonQuery();
            while (dataReader.Read()){
                //Console.WriteLine("Reading");
            }
            cache.Set(queryString,dataReader.ToString(),null);
            dataReader.Close();
                Console.WriteLine("Ready "+Thread.CurrentThread.Name);
            conection.Close();
            saveStatistics(DateTime.Now.Subtract(frs)," -> Read from Db");
        }
        catch (System.Exception x)
        {
            this.safeError(Thread.CurrentThread.Name+": "+"Error");
            Console.WriteLine(x);
        }

        Thread.Sleep(0);
    }

    public void query(string queryString){
        DateTime frs;
        
        frs = DateTime.Now;
        if(cache[queryString] == null){
            
            lock(lockObject){
                frs = DateTime.Now;
                if(cache[queryString] == null){

                    queryToDb(queryString);
                    
                }else
                {
                    saveStatistics(DateTime.Now.Subtract(frs)," -> Read from cache");
                    Console.WriteLine("Read from cache "+Thread.CurrentThread.Name);
                }
                
            }
            
        }else
        {
            saveStatistics(DateTime.Now.Subtract(frs)," -> Read from cache");
            Console.WriteLine("Read from cache "+Thread.CurrentThread.Name);
        }
         
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