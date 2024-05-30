using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Security.Claims;

namespace RockMove.Pages
{

    public class EmployeeLoginModel : PageModel
    {
        // Private field to hold an instance of EmployeeCredentialsManager
        private readonly EmployeeCredentialsManager _credentialsManager;

        // Constructor to initialize EmployeeCredentialsManager instance
        public EmployeeLoginModel(EmployeeCredentialsManager credentialsManager)
        {
            _credentialsManager = credentialsManager;
        }

        // Properties to bind input values from login form
        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        // Property to indicate if the provided credentials are invalid
        [BindProperty]
        public bool InvalidCredentials { get; set; } = false;


        public IActionResult OnPost()
        {
            // Validate the employee's credentials using the EmployeeCredentialsManager class
            if (_credentialsManager.AreValidCredentials(Username, Password))
            {
                // Create claims for the authenticated employee
                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, Username),
                    new Claim(ClaimTypes.Role, "Employee") // We set the role of employee to "Employee"
                };

                // Create identity object
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Create authentication properties
                AuthenticationProperties authProperties = new AuthenticationProperties
                {
                    IsPersistent = true // Persist the authentication session across requests
                };

                // Sign in the employee
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                // Redirect to the employee dashboard upon successful login
                return RedirectToPage("/EmployeeDashboard");
            }
            else
            {
                // Set flag indicating invalid credentials for display in UI
                InvalidCredentials = true;
                return Page();
            }
        }
    }
}
