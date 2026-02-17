using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Business.Helpers;
using TaskManager.Data.Repositories.Auth;
using TaskManager.Model.Model.BusinessLayer.RequestDTO;
using TaskManager.Model.Model.BusinessLayer.ResponseDTO;
using TaskManager.Model.Model.Common;
using TaskManager.Model.Model.DataLayer;

namespace TaskManager.Business.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IConfiguration _configuration;

        public AuthService(IAuthRepository authRepository, IJwtTokenGenerator jwtTokenGenerator, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _configuration = configuration;
        }

        public async Task<ResponseMessage> Login(LoginRequestDTO loginRequestDTO)
        {
            try
            {
                var response = await _authRepository.Login(loginRequestDTO);

                if (!response.success || response.data == null)
                {
                    return ResponseMessage.Unauthorized("Invalid username or password");
                }

                var user = response.data as User;
                if (user == null)
                {
                    return ResponseMessage.Unauthorized("Invalid user data");
                }

                var isPasswordValid = BCrypt.Net.BCrypt.Verify(loginRequestDTO.Password, user.Password);
                if (!isPasswordValid)
                {
                    return ResponseMessage.Unauthorized("Invalid username or password");
                }

                var token = _jwtTokenGenerator.GenerateToken(user);
                var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationMinutes"] ?? "60");

                var loginResponse = new LoginResponseDTO
                {
                    Token = token,
                    UserId = user.UserId,
                    Email = user.Email ?? string.Empty,
                    FullName = user.FullName ?? string.Empty,
                    RoleId = user.RoleId,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes)
                };

                return ResponseMessage.Ok(loginResponse, "Login successful"); 
            }
            catch (Exception ex)
            {
                return ResponseMessage.Error(ex.Message);
            }
        }

        public async Task<ResponseMessage> Signup(SignupRequestDTO signupRequestDTO)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(signupRequestDTO.Email) || 
                    string.IsNullOrWhiteSpace(signupRequestDTO.Password) ||
                    string.IsNullOrWhiteSpace(signupRequestDTO.FullName))
                {
                    return ResponseMessage.BadRequest("All fields are required");
                }

                // Parse FullName into FirstName and LastName
                var nameParts = signupRequestDTO.FullName.Trim().Split(' ', 2);
                var firstName = nameParts[0];
                var lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;

                // Hash password using BCrypt (CRITICAL for security)
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(signupRequestDTO.Password);

                // Create new signup DTO with hashed password and parsed names
                var signupWithHashedPassword = new SignupRequestDTO
                {
                    Email = signupRequestDTO.Email.Trim().ToLower(),
                    FullName = signupRequestDTO.FullName.Trim(),
                    Password = hashedPassword
                };

                var response = await _authRepository.Signup(signupWithHashedPassword, firstName, lastName);

                if (!response.success)
                {
                    return ResponseMessage.BadRequest(response.message);
                }

                return ResponseMessage.Created(null!, "Signup successful. Please login to continue.");
            }
            catch (Exception ex)
            {
                return ResponseMessage.Error(ex.Message);
            }
        }
    }
}
