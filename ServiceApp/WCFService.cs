using Manager;
using ServiceContracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ServiceApp
{
    public class WCFService : IWCFContract
    {
        public static Dictionary<string, List<String>> zalbe = new Dictionary<string, List<String>>();
        public static Dictionary<string, List<String>> NevalidneZalbe = new Dictionary<string, List<String>>();
        public static List<string> nedozvoljeneReci = new List<string>();



        public void SendMessage(string korisnik,string message)
        {

            string cert = OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name;
            string ou = cert.Split(';')[0].Split('=')[2];

            if (ou == "Korisnik")
            {
               
                if (zalbe.ContainsKey(korisnik))
                {
                    zalbe[korisnik].Add(message);
                }

                zalbe[korisnik] = new List<string>();
                zalbe[korisnik].Add(message);


                //foreach (var kp in zalbe)
                //{
                //    foreach (string k in zalbe.Keys)
                //    {
                //        foreach (string z in zalbe[k])
                //        {

                //            Console.WriteLine("Zalba:{0}", message);
                //        }
                //    }
                //}

            }
            else
            {
                throw new SecurityException("Nemate pravo pristupa metodi SendMessage()!");
            }

        }

       
        public Dictionary<string,List<string>> PrikaziZalbe()
        {
            string cert = OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name;
            string ou = cert.Split(';')[0].Split('=')[2];

            if(ou == "Nadzor")
            {
                //zalbe.Add("korisnik1", "mrs majmuni");
                StreamReader sr = new StreamReader("nedozvoljeneReci.txt");
                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    nedozvoljeneReci.Add(s);
                }
                //popraviti!!!!!
                foreach (var kp in zalbe)
                {
                    foreach (string kor in zalbe.Keys)
                    {
                        foreach (string zalba in zalbe[kor])
                        {
                            foreach (string nedozvoljenaRec in nedozvoljeneReci)
                            {
                                if (zalba.Contains(nedozvoljenaRec))
                                {
                                    if (NevalidneZalbe.ContainsKey(kor))
                                    {
                                        NevalidneZalbe[kor].Add(zalba);
                                    }

                                    NevalidneZalbe[kor] = new List<string>();
                                    NevalidneZalbe[kor].Add(zalba);

                                }
                            }
                        }
                    }

                }
              

                return NevalidneZalbe;

            }
            else
            {
                throw new SecurityException("Nemate pravo pristupa metodi PrikaziZalbe()!");

            }

           
        }

        
        public void TestCommunication()
        {
            Console.WriteLine("Communication established.");
        }

        public void BanUser()
        {
            string cert=OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name;
            string ou= cert.Split(';')[0].Split('=')[2];

            if (ou == "Nadzor")
            {
                foreach (var kp in NevalidneZalbe)
                {
                    //if (nevalidnaZalba.Contains("mrs") || nevalidnaZalba.Contains("govna") || nevalidnaZalba.Contains("kurve") || nevalidnaZalba.Contains("olosi"))
                    //{
                        XDocument xDocument = XDocument.Load("banned_certs.xml");
                        XElement korisnici = xDocument.Element("Korisnici");
                        IEnumerable<XElement> rows = korisnici.Descendants("Korisnik");
                        XElement firstRow = rows.First();
                        firstRow.AddBeforeSelf(new XElement("Korisnik", "CN=", NevalidneZalbe.Keys));
                        xDocument.Save("banned_certs.xml");
                     //}

                }

            }
            else
            {
                throw new SecurityException("Nemate pravo pristupa metodi BanUser()!");
            }
           

        }
    }
}
 
