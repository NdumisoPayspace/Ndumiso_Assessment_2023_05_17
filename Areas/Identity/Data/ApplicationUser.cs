using Microsoft.AspNetCore.Identity;

namespace Ndumiso_Assessment_2023_05_17.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string Name { get; set; }
}

