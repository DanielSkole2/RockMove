using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RockMove;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RockMove.Pages
{

    public class CustomerLoginModel : PageModel
    {
        // Define properties to bind customer ID and register invalid credentials
        [BindProperty]
        public long CustomerId { get; set; }

        [BindProperty]
        public bool InvalidCredentials { get; set; } = false;

        // Store reference to customer catalog for retrieving customer IDs
        private readonly CustomerCatalog _customerCatalog;

        // Constructor to initialize CustomerLoginModel with a CustomerCatalog instance
        public CustomerLoginModel(CustomerCatalog customerCatalog)
        {
            _customerCatalog = customerCatalog;
        }

        // Asynchronous method to handle HTTP POST requests for customer login
        public async Task<IActionResult> OnPostAsync()
        {
            // Validate the customer's credentials by checking if the provided CustomerId exists in the catalog
            if (_customerCatalog.GetCustomerIds().Contains(CustomerId))
            {
                // If credentials are valid, create claims for the authenticated customer
                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, CustomerId.ToString()), // Claim for customer's name
                    new Claim(ClaimTypes.Role, "Customer") // Here we assume the role is "Customer"
                };

                // Create a claims identity object using the claims and authentication scheme
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Create authentication properties, specifying that it's not persistent and expires in 30 seconds
                AuthenticationProperties authProperties = new AuthenticationProperties
                {
                    IsPersistent = false, // Session-based authentication
                    ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(30) // Expires in 30 seconds
                };

                // Sign in the customer using the authentication scheme, claims principal, and authentication properties
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                // Redirect the authenticated customer to a protected page for accessing audio files
                return RedirectToPage("/AudioFiles");
            }
            else
            {
                // If credentials are invalid, set the register for invalid credentials and return the current page
                InvalidCredentials = true;
                return Page();
            }
        }
    }
}
