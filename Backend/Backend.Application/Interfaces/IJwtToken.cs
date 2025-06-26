using System.Security.Claims;

namespace Backend.Application.Interfaces
{
    public interface IJwtToken
    {
        string GenerateJwtToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
    }
}
