using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Identity;

namespace Chatly.Models;

public class User : IdentityUser
{
    public string? DisplayName { get; set; } 
    public string? Theme { get; set; } = "system";
    public DateTime? LastSeen { get; set; } = DateTime.Now;
    public bool IsOnline { get; set; } = false;
}