using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
    [ServiceContract]
    public interface IWCFContract
    {
        [OperationContract]
        void TestCommunication();

        [OperationContract]
        void SendMessage(string korisnik,string message);

        [OperationContract]
        Dictionary<string,List<string>> PrikaziZalbe();

        [OperationContract]
        void BanUser();
            
   }
}
