using DataValidationLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSanitizationConsole
{

    class Program
    {

        static void Main(string[] args)
        {
            var customer = new Customer() { Name = "Ramsh", City = "-Banglaore", Address="Electronic city Bangalore", Email = "test@test.com", Orders = new string[] { "11", "12" } , FaxNumber="123-111-2123"};
            var customer2 = new Customer() { Name = "Ramsh", City = "Banglaore", Address = "Electronic city Bangalore", Email = "test@test.com", Orders = new string[] { "11", "12" }, FaxNumber = "123-111-2123" };
            var customer3 = new Customer() { Name = "Ramsh", City = "Banglaore", Address = "Electronic city Bangalore", Email = "test@test.com", Orders = new string[] { "11", "12" }, FaxNumber = "123-111-2123" };
            
            var order = new Order() { OrderId = 1, OrderRemarks = "testorder", Customers= new Customer[] { customer,customer2 }, Customer=customer3 };             
            //string customer = null;
            //var regex = @"^[a-zA-Z ]+(&)?[a-zA-Z ]+(&')?[a-zA-Z ]+$";
            var dataVal = new DataValidation();

            var result = dataVal.ValidateRequest<Order>(order);
            
            Console.Write(result);
            
            Console.Read();
        }
    }

    public class Customer
    {
        [Validation(true)]
        public string Name { get; set; }
         
        public string City { get; set; }
         [Validation(false,AllowedCharacters=" ", AllowedCharactersMultiplicity=Multiplicity.OneOrMore)]
        public string Address { get; set; }

        [Validation(false,AllowedCharacters="@",AllowedCharactersMultiplicity=Multiplicity.ZeroOrOnce)]
        public string Email { get; set; }
        [Validation(true)]
        public IList<string> Orders { get; set; }
        [Validation(false, RegexString = @"^\d{3}-\d{3}-\d{4}$")]
        public string FaxNumber { get; set; }

        
    }
    [ValidationAttribute(true,ValidateAssociatedObject=true)]
    public class Order
    {
        public int OrderId{get;set;}

        [Validation(true)]
        public string OrderRemarks { get; set; }

         [Validation(true)]
        public IList<Customer> Customers { get; set; }

        [Validation(true)]
         public Customer Customer { get; set; }
    
    }
}
