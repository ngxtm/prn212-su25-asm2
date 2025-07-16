using DAL.Entities;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class CustomerRepositories
    {
        private readonly FuminiHotelManagementContext _db;

        public CustomerRepositories()
        {
            this._db = new FuminiHotelManagementContext();
        }

        public void AddCustomer(Customer customer)
        {
            _db.Customers.Add(customer);
            _db.SaveChanges();
        }

        public void DeleteCustomer(int id)
        {
            var customer = _db.Customers.Where(c => c.CustomerId == id);
            if (customer.Any())
            {
                _db.Customers.Remove(customer.First());
                _db.SaveChanges();
            }
        }

        public void UpdateCustomer(Customer customer)
        {
            _db.Customers.Update(customer);
            _db.SaveChanges();
        }

        public List<Customer> GetAllCustomers()
        {
            try
            {
                return _db.Customers
                    .Where(c => c.CustomerFullName != null && c.Telephone != null)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllCustomers: {ex.Message}");
                return new List<Customer>();
            }
        }

        public List<Customer> SearchCustomer(string keyword)
        {
            try
            {
                if (string.IsNullOrEmpty(keyword)) return _db.Customers.ToList();

                return _db.Customers
                    .Where(c => 
                        (c.CustomerFullName != null && c.CustomerFullName.Contains(keyword)) ||
                        (c.Telephone != null && c.Telephone.Contains(keyword)) ||
                        (c.EmailAddress != null && c.EmailAddress.Contains(keyword)))
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SearchCustomer: {ex.Message}");
                return new List<Customer>();
            }
        }

        public Customer? GetCustomerByEmailAndPassword(string email, string password)
        {
            return _db.Customers.FirstOrDefault(c => c.EmailAddress == email && c.Password == password);
        }
    }
}
