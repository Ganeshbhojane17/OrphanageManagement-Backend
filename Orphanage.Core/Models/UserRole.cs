using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orphanage.Core.Models
{
    public class UserRole
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}
