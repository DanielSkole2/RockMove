using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RockMove.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RockMove.Pages
{
    // Only accessible to logged-in admins and employees
    [Authorize(Roles = "Admin,Employee", AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class DeleteArtistModel : PageModel
    {
        [BindProperty] // Specifies that this property should be bound with incoming request data.
        public Artist? Artist { get; set; }

        public IActionResult OnGet(int id)
        {

            // Reading artists from a file and assigning the first artist with matching id to the Artist property.
            /*So, the entire expression reads as follows: "Find the first Artist object in the list of artists where the Id property matches the provided id." 
             * If such an Artist object is found, it's assigned to the Artist property of the class. 
             * If no matching Artist object is found (i.e., the sequence is empty), null is assigned to the Artist property.*/
            Artist = ArtistStore.ReadArtistsFromFile().FirstOrDefault(a => a.Id == id);
            if (Artist == null) // Checking if the Artist is null.
            {
                return NotFound(); // Returning a 404 Not Found response.
            }
            return Page(); // Returning the Razor Page.
        }

        public IActionResult OnPost(int id)
        {
            // Reading artists from a file and storing them in a list.
            List<Artist> artists = ArtistStore.ReadArtistsFromFile();

            /*So, the entire expression reads as follows: "Find the first Artist object in the 
             list artists where the Id property matches the provided id."
             If such an Artist object is found, it's assigned to the artistToRemove variable. 
             If no matching Artist object is found, null is assigned to artistToRemove.*/

            Artist artistToRemove = artists.Find(a => a.Id == id); // Finding the artist in the list with the specified id


            if (artistToRemove != null) // Checking if the artist to remove is not null.
            {
                artists.Remove(artistToRemove); // Removing the artist from the list.
                ArtistStore.WriteArtistsToFile(artists); // Writing the updated list of artists back to the file.
            }

            return RedirectToPage("./Artist"); // Redirecting to the Artist page.
        }
    }
}
