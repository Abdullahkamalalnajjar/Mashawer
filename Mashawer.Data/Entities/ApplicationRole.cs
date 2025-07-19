using Microsoft.AspNetCore.Identity;

namespace Mashawer.Data.Entities
{
    public class ApplicationRole : IdentityRole
    {
        public bool IsDefualt { get; set; }
        public bool IsDeleted { get; set; }

    }
}
