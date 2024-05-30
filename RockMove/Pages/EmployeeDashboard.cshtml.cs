using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RockMove.Pages // Defining namespace for the page
{
    // This class represents the Employee Dashboard page, accessible only to logged-in employees
    [Authorize(Roles = "Employee", AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class EmployeeDashboardModel : PageModel
    {
        // Instance of IWebHostEnvironment for hosting environment operations
        private readonly IWebHostEnvironment _webHostEnvironment;

        // List to store audio file names
        public List<string> AudioFiles { get; private set; }

        // Dictionary to store audio file descriptions
        public Dictionary<string, string> AudioDescriptions { get; private set; }

        // Constructor to initialize EmployeeDashboardModel with IWebHostEnvironment instance
        public EmployeeDashboardModel(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        // Method to initialize data on page load
        public void OnGet()
        {
            RefreshAudioFilesList(); // Refreshing list of audio files
            LoadAudioDescriptions(); // Loading audio file descriptions
        }

        // Method to handle audio file upload
        public async Task<IActionResult> OnPostAsync(IFormFile audioFile)
        {
            if (audioFile == null || audioFile.Length == 0)
            {
                return Page(); // Returning the page if no file is uploaded
            }

            // Setting up file paths
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "audio");
            string fileName = RemoveGuidPrefix(Path.GetFileName(audioFile.FileName));
            string filePath = Path.Combine(uploadsFolder, fileName);

            // Copying the uploaded file to the server
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await audioFile.CopyToAsync(fileStream);
            }

            RefreshAudioFilesList(); // Refreshing list of audio files
            LoadAudioDescriptions(); // Reloading audio file descriptions after upload

            return RedirectToPage("/EmployeeDashboard");
        }

        // Method to handle audio file deletion
        public IActionResult OnPostDelete(string fileName)
        {
            // Setting up file paths
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "audio");
            string filePath = Path.Combine(uploadsFolder, fileName);

            // Deleting the audio file
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);

                // Deleting associated description file if it exists
                string descriptionFileName = $"{fileName}.txt";
                string descriptionFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "audiofiledescrip", descriptionFileName);
                if (System.IO.File.Exists(descriptionFilePath))
                {
                    System.IO.File.Delete(descriptionFilePath);
                }

                RefreshAudioFilesList(); // Refreshing list of audio files
                LoadAudioDescriptions(); // Reloading audio file descriptions after deletion
            }

            return RedirectToPage("/EmployeeDashboard"); // Redirecting to the EmployeeDashboard page
        }

        // Method to handle description save
        public IActionResult OnPostSaveDescription(string audioFileName, string description)
        {
            if (AudioDescriptions == null)
            {
                AudioDescriptions = new Dictionary<string, string>();
            }

            if (!string.IsNullOrEmpty(audioFileName))
            {
                AudioDescriptions[audioFileName] = description; // Adding or updating the description for the audio file
                SaveAudioDescriptions(); // Saving the audio file descriptions
            }

            return RedirectToPage("/EmployeeDashboard"); // Redirecting to the EmployeeDashboard page
        }

        // Method to remove GUID prefix from file name
        private string RemoveGuidPrefix(string fileName)
        {
            string[] parts = fileName.Split('_');
            if (parts.Length > 1)
            {
                return string.Join("_", parts.Skip(1)); // Joining the parts after skipping the GUID prefix
            }

            return fileName;
        }

        // Method to refresh the list of audio files
        private void RefreshAudioFilesList()
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "audio");
            // Getting file names from the folder and adding them to the list
            AudioFiles = Directory.GetFiles(uploadsFolder).Select(Path.GetFileName).ToList();
        }

        // Method to load existing audio file descriptions
        private void LoadAudioDescriptions()
        {
            AudioDescriptions = new Dictionary<string, string>();
            string descriptionsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "audiofiledescrip");
            if (Directory.Exists(descriptionsFolder))
            {
                foreach (string filePath in Directory.GetFiles(descriptionsFolder, "*.txt"))
                {
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    string description = System.IO.File.ReadAllText(filePath);
                    AudioDescriptions.Add(fileName, description); // Adding file name and description to the dictionary
                }
            }
        }

        // Method to save audio file descriptions to text files
        private void SaveAudioDescriptions()
        {
            string descriptionsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "audiofiledescrip");
            if (!Directory.Exists(descriptionsFolder))
            {
                Directory.CreateDirectory(descriptionsFolder); // Creating the folder if it doesn't exist
            }

            foreach (KeyValuePair<string, string> kvp in AudioDescriptions)
            {
                string fileName = $"{kvp.Key}.txt";
                string filePath = Path.Combine(descriptionsFolder, fileName);
                System.IO.File.WriteAllText(filePath, kvp.Value); // Writing description to text file
            }
        }
    }
}
