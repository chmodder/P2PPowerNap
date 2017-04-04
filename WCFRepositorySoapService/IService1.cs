using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.UI.WebControls;

namespace WCFRepositorySoapService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        /// <summary>
        /// Counts all the entries in the index
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        int Count();

        /// <summary>
        /// Counts all different filenames in the index
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        int CountFilenames();

        /// <summary>
        /// Returns true if something was added to the index
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        [OperationContract]
        bool Add(string fileName, string hostName, int port);

        /// <summary>
        /// Gets a list of destinations where the file is located
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [OperationContract]
        List<Destination> Get(string fileName);

        /// <summary>
        /// Returns true if something was removed from the index
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        [OperationContract]
        bool Remove(string fileName, string host, int port);

        //TODO add extras: "int HowMany AddAll(List<filename>, host, port)" and "int HowMany RemoveAll(host, port)"

    }

    [DataContract]
    public class Destination
    {
        public Destination(string host, int port)
        {
            this.Host = host;
            this.Port = port;
        }

        [DataMember]
        public string Host { get; set; }

        [DataMember]
        public int Port { get; set; }
        
    }


}
