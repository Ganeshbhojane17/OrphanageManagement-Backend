using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orphanage.Core.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Token { get; set; } = null!;
        public DateTime Expires { get; set; }
        public bool IsRevoked { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
