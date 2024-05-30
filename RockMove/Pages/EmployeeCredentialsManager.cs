using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace RockMove.Pages
{
    public class EmployeeCredentialsManager
    {
        private readonly string _filePath; // Declaring a private readonly variable to store the file path
        private Dictionary<string, string> _employeeCredentials; // Declaring a private variable to store employee credentials
        private readonly object _lock = new object(); // Declaring a lock object for thread safety

        public EmployeeCredentialsManager(string filePath) // Constructor for EmployeeCredentialsManager, takes a file path as input
        {
            _filePath = filePath; // Assigning the input file path to the private variable
            _employeeCredentials = LoadCredentialsFromFile(); // Loading credentials from file and assigning them to the private variable
        }

        private Dictionary<string, string> LoadCredentialsFromFile() // Method to load credentials from a file
        {
            if (File.Exists(_filePath)) // Checking if the file exists
            {
                return File.ReadAllLines(_filePath) // Reading all lines from the file
                           .Select(line => line.Split(',')) // Splitting each line by comma
                           .ToDictionary(parts => parts[0], parts => parts[1]); // Converting the split parts into a dictionary
            }
            else
            {
                return new Dictionary<string, string>(); // Returning an empty dictionary
            }
        }

        // Method to validate user credentials
        public bool AreValidCredentials(string username, string password)
        {
            lock (_lock) // Using a lock for thread safety
            {
                if (_employeeCredentials.TryGetValue(username, out string storedPassword)) // Checking if the username exists in credentials
                {
                    return storedPassword == password; // Returning true if the passwords match, false otherwise
                }
                return false; // Returning false if username doesn't exist
            }
        }

        public void AddCredentials(string username, string password) // Method to add new credentials
        {
            lock (_lock) // Using a lock for thread safety
            {
                _employeeCredentials[username] = password; // Adding the new username and password to the credentials
                SaveCredentialsToFile(); // Saving the updated credentials to the file
            }
        }

        public void UpdateCredentials(string username, string oldPassword, string newPassword) // Method to update existing credentials
        {
            lock (_lock) // Using a lock for thread safety
            {
                if (_employeeCredentials.TryGetValue(username, out string storedPassword) && storedPassword == oldPassword) // Checking if the username and old password match existing credentials
                {
                    _employeeCredentials[username] = newPassword; // Updating the password
                    SaveCredentialsToFile(); // Saving the updated credentials to the file
                }
            }
        }

        public void RemoveCredentials(string username) // Method to remove credentials
        {
            lock (_lock) // Using a lock for thread safety
            {
                if (_employeeCredentials.ContainsKey(username)) // Checking if the username exists in credentials
                {
                    _employeeCredentials.Remove(username); // Removing the username from credentials
                    SaveCredentialsToFile(); // Saving the updated credentials to the file
                }
            }
        }

        private void SaveCredentialsToFile() // Method to save credentials to a file
        {
            lock (_lock) // Using a lock for thread safety
            {
                File.WriteAllLines(_filePath, _employeeCredentials.Select(kv => $"{kv.Key},{kv.Value}")); // Writing credentials to the file
            }
        }

        public Dictionary<string, string> GetEmployeeCredentials() // Method to get all employee credentials
        {
            lock (_lock) // Using a lock for thread safety
            {
                return _employeeCredentials; // Returning the credentials
            }
        }
    }


}
