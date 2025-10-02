using Microsoft.AspNetCore.Identity;

namespace AnimeHub.Models;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Bio { get; set; }
    public DateTime? DateOfBirth { get; set; }
}