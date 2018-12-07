using System;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace WcfService1
{
    public class Customer
	{
		public int? CustomerId {get;set;}
		public bool IsActive { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string CompanyName { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
	}
    public class CustomerService : ICustomerService
    {
        private Customer demoCustomer {
            get {
                return new Customer() {
                    CustomerId = 1,
                    CompanyName = "",
                    Phone = "123-123-1234",
                    Email = "no@email.com",
                    FirstName = "MyFirst",
                    IsActive = true,
                    LastName = "MyLast"
                };
            }
        }

        public Customer Get()
        {
            return demoCustomer;
        }
        public Customer Insert(Customer newCustomer)
        {
            return newCustomer;
        }
    }

    [ServiceContract]
    public interface ICustomerService
    {
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        Customer Get();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        Customer Insert(Customer customer);
    }
}