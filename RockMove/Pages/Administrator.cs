namespace RockMove.Pages
{
    public class Administrator
    {
        //Declaring 2 properties
        public string Username { get; }
        public string Password { get; }

        // The default constructor is here so that one can create Administrator objects without providing a username and password.
        public Administrator()
        {

        }
        // Here we are defining a constructor with parameters for our Administrator class
        public Administrator(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
