public class UserService
{
    public void CreateUser(string name, string email, string password, string role)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            throw new Exception("Invalid input");
        }

        var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        if (!emailRegex.IsMatch(email))
        {
            throw new Exception("Invalid email");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        using (var db = new SqlConnection("connectionString"))
        {
            db.Open();
            var command = new SqlCommand($"INSERT INTO Users (Name, Email, PasswordHash, Role) VALUES ('{name}', '{email}', '{passwordHash}', '{role}')", db);
            command.ExecuteNonQuery();
        }
    }

    public List<string> GetUsers()
    {
        var users = new List<string>();

        using (var db = new SqlConnection("connectionString"))
        {
            db.Open();
            var command = new SqlCommand("SELECT Name FROM Users", db);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    users.Add(reader.GetString(0));
                }
            }
        }

        return users;
    }

    public void UpdateUserRole(int userId, string newRole)
    {
        if (newRole != "Admin" && newRole != "User")
        {
            throw new Exception("Invalid role");
        }

        using (var db = new SqlConnection("connectionString"))
        {
            db.Open();
            var command = new SqlCommand($"UPDATE Users SET Role = '{newRole}' WHERE Id = {userId}", db);
            command.ExecuteNonQuery();
        }
    }
}