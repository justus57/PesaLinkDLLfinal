using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace PesaLinkDLL
{
    public class Library
    {
        public Library()
        {
        }

        static string certPath = "bank0089_transport.cert.pfx";
      
        static X509Certificate2 cert = new X509Certificate2(File.ReadAllBytes(certPath), ""); //path to certificate

        public static string AccountVerificationPesalinK(string BankCode, string Accountno)
        {
            string ipsresponse = null;

            try
            {
              
                //create JSON

                var PrintCommand = new AccountVerificationRequest
                {
                    AccountNo = Accountno,
                    BackCode=BankCode
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                string jsonData = js.Serialize(PrintCommand);

                X509CertificateCollection clientCerts = new X509CertificateCollection();
                clientCerts.Add(cert);

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                var client = new RestClient("https://0089.ips.pesalink.co.ke:4431/sync/v1/verification");
                client.Timeout = -1;
                client.ClientCertificates = clientCerts;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", jsonData, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                ipsresponse = response.Content;
                // Console.WriteLine(response.Content);
            }
            catch (Exception es)
            {
                WriteLog(es.Message);
            }            
            return ipsresponse;
        }

        private static void WriteLog(string text)
        {
           
                try
                {
                    //set up a filestream
                    string strPath = @"C:\Logs\PesalinkClasslibrary";
                    string fileName = DateTime.Now.ToString("MMddyyyy") + "_logs.txt";
                    string filenamePath = strPath + '\\' + fileName;
                    Directory.CreateDirectory(strPath);
                    FileStream fs = new FileStream(filenamePath, FileMode.OpenOrCreate, FileAccess.Write);
                    //set up a streamwriter for adding text
                    StreamWriter sw = new StreamWriter(fs);
                    //find the end of the underlying filestream
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    //add the text
                    sw.WriteLine(DateTime.Now.ToString() + " : " + text);
                    //add the text to the underlying filestream
                    sw.Flush();
                    //close the writer
                    sw.Close();
                }
                catch (Exception ex)
                {
                    //throw;
                    ex.Data.Clear();
                }
        }
        
    }

    internal class AccountVerificationRequest
    {
        public string BackCode { get; set; }
        public string AccountNo { get; set; }
    }
    internal class VerificationResponse
    {
        public string Backcode { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string message { get; internal set; }
        public bool status { get; set; }
    }

   
}
