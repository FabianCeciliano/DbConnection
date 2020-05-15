using System;
using System.Data.SqlClient;
class example{

    public string connetionString = "Data Source=127.0.0.1;Initial Catalog=Caso2Granel;User ID=fab;Password=fabianceciliano;Min Pool Size=3;Max Pool Size=3;Connect Timeout=60";
    public example(){

    }

    public void trash(){
        /*string connetionString = null;
            SqlConnection cnn ;
			connetionString = "Data Source=127.0.0.1;Initial Catalog=Caso2Granel;User ID=fab;Password=fabianceciliano;Min Pool Size=2;Max Pool Size=2";
            cnn = new SqlConnection(connetionString);
            try
            {
                
                /*cnn.Open();
                Console.WriteLine("Listo");
                SqlCommand command = new SqlCommand("select top 1 * from city",cnn);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    Console.WriteLine(dataReader.GetValue(2));
                }
                dataReader.Close();
                command.Dispose();
                cnn.Close();*/

                /*
                cnn.Open();
                int x=1;
                
                while(x<5){
                    SqlCommand command = new SqlCommand("waitfor delay '00:00:04'",cnn);
                    Console.WriteLine("query # "+x);
                    command.ExecuteNonQuery();
                    //SqlDataReader dataReader = command.ExecuteReader();
                    //dataReader.Close();
                    command.DisposeAsync();
                    x++;
                }
                cnn.Close();*/

            /*}
            catch (System.Exception x)
            {
                Console.WriteLine(x);
            }*/
    }

}