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
    class Program
    {
        static void Main(string[] args)
        {
            string srvCertCN = "service1";

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);
           
            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:10001/Service"),new X509CertificateEndpointIdentity(srvCert));


            using (WCFClient proxy = new WCFClient(binding, address))
            {
                proxy.TestCommunication();

                Console.WriteLine("TestCommunication() finished. Press <enter> to continue ...");

                while (true)
                {
                    Console.WriteLine("------ MENI ---------");
                    Console.WriteLine("1.Posalji zalbu ");
                    Console.WriteLine("2.Prikazi nevalidne zalbe");
                    Console.WriteLine("3.Zabrani rad korisniku");


                    string opcija = Console.ReadLine();

                    switch (opcija)
                    {
                        case "1":                          
                              Console.WriteLine("Napisite zalbu: ");
                              string zalba = Console.ReadLine();
                               
                               if (zalba == "")
                                  break;
                              else
                              {
                               
                              
                                proxy.SendMessage(/*"korisnik1"*/Formatter.ParseName(WindowsIdentity.GetCurrent().Name), zalba);
                              }

                            
                            break;

                        case "2":
                            Dictionary<string, List<string>> nevalidne = proxy.PrikaziZalbe();

                            foreach (var kp in nevalidne)
                            {
                                 foreach (string korisnik in nevalidne.Keys)
                                 {
                                    foreach (string nevalidnaZalba in nevalidne[korisnik])
                                    {
                                        Console.WriteLine("Korisnik koji je poslao zalbu: {0}", korisnik);
                                        Console.WriteLine("Nevalidna zalba: {0}", nevalidnaZalba);
                                    }
                                }
                            }
                         
                            break;

                        case "3":
                            proxy.BanUser();
                            Console.WriteLine("Korisniku je zabranjen pristup sistemu!");
                            break;

                    }
                    Console.ReadLine();
                }
              


                
               

            }

             



        }
    }
}
