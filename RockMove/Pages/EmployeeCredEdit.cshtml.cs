using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace RockMove.Pages
{
    // Only accessible to logged-in admins
    [Authorize(Roles = "Admin", AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class EmployeeCredEditModel : PageModel
    {
        private readonly EmployeeCredentialsManager _credentialsManager;

        // Constructor injection to provide an instance of EmployeeCredentialsManager.
        public EmployeeCredEditModel(EmployeeCredentialsManager credentialsManager)
        {
            _credentialsManager = credentialsManager;
        }

        // Properties to bind input fields in the Razor Page.
        [BindProperty]
        public string NewUsername { get; set; }

        [BindProperty]
        public string NewPassword { get; set; }

        [BindProperty]
        public string OldUsername { get; set; }

        [BindProperty]
        public string OldPassword { get; set; }

        public Dictionary<string, string> EmployeeCredentials { get; private set; }

        public void OnGet()
        {
            // Fetch current employee credentials
            EmployeeCredentials = _credentialsManager.GetEmployeeCredentials();

            // Ensure EmployeeCredentials is initialized
            if (EmployeeCredentials == null)
            {
                EmployeeCredentials = new Dictionary<string, string>();
            }
        }

        public IActionResult OnPost()
        {
            // Validate inputs
            if (!string.IsNullOrEmpty(NewUsername) && !string.IsNullOrEmpty(NewPassword))
            {
                // Add new username-password pair to the dictionary
                _credentialsManager.AddCredentials(NewUsername, NewPassword);
            }

            // Refresh the page to display updated credentials
            return RedirectToPage();
        }

        public IActionResult OnPostUpdateCredentials()
        {
            // Validate old username and password
            if (_credentialsManager.AreValidCredentials(OldUsername, OldPassword))
            {
                // Update password for the specified username
                _credentialsManager.UpdateCredentials(OldUsername, OldPassword, NewPassword);
                // Refresh the page to display updated credentials
                return RedirectToPage();
            }
            else
            {
                // Incorrect credentials, display error message
                ViewData["UpdateCredentialsError"] = "Incorrect username or password.";
                return Page();
            }
        }

        public IActionResult OnPostDeleteEmployee()
        {
            // Retrieve provided delete username and password
            string deleteUsername = Request.Form["DeleteUsername"];
            string deletePassword = Request.Form["DeletePassword"];

            // Validate delete credentials
            if (_credentialsManager.AreValidCredentials(deleteUsername, deletePassword))
            {
                // Check if the employee exists
                if (_credentialsManager.GetEmployeeCredentials().ContainsKey(deleteUsername))
                {
                    // Remove employee from dictionary and file
                    _credentialsManager.RemoveCredentials(deleteUsername);
                    // Refresh the page to reflect changes
                    return RedirectToPage();
                }
                else
                {
                    // Employee not found, display error message
                    ViewData["DeleteCredentialsError"] = "Employee not found.";
                    return Page();
                }
            }
            else
            {
                // Incorrect credentials, display error message
                ViewData["DeleteCredentialsError"] = "Incorrect username or password.";
                return Page();
            }
        }
    }
}
