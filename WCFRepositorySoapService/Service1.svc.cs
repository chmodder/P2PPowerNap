using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WCFRepositorySoapService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        public const string ConnString = "Server=tcp:dalvang.database.windows.net,1433;Initial Catalog=DalvangDb;Persist Security Info=False;User ID=Dalvang;Password=)ktk)Pa79NXDlMapMK;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";


        /// <summary>
        /// Counts all the entries in the index
        /// </summary>
        /// <returns>Number of entries (Int32) in Repository</returns>
        public int Count()
        {
            string cmdText = @"SELECT COUNT(*) NumberOfEntries FROM [WCFRepositorySoapService].[Index]";

            int numberOfEntries = 0;

            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(cmdText, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    foreach (IDataRecord row in reader)
                    {
                        numberOfEntries = row.GetInt32(0);
                    }
                }
            }

            return numberOfEntries;
        }


        /// <summary>
        /// Counts all different filenames in the index
        /// </summary>
        /// <returns></returns>
        public int CountFilenames()
        {
            string cmdText = @"SELECT COUNT(DISTINCT(FileName)) NumberOfUniqueFileNames FROM [WCFRepositorySoapService].[Index]";

            int NumberOfUniqueFileNames = 0;

            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(cmdText, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    foreach (IDataRecord row in reader)
                    {
                        NumberOfUniqueFileNames = row.GetInt32(0);
                    }
                }
            }

            return NumberOfUniqueFileNames;
        }

        /// <summary>
        /// Returns true if something was added to the index
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool Add(string fileName, string hostName, int port)
        {
            string cmdText = @"INSERT INTO [WCFRepositorySoapService].[Endpoint] VALUES (@hostName, @port);";
            string cmdText2 = @"INSERT INTO [WCFRepositorySoapService].[Index] VALUES (@fileName, @hostName, @port);";
            int numberOfRowsAffected = 0;
            bool insertedIntoRepositorySuccessfully = false;

            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(cmdText, conn))
                {
                    cmd.Parameters.AddWithValue("fileName", fileName);
                    cmd.Parameters.AddWithValue("hostName", hostName);
                    cmd.Parameters.AddWithValue("port", port);

                    //Trying to insert into Endpoint table
                    try
                    {
                        //numberOfRowsAffected doesnt serve any purpose here except for storing the return value teporaryly until being set again when executing the next query below
                        numberOfRowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {

                    }

                    //Changing commandtext for inserting data into Index table
                    cmd.CommandText = cmdText2;

                    //Trying to insert into Index table
                    try
                    {
                        numberOfRowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {

                    }

                    if (numberOfRowsAffected > 0)
                    {
                        insertedIntoRepositorySuccessfully = true;
                    }
                }
            }
            return insertedIntoRepositorySuccessfully;
        }

        /// <summary>
        /// Gets a list of destinations where the file is located
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public List<Destination> Get(string fileName)
        {
            string cmdText = @"
                               SELECT Fk_Host, Fk_Port FROM [WCFRepositorySoapService].[Index]
                               WHERE FileName LIKE @fileName";

            List<Destination> fileLocations = new List<Destination>();

            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(cmdText, conn))
                {
                    cmd.Parameters.AddWithValue("fileName", fileName);

                    //Trying to get the data (Endpoint locations where filename is located) from Index table
                    try
                    {
                        SqlDataReader reader = cmd.ExecuteReader();

                        foreach (IDataRecord record in reader)
                        {
                            Destination d = new Destination(record.GetString(0),record.GetInt32(1));
                            fileLocations.Add(d);
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
            }

            return fileLocations;
        }

        /// <summary>
        /// Returns true if something was removed from the index
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool Remove(string fileName, string host, int port)
        {
            string cmdText = @"
                                DELETE FROM [WCFRepositorySoapService].[Index]
                                WHERE FileName LIKE @fileName 
                                        AND Fk_Host LIKE @host 
                                        AND Fk_Port = @port;";

            int numberOfRowsAffected = 0;
            bool deletedFromIndexTableSuccessfully = false;

            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(cmdText, conn))
                {
                    cmd.Parameters.AddWithValue("fileName", fileName);
                    cmd.Parameters.AddWithValue("host", host);
                    cmd.Parameters.AddWithValue("port", port);

                    //Trying to Delete records where fileName exists in Index table
                    try
                    {
                        numberOfRowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {

                    }

                    if (numberOfRowsAffected > 0)
                    {
                        deletedFromIndexTableSuccessfully = true;
                    }
                }
            }

            return deletedFromIndexTableSuccessfully;
        }
    }
}
