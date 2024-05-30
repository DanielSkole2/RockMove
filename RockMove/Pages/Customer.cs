namespace RockMove.Pages
{


    public class Customer
    {

        //Here are 3 properties:
        public long Id { get; }

        public string Username { get; }

        public string Password { get; }

        // Constructor without parameters.
        public Customer()
        {

            // Assigns the result of the method CreateUniqueId() to the Id property.
            Id = CreateUniqueId();

        }

        // Constructor with parameters.
        public Customer(string username, string password)
        {


            Username = username;
            // Assign the value of the parameter "username" to the Username property.

            Password = password;
            // Assign the value of the parameter "password" to the Password property.

            Id = CreateUniqueId();
            // Assign the result of the method CreateUniqueId() to the Id property.
        }


        // Create a unique Customer ID based on the current time in milliseconds.

        private long CreateUniqueId()
        {

            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            // Return the current timestamp in milliseconds.
        }
    }
}
