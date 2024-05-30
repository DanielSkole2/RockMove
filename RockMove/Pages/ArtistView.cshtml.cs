using Microsoft.AspNetCore.Mvc.RazorPages;
using RockMove.Pages;
using System.Collections.Generic;
using System.Linq;

namespace RockMove.Pages
{
    public class ArtistViewModel : PageModel
    {
        // Defines a public property named Artists of type List<Artist>
        public List<Artist> Artists { get; set; }

        public void OnGet()
        {
            // Retrieve list of artists from the .txt file
            Artists = ArtistStore.ReadArtistsFromFile();
        }
    }
}
