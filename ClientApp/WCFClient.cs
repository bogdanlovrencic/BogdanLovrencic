using Manager;
using ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
    public class WCFClient : ChannelFactory<IWCFContract>, IWCFContract, IDisposable
    {
        IWCFContract factory;
        public WCFClient(NetTcpBinding binding, EndpointAddress address): base(binding,address)
        {
            string cltCertCN = /*"korisnik1";*/ Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom;
            this.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new ClientCertValidator();
            this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);

            factory = this.CreateChannel();

            Console.WriteLine(cltCertCN);

        }
        public void TestCommunication()
        {
            try
            {
                factory.TestCommunication();
            }
            catch (SecurityException e)
            {
                Console.WriteLine("[TestCommunication] ERROR = {0}", e.Message);
            }
        }
        public void SendMessage(string korisnik,string message)
        {
            try
            {
                factory.SendMessage(korisnik,message);
            }
            catch (SecurityException e)
            {
                Console.WriteLine("Error:{0}", e.Message);
            }
        }

        public Dictionary<string,List<string>> PrikaziZalbe()
        {
            Dictionary<string, List<string>> zalbe=null;

            try
            {
               zalbe= factory.PrikaziZalbe();
            }
            catch(SecurityException e)
            {
                Console.WriteLine("Error:{0}", e.Message);
            }

            return zalbe;
        }

        public void BanUser()
        {
            try
            {
                factory.BanUser();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
        }

      

        public void Dispose()
        {
            if (factory != null)
            {
                factory = null;
            }
           
            this.Close();
          
        }

       
    }
}
