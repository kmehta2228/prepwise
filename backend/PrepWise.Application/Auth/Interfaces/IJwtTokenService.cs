using PrepWise.Domain.Entities;

namespace PrepWise.Application.Auth.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}
