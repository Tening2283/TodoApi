namespace TodoApi.Models;

// Modèle utilisateur pour l'authentification JWT (exigence TP)
public class AppUser
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    // Rôle : "Admin" ou "User"
    public string Role { get; set; } = "User";
}

public class RegisterRequest
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Role { get; set; } = "User";
}

public class LoginRequest
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class LoginResponse
{
    public string Token { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Role { get; set; } = null!;
}
