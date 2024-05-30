using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using RockMove.Pages;
using RockMove;

namespace RockMove
{
    public class Program
    {
        // Application starts here!!!
        public static void Main(string[] args) 
        {
            // Building the web application
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Adding RazorPages services to the application
            builder.Services.AddRazorPages();

            // Registering CustomerCatalog as a singleton service
            builder.Services.AddSingleton<CustomerCatalog>();

            // Registering EmployeeCredentialsManager as a Singleton service
            builder.Services.AddSingleton<EmployeeCredentialsManager>(provider =>
            {
                string filePath = "employee_credentials.txt"; // File path where Credentials are stored
                return new EmployeeCredentialsManager(filePath);
            });

            // Configuring authentication using cookies
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

            // Building the web application
            WebApplication app = builder.Build();

            // Configuring the HTTP request pipeline.
            if (!app.Environment.IsDevelopment()) // Checking if the application is not in development environment
            {
                app.UseExceptionHandler("/Error"); // Using the Error page for handling exceptions
                app.UseHsts(); // Enabling HTTP Strict Transport Security (HSTS)
            }

            app.UseHttpsRedirection(); // Enabling HTTPS redirection
            app.UseStaticFiles(); // Enabling serving static files

            app.UseRouting(); // Enabling routing

            app.UseAuthentication(); // Adding authentication middleware
            app.UseAuthorization(); // Adding authorization middleware

            app.MapRazorPages(); // Mapping Razor pages

            // Handling 404 Not Found errors by redirecting to the Index page
            app.Use(async (context, next) =>
            {
                await next(); // Calling the next middleware

                if (context.Response.StatusCode == StatusCodes.Status404NotFound) // Checking if the response status code is 404
                {
                    context.Response.Redirect("/Index"); // Redirecting to the Index page
                }
            });

            // Resolving CustomerCatalog service
            CustomerCatalog customerCatalog = app.Services.GetRequiredService<CustomerCatalog>();

            // Creating customers with a delay between 200 and 300 milliseconds
            CreateCustomers(customerCatalog);

            // Printing customer IDs from the CustomerCatalog
            foreach (long customerId in customerCatalog.GetCustomerIds())
            {
                Console.WriteLine("Customer ID: " + customerId);
            }

            // Starting a background task to remove expired customer IDs
            StartExpirationCheckTask(customerCatalog);

            app.Run(); // Running the application
        }

        // Simulating creating customers with a delay between 200 and 300 milliseconds
        private static void CreateCustomers(CustomerCatalog customerCatalog)
        {
            Random random = new Random();

            for (int i = 0; i < 8; i++) // Looping to create 8 customers
            {
                Customer customer = new Customer(); // Creating a new customer object
                customerCatalog.AddCustomerId(customer.Id); // Adding customer ID to the CustomerCatalog

                // Generating random delay between 200 and 300 milliseconds
                int delay = random.Next(200, 301);
                Thread.Sleep(delay); // Pausing execution for the specified delay
            }
        }

        // Starting a background task to periodically check and remove expired customer IDs
        private static void StartExpirationCheckTask(CustomerCatalog customerCatalog)
        {
            Timer timer = new Timer((state) =>
            {
                // Getting the current timestamp
                long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                // Getting the list of customer IDs
                List<long> customerIds = customerCatalog.GetCustomerIds();

                // Iterating through the customer IDs and removing expired ones
                foreach (long customerId in customerIds.ToArray()) // Using ToArray() to avoid collection modified exception
                {
                    // Checking if the customer ID is expired (added more than 3 minutes ago)
                    if (currentTime - customerId > 3 * 60 * 1000)
                    {
                        customerCatalog.RemoveCustomerId(customerId); // Removing expired customer ID from CustomerCatalog
                        Console.WriteLine("Removed expired customer ID: " + customerId); // Logging the removal of expired customer ID
                    }
                }
            }, null, TimeSpan.Zero, TimeSpan.FromMinutes(1)); // Running every minute
        }
    }
}