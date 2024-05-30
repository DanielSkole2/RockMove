using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RockMove.Pages;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RockMove.Pages
{
    // Apply authorization to this Razor Page using cookie-based authentication.
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class AudioFilesModel : PageModel
    {
        // Private field to hold a reference to the web host environment.
        private readonly IWebHostEnvironment _webHostEnvironment;

        // Public property to store a list of audio files.
        public List<AudioFil> AudioFiles { get; private set; }

        // Constructor to initialize the AudioFilesModel with the web host environment.
        public AudioFilesModel(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            AudioFiles = new List<AudioFil>(); // Initialize the list of audio files.
        }


        public void OnGet()
        {
            // Getting the path of audio and audiofiledescrip folders in the web root
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "audio");
            string descriptionsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "audiofiledescrip");

            // Get a list of file names (without paths) in the 'uploadsFolder'.
            IEnumerable<string> audioFileNames = Directory.GetFiles(uploadsFolder).Select(Path.GetFileName);

            // Iterate through each file name.
            foreach (string fileName in audioFileNames)
            {
                // Construct the path to the audio file and its corresponding description file.
                string audioFilePath = Path.Combine("audio", fileName);
                string descriptionFilePath = Path.Combine(descriptionsFolder, fileName + ".txt");

                string description = string.Empty; // Initialize an empty string to store the description.

                // Check if the description file exists.
                if (System.IO.File.Exists(descriptionFilePath))
                {
                    // If the description file exists, read its content.
                    description = System.IO.File.ReadAllText(descriptionFilePath);
                }

                // Create a new AudioFil object and add it to the list of audio files.
                AudioFiles.Add(new AudioFil
                {
                    FileName = fileName,
                    AudioFilePath = audioFilePath,
                    Description = description
                });
            }
        }
    }
}
