using System;
using System.Web.Routing;
using System.ServiceModel.Activation;

namespace WcfService1
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes();
        }
        private void RegisterRoutes()
        {
            WebServiceHostFactory factory = new WebServiceHostFactory();
            RouteTable.Routes.Add(new ServiceRoute("", factory, typeof(CustomerService)));
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}