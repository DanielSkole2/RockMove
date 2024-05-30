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
    public class EditArtistModel : PageModel
    {
        // Property to bind the Artist object to the page.
        [BindProperty]
        public Artist Artist { get; set; }

        // Handles GET requests for editing an artist.
        // Retrieves the artist with the given ID from the data store and renders the page.
        public IActionResult OnGet(int id)
        {
            /*This lambda expression is used to find the first Artist object in the collection returned 
             * by ArtistStore.ReadArtistsFromFile() 
             * whose Id property matches the provided id. If such an artist is found, it is assigned to the Artist property of 
             * the EditArtistModel class. 
             * If no matching artist is found, null is assigned to Artist, and a 404 Not Found response is returned.*/

            Artist = ArtistStore.ReadArtistsFromFile().FirstOrDefault(a => a.Id == id);

            if (Artist == null)
            {
                return NotFound(); // Returns a 404 Not Found response if the artist is not found.
            }
            return Page(); // Renders the page.
        }


        public IActionResult OnPost()
        {
            // Checks if the model state is not valid.
            if (!ModelState.IsValid)
            {
                return Page(); // Returns the current page if the model state is not valid.
            }

            // Retrieves the list of artists from the data store.
            List<Artist> artists = ArtistStore.ReadArtistsFromFile();

            // Finds the index of the artist to be edited in the list.
            /*The FindIndex method searches through the artists list and returns the index of the 
             * first element that satisfies the condition specified by the lambda expression. 
             * In this case, it searches for an Artist object in the list whose Id property matches the Id property of the Artist 
             * object stored in the Artist property of the EditArtistModel class. 
             * If such an element is found, its index in the list is assigned to the index variable. 
             * If no matching element is found, -1 is assigned to index.*/

            int index = artists.FindIndex(a => a.Id == Artist.Id);

            // If the artist is found in the list.
            if (index != -1)
            {
                // Updates the artist in the list with the new values.
                artists[index] = Artist;
                // Writes the updated list of artists back to the data store.
                ArtistStore.WriteArtistsToFile(artists);
            }


            return RedirectToPage("./Artist");
        }
    }
}
