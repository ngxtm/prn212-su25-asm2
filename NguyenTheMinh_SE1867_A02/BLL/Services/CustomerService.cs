using DAL.Entities;
using DAL.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class CustomerService
    {
        private readonly CustomerRepositories _customerRepositories;    
        public CustomerService()
        {
            this._customerRepositories = new CustomerRepositories();
        }

        public List<Customer> GetAllCustomers()
        {
            return _customerRepositories.GetAllCustomers();
        }

        public List<Customer> SearchCustomer(string keyword)
        {
            return _customerRepositories.SearchCustomer(keyword);
        }

        public void AddCustomer(Customer customer)
        {
            _customerRepositories.AddCustomer(customer);
        }

        public void DeleteCustomer(int id)
        {
            _customerRepositories.DeleteCustomer(id);
        }
    }
}
