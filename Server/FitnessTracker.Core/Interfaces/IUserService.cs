using FitnessTracker.Core.DTO.Auth;

namespace FitnessTracker.Core.Interfaces;

public interface IUserService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}