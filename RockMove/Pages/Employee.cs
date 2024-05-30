namespace RockMove.Pages
{
    public class Employee
    {
        public string Username { get; }
        public string Password { get; }

        // The default constructor is here so that one can create Employee objects without providing a username and password.
        public Employee()
        {

        }

        public Employee(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
