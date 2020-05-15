using System;
using System.IO;
using System.Data.SqlClient;
using System.Threading;
using System.Runtime.Caching;


class DbQuery{
    private static DbQuery instance;
    private string connetionString;
    private ObjectCache cache = MemoryCache.Default;
    private static Object lockObject = new Object();
    private FileStream file;
    private StreamWriter writer;
    private String type; public int counterExecuted,counterFail; public double prom;

    private DbQuery(string type){
        
        this.type=type;

        if(type=="Pool"){
            connetionString = "Data Source=127.0.0.1;Initial Catalog=Caso2Granel;User ID=fab;Password=fabianceciliano;Min Pool Size=1;Max Pool Size=3;Connect Timeout=60";
        }else{
            connetionString = "Data Source=127.0.0.1;Initial Catalog=Caso2Granel;User ID=fab;Password=fabianceciliano;Pooling=false;Connect Timeout=60";
        }
        
        this.file = new FileStream("C:\\Users\\DELL\\Desktop\\C#\\"+type+".txt", FileMode.Create);
        this.writer = new StreamWriter(file);
    }

    public static DbQuery getInstance(string type){
        if(instance!=null){
            return instance;
        }else{
            lock(lockObject){
                if(instance==null){
                    instance=new DbQuery(type);
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
            safeError(Thread.CurrentThread.Name+": "+"Error");
            Console.WriteLine(x);
        }

        Thread.Sleep(0);
    }

    public void queryCaching(string queryString){
        DateTime frs;
        
        frs = DateTime.Now;
        if(cache[queryString] == null){
            
            lock(lockObject){
                frs = DateTime.Now;
                if(cache[queryString] == null){

                    queryToDb(queryString);
                    
                }else
                {
                    Console.WriteLine("Read from cache "+Thread.CurrentThread.Name);
                    saveStatistics(DateTime.Now.Subtract(frs)," -> Read from cache");
                }
                
            }
            
        }else
        {
            Console.WriteLine("Read from cache "+Thread.CurrentThread.Name);
            saveStatistics(DateTime.Now.Subtract(frs)," -> Read from cache");
        }
    }

    public void query(string queryString){
        if(type=="Cache"){
            this.queryCaching(queryString);
        }else{
            queryToDb(queryString);            
        }
    }

    private void safeError(string value){
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