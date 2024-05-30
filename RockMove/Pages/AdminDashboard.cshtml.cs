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

namespace RockMove.Pages
{
    // Only accessible to logged-in admins
    [Authorize(Roles = "Admin", AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class AdminDashboardModel : PageModel
    {
        // Declaring a variable to hold the web hosting environment.
        private readonly IWebHostEnvironment _webHostEnvironment;

        // List to store audio file names
        public List<string> AudioFiles { get; private set; }

        // Dictionary to store audio file descriptions
        public Dictionary<string, string> AudioDescriptions { get; private set; }

        // Constructor to initialize the web hosting environment.
        public AdminDashboardModel(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        // Initialize data on page load
        public void OnGet()
        {
            RefreshAudioFilesList();
            LoadAudioDescriptions();
        }

        // Method to handle audio file upload
        public async Task<IActionResult> OnPostAsync(IFormFile audioFile)
        {
            // Check if the uploaded file is null or empty.
            if (audioFile == null || audioFile.Length == 0)
            {
                return Page();
            }

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "audio"); // Determine the upload folder path.
            string fileName = RemoveGuidPrefix(Path.GetFileName(audioFile.FileName)); //Removing the GUID prefix from the filename
            string filePath = Path.Combine(uploadsFolder, fileName); // Combine the folder path and file name.

            // Create a new file stream to save the uploaded file
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                // Copy the uploaded file to the file stream asynchronously
                await audioFile.CopyToAsync(fileStream);
            }

            RefreshAudioFilesList(); //Refreshing the list of audiofiles after upload
            LoadAudioDescriptions(); // Reload descriptions after upload

            return RedirectToPage("/AdminDashboard");
        }



        // 

        // Method to handle audio file deletion
        public IActionResult OnPostDelete(string fileName)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "audio"); // Determine the upload folder path
            string filePath = Path.Combine(uploadsFolder, fileName); // Combine the folder path and file name

            // Check if the file exists
            if (System.IO.File.Exists(filePath))
            {
                //Delete the file if it exists
                System.IO.File.Delete(filePath);

                // Delete associated description file if it exists
                string descriptionFileName = $"{fileName}.txt"; // Include the extension (the extension here is mp3) in the description file name
                //Determingen the file path of the description
                string descriptionFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "audiofiledescrip", descriptionFileName);
                if (System.IO.File.Exists(descriptionFilePath))
                {
                    //Delete the description file
                    System.IO.File.Delete(descriptionFilePath);
                }

                RefreshAudioFilesList();
                LoadAudioDescriptions(); // Reload descriptions after deletion
            }

            return RedirectToPage("/AdminDashboard");
        }



        // Method to handle description save
        public IActionResult OnPostSaveDescription(string audioFileName, string description)
        {
            //Checking if the audiodescriptions is null
            if (AudioDescriptions == null)
            {
                //Here we initialize the AudioDescriptions dictionary
                AudioDescriptions = new Dictionary<string, string>();
            }
            //Checking if the audiodilename is not null or empty
            if (!string.IsNullOrEmpty(audioFileName))
            {
                //Here we add/update the description for the audiofile
                AudioDescriptions[audioFileName] = description;
                SaveAudioDescriptions();
            }

            return RedirectToPage("/AdminDashboard");
        }




        // Method to remove GUID prefix from file name
        private string RemoveGuidPrefix(string fileName)
        {
            // Here we split the file name by " _ " (underscore)
            string[] parts = fileName.Split('_');
            //Here we check if the split results has more than 1 part (there is included a normal filename part and a guid prefix part)
            if (parts.Length > 1)
            {
                //Join the parts with the exception of the first 1 (the guid prefix part)
                return string.Join("_", parts.Skip(1));
            }
            //You return the original filename is there is no GUID prefix
            return fileName;
        }

        // Method to refresh the list of audio files
        private void RefreshAudioFilesList()
        {
            // Determine the upload folder path
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "audio");
            // Here we get the list of file names in the folder
            AudioFiles = Directory.GetFiles(uploadsFolder).Select(Path.GetFileName).ToList();
        }

        // Method to load existing audio file descriptions
        private void LoadAudioDescriptions()
        {
            // Here we initialize the AudioDescriptions dictionary
            AudioDescriptions = new Dictionary<string, string>();
            // Determine the descriptions folder path
            string descriptionsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "audiofiledescrip");
            // Checking if the descriptions folder exists
            if (Directory.Exists(descriptionsFolder))
            {
                // Iterate through all the text files in the descriptions folder
                foreach (string filePath in Directory.GetFiles(descriptionsFolder, "*.txt"))
                {
                    string fileName = Path.GetFileNameWithoutExtension(filePath); // Get the file name without extension
                    string description = System.IO.File.ReadAllText(filePath); // Read the description from the file
                    AudioDescriptions.Add(fileName, description); // Add the description to the dictionary
                }
            }
        }

        // Method to save audio file descriptions to text files
        private void SaveAudioDescriptions()
        {
            //Determining the folder path of the description
            string descriptionsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "audiofiledescrip");
            //Checking if the descriptionsfolder does not exist
            if (!Directory.Exists(descriptionsFolder))
            {
                //If it is true we create a descriptionsfolder
                Directory.CreateDirectory(descriptionsFolder);
            }
            // Iterate through the audio descriptions dictionary
            foreach (KeyValuePair<string, string> kvp in AudioDescriptions)
            {
                string fileName = $"{kvp.Key}.txt"; // Determine the file name for the description file
                string filePath = Path.Combine(descriptionsFolder, fileName); // Combine the folder path and file name
                System.IO.File.WriteAllText(filePath, kvp.Value); // Write the description to the file
            }
        }
    }
}
