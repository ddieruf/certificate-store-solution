using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;

namespace WebApplication1.Controllers
{
    [Route("")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private CloudFoundryApplicationOptions AppOptions;
        private CloudFoundryServicesOptions ServiceOptions;
        private X509Certificate2Collection certificates;

        private const bool USE_CERT_STORE = false;

        public ValuesController(IOptions<CloudFoundryApplicationOptions> appOptions,
                            IOptions<CloudFoundryServicesOptions> serviceOptions)
        {
            AppOptions = appOptions.Value;
            ServiceOptions = serviceOptions.Value;

            try {
                certificates = new X509Certificate2Collection();
                certificates.Add(LoadCertificate());
            } catch (ArgumentNullException){
                using (X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser)) {
                    certStore.Open(OpenFlags.ReadOnly);
                    certificates = certStore.Certificates;
                    certStore.Close();
                }
            }
        }

        private X509Certificate2 LoadCertificate()
        {
            //Platform values
            string fileStr = ServiceOptions.Services["credhub"][0].Credentials["certificate"].Value;
            string password = ServiceOptions.Services["credhub"][0].Credentials["password"].Value;

            //Local testing
            /*string fileStr1 = Environment.GetEnvironmentVariable("PEM_1", EnvironmentVariableTarget.User);
            string fileStr2 = Environment.GetEnvironmentVariable("PEM_2", EnvironmentVariableTarget.User);
            string fileStr3 = Environment.GetEnvironmentVariable("PEM_3", EnvironmentVariableTarget.User);
            string password = Environment.GetEnvironmentVariable("password", EnvironmentVariableTarget.User);
            string fileStr = fileStr1 + fileStr2 + fileStr3;*/

            var bytes = Convert.FromBase64String(fileStr);

            if (bytes.Length == 0) {
                throw new Exception("Certificate not loaded");
            }

            X509Certificate2 xcert;
            try {
                xcert = new X509Certificate2(bytes, password, X509KeyStorageFlags.PersistKeySet);
            } catch (System.Security.Cryptography.CryptographicException ex) {
                //Did the Base64 string get cut off?
                throw new Exception("An error occurred converting to X509Certificate2 object. " + ex.Message);
            }

            if (USE_CERT_STORE) {
                using (X509Store certificateStore = new X509Store(StoreName.My, StoreLocation.CurrentUser)) {
                    certificateStore.Open(OpenFlags.ReadWrite);
                    certificateStore.Add(xcert);
                    certificateStore.Close();
                }
                return null;
            }
            
            return xcert;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<string> Get()
        {
            if (certificates.Count < 1)
                return BadRequest("No certificates found");
            
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://xxxxxx");
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
        /*
        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }*/
    }
}
