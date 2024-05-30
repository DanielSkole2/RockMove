using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RockMove.Pages;
using System.Collections.Generic;
using System.Security.Claims;

namespace RockMove.Pages
{
    public class LoginModel : PageModel
    {
        public LoginModel() // Constructor for the LoginModel class
        {
            Administrator = new Administrator("admin", "password"); // Creating a new Administrator object with default credentials
        }

        [BindProperty]
        public bool IsLoggedIn { get; set; } = false; // Property to track if the user is logged in, bound to the page

        [BindProperty]
        public Administrator Administrator { get; } = new Administrator("admin", "password"); // Property representing the administrator, bound to the page

        [BindProperty]
        public bool InvalidCredentials { get; set; } = false; // Property to track if the login credentials are invalid, bound to the page

        public IActionResult OnPost(string username, string password) // Method that handles POST requests when the login form is submitted
        {
            // Checking if the entered username and password match the administrator's credentials
            if (username == Administrator.Username && password == Administrator.Password)
            {
                // Create claims for the authenticated user 
                List<Claim> claims = new List<Claim> // Creating a list to hold claims for the authenticated user
                {
                    new Claim(ClaimTypes.Name, username), // Adding a claim for the username
                    new Claim(ClaimTypes.Role, "Admin") // We set the admin role to "Admin"
                };

                // Create identity object
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Create authentication properties
                AuthenticationProperties authProperties = new AuthenticationProperties // Creating properties for authentication
                {
                    IsPersistent = true // Setting the authentication session to persist across requests
                };

                // Signing in the user using cookie-based authentication
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);


                IsLoggedIn = true; // Setting the IsLoggedIn property to true since the user is now logged in

                // Redirect to the admin dashboard upon successful login
                return RedirectToPage("/AdminDashboard");
            }
            else
            {
                InvalidCredentials = true; // Setting InvalidCredentials to true since the entered credentials are invalid
                return Page(); // Returning the current page, which will display an error message
            }
        }
    }
}