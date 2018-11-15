using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Selectors;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Manager
{
    public class ServiceCertValidator : X509CertificateValidator
    {
        public override void Validate(X509Certificate2 certificate)
        {
            List<string> korisnici = new List<string>();

            using (XmlReader reader = XmlReader.Create("banned_certs.xml"))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        if (reader.Name.Equals("Korisnik"))
                        {

                              korisnici.Add(reader.ReadElementContentAsString());
                        }
                    }
                }

            }

            EventLog eventLog = new EventLog("Application", Environment.MachineName, "TestApplication");

            if (!EventLog.SourceExists("TestApplication"))
            {
                EventLog.CreateEventSource("TestApplication", "Application");
                Console.WriteLine("CreatedEventSource");

            }

            eventLog.Source = "TestApplication";
            int eventID = 7;

            foreach (string korisnik in korisnici)
            {

                if ((certificate.SubjectName.Name.StartsWith(korisnik))|| certificate.IssuerName.Name.Equals(korisnik))
                {

                    eventLog.WriteEntry("User " +korisnik.Split('=')[1]+ " is unsuccessfully authenticated.", EventLogEntryType.Information, eventID);
                    eventLog.Close();

                    throw new Exception("Certificate subject name or issuer is banned.");
                }               

            }

            eventLog.WriteEntry("User " + certificate.SubjectName.Name.Split('=',',')[1] + " is successfully authenticated.", EventLogEntryType.Information, eventID);
            eventLog.Close();




        }
    }
}
