namespace JustDoItApi.Models.Auth;

/// <summary>
/// Json - модель для входу користувача
/// </summary>
public class LoginModel
{
    /// <summary>
    /// Email
    /// </summary>
    /// <example>
    /// admin@example.com
    /// </example>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Password
    /// </summary>
    /// <example>
    /// Admin123!
    /// </example>

    public string Password { get; set; } = string.Empty;
}
