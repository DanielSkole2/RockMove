using RockMove.Pages;
using System.Collections.Generic;
using System.IO;

namespace RockMove.Pages
{
    // Public static indicates that the class can be accessed globally, and instances of the class don't need to be created
    public static class ArtistStore
    {
        // This is a constant string because it does not change
        private const string FilePath = "artists.txt";

        // This is a static method that reads artist data from a file and returns a list of Artist objects
        public static List<Artist> ReadArtistsFromFile()
        {
            List<Artist> artists = new List<Artist>();

            if (File.Exists(FilePath))
            {

                // If the file exists, it uses a StreamReader to read the file line by line
                using (StreamReader reader = new StreamReader(FilePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {

                        // Each line is split into 5 parts
                        string[] parts = line.Split(';');
                        if (parts.Length == 5)
                        {
                            int id = int.Parse(parts[0]);
                            string name = parts[1];
                            string genre = parts[2];
                            string period = parts[3];
                            string description = parts[4];

                            // An Artist object is created from the parsed parts and added to the list of artists
                            Artist artist = new Artist(id, name, genre, period, description);
                            artists.Add(artist);
                        }
                    }
                }
            }
            return artists;
        }

        // WriteArtistsToFile is a static method that takes the list of artist objects and writes them to a file
        public static void WriteArtistsToFile(List<Artist> artists)
        {
            // Using StreamWriter to write to a file specified by FilePath
            using (StreamWriter writer = new StreamWriter(FilePath))
            {
                // Iterates through each Artist object in the list and writes its properties to the file
                foreach (Artist artist in artists)
                {
                    writer.WriteLine($"{artist.Id};{artist.Name};{artist.Genre};{artist.Period};{artist.Description}");
                }
            }
        }
    }
}
