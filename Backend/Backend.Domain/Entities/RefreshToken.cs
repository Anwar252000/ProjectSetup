using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Domain.Entities
{
    public class RefreshToken
    {
        public int RefreshTokenId { get; set; }

        [ForeignKey("Users")]
        public int UserId { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool IsRevoked { get; set; }

        //Navigation Property
        public User? Users { get; set; }
    }
}
