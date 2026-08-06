using System;
using System.Threading.Tasks;
using MedCore.Domain.Entities.Core;

namespace MedCore.Application.Features.Auth.Interfaces;

public interface ITokenService
{
    Task<string> GenerateAccessTokenAsync(User user);
    string GenerateRefreshToken();
}
