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
    // Defining a Razor Pages model for creating artists, requiring authorization with specific roles and using cookie authentication.
    [Authorize(Roles = "Admin,Employee", AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class CreateArtistModel : PageModel
    {
        // Property to bind the Artist model from form data.
        [BindProperty]
        public Artist? Artist { get; set; }


        public IActionResult OnPost()
        {
            // If form data is not valid, return the page.
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Read existing artists from file.
            List<Artist> artists = ArtistStore.ReadArtistsFromFile();

            // Generate an ID for the new artist.


            /*artists.Count > 0: This checks if there are any artists in the artists list. If the count of artists is >0 it means there are existing artists.
            artists.Max(a => a.Id): This part is a LINQ query (used. It finds the maximum value of the Id property among all the Artist objects in the artists list. 
            The Max method takes a lambda expression (a => a.Id) as an argument. This lambda expression represents 
            a function that takes an Artist object (a) and returns the value of its Id property.

            + 1: If there are existing artists, we want to generate a new ID for the next artist. To do this, we add 1 to the maximum ID found in the previous step.

            : 1: This is the "else" part of the ternary conditional operator (? :). If artists.Count is not greater than zero (= zero artists), 
            then nextId is assigned a value of 1 directly, indicating that the new artist will have ID 1.

            So, overall, this line of code calculates the next available ID for the new artist to be added. If there are existing artists,
            it finds the maximum ID and adds 1 to it. If there are no existing artists, it assigns an ID of 1 to the new artist directly.
            After calculating the nextId, it assigns this ID to the Artist object.*/

            int nextId = artists.Count > 0 ? artists.Max(a => a.Id) + 1 : 1;
            Artist.Id = nextId;

            // Add the new artist to the list.
            artists.Add(Artist);

            // Write the updated list of artists to file.
            ArtistStore.WriteArtistsToFile(artists);

            // Redirect to the Artist page after successfully adding the artist.
            return RedirectToPage("./Artist");
        }
    }
}
