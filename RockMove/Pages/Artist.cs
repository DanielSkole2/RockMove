using System.ComponentModel.DataAnnotations;

namespace RockMove.Pages
{
    public class Artist
    {
        public int Id { get; set; }
        //Here we specify that the datafield Name is required, using data annotation
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "No more than 100 words")]
        public string Name { get; set; }
        // Data annotation to limit the number of characters for the the different properties

        [StringLength(50, ErrorMessage = "No more than 50 characters")]
        public string Genre { get; set; }
        [StringLength(50, ErrorMessage = "No more than 50 characters")]
        public string Period { get; set; }

        [StringLength(500, ErrorMessage = "No more thaan 500 characters.")]
        public string Description { get; set; }

        // The default constructor is here so that one can create Artist objects without providing a .
        // The default constructor is here so that one can create Artist objects without providing Name, Genre, Period, and Description.
        //This constructor does not set the Name property, but Name is required and should be set later.
        public Artist()
        {

        }

        //Here is a constructor with parameters for the artist class 
        public Artist(int id, string name, string genre, string period, string description)
        {
            Id = id;
            Name = name;
            Genre = genre;
            Period = period;
            Description = description;
        }
    }
}
