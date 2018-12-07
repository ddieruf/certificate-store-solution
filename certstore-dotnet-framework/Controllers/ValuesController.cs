using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.IO;

namespace WebApplication2.Controllers
{
    public class ValuesController : ApiController
    {
        private X509Certificate2Collection certificates;
        //private const bool USE_CERT_STORE = false;

        public ValuesController()
        {
            using (X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser)) {
                certStore.Open(OpenFlags.ReadOnly);
                certificates = certStore.Certificates;
                certStore.Close();
            }
        }
        // GET api/
        public string Get()
        {
            if (certificates.Count < 1)
                throw new Exception("No certificates found");

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://XXXXXXXX");
            request.Method = "GET";
            request.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true; //Because of self-signed certificate
            request.ClientCertificates.AddRange(certificates);

            string ret = "";
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                using (StreamReader sr = new StreamReader(response.GetResponseStream())) {
                    ret = sr.ReadToEnd();
                }
            }

            return ret;
        }

        // GET api/values/5
        /*public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }*/
    }
}
