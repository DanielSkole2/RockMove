using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;



namespace RockMove
{

    public class CustomerCatalog
    {
        // Declare a private field to store customer IDs as a list of long
        private List<long> customerIds;

        // Constructor for CustomerCatalog class
        public CustomerCatalog()
        {
            // Initialize the list of customer IDs
            customerIds = new List<long>();
        }

        // Method to add a customer ID to the catalog
        public void AddCustomerId(long customerId)
        {
            // Add the provided customer ID to the list
            customerIds.Add(customerId);
        }

        // Method to remove a customer ID from the catalog
        public void RemoveCustomerId(long customerId)
        {
            // Remove the provided customer ID from the list
            customerIds.Remove(customerId);
        }

        // Method to retrieve all customer IDs from the catalog
        public List<long> GetCustomerIds()
        {
            // Return the list of customer IDs
            return customerIds;
        }
    }
}
