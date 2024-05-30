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
    //Doing so only those with roles as Admin or Employee can go the this page
    [Authorize(Roles = "Admin,Employee", AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class ArtistModel : PageModel
    {
        //Declaring a public property Artists of type List<Artist>
        public List<Artist> Artists { get; set; }

        public void OnGet()
        {
            // Reading the list of artists from a file and assigning it to the Artists property
            Artists = ArtistStore.ReadArtistsFromFile();
        }

        public IActionResult OnPostCreate(Artist artist)
        {
            // Checking if the model state is valid or not valid
            if (!ModelState.IsValid)
            {
                return Page();
            }
            // Reading the list of artists from a file
            List<Artist> artists = ArtistStore.ReadArtistsFromFile();
            // Adding the new artist to the list
            artists.Add(artist);
            // Writing the updated list of artists back to the file
            ArtistStore.WriteArtistsToFile(artists);

            return RedirectToPage("./Index");
        }

        public IActionResult OnPostEdit(Artist artist)
        {
            // Checking if the model state is valid or not valid
            if (!ModelState.IsValid)
            {
                return Page();
            }
            // Reading the list of artists from a file
            List<Artist> artists = ArtistStore.ReadArtistsFromFile();

            // Finding the index of the artist to be edited in the list. below is some more detail of this
            //(a => a.Id == artist.Id) is a lambda expression,
            //where a represents each element of the list "artists"
            //a.Id == artist.Id is the condition that is checked for each element a in the list
            //If the condition a.Id == artist.Id is true for any element a in the list,
            //FindIndex returns the index of that element. Otherwise, it returns -1
            int index = artists.FindIndex(a => a.Id == artist.Id);

            // Checking if the artist was found (it checks if the index variable is not equal to -1, which indicates that the item is found in the list)
            if (index != -1)
            {
                // Updating the artist at the found index with the new data
                artists[index] = artist;
                // Writing the updated list of artists back to the file
                ArtistStore.WriteArtistsToFile(artists);
            }

            return RedirectToPage("./Index");
        }

        public IActionResult OnPostDelete(int id)
        {
            List<Artist> artists = ArtistStore.ReadArtistsFromFile();

            // Finding the index of the artist to be edited in the list
            //(a => a.Id == id) is a lambda expression,
            //where a represents each element of the list "artists"
            //a.Id == Id is the condition that is checked for each element a in the list
            //If the condition a.Id == Id is true for any element a in the list,
            //Find returns the first artist that satisfies the condition. If no artist satisfies the condition, Find returns null
            Artist artistToRemove = artists.Find(a => a.Id == id);


            // Checking if the artist was found
            if (artistToRemove != null)
            {
                artists.Remove(artistToRemove);
                ArtistStore.WriteArtistsToFile(artists); // Writing the updated list of artists back to the file
            }

            return RedirectToPage("./Index");
        }
    }
}
