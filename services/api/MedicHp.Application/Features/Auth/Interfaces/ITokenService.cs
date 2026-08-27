using System;
using System.Threading.Tasks;
using MedicHp.Domain.Entities.Core;

namespace MedicHp.Application.Features.Auth.Interfaces;

public interface ITokenService
{
    Task<string> GenerateAccessTokenAsync(User user);
    string GenerateRefreshToken();
}
