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
            string cmdText = @"SELECT COUNT(*) NumberOfEntries FROM [WCFRepositorySoapService].[Repository]";

            int numberOfEntries = 0;

            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(cmdText,conn))
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
            //TODO make commandtext
            string cmdText = @"SELECT COUNT(DISTINCT(Fk_FileName)) NumberOfUniqueFileNames FROM [WCFRepositorySoapService].[Repository]";

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
        public bool Add(string fileName, string hostName, int port)
        {
            throw new NotImplementedException();
        }

        public List<Destination> Get(string fileName)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string fileName, string host, int port)
        {
            throw new NotImplementedException();
        }
    }
}
