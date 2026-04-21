using GameLibraryDomain.Model;
using Microsoft.AspNetCore.Identity;

namespace GameLibraryInfrastructure.Models
{
    public class User : IdentityUser
    {
        public DateTime Createdat { get; set; }
        public virtual ICollection<Userlibrary> Userlibraries { get; set; } = new List<Userlibrary>();
    }
}
