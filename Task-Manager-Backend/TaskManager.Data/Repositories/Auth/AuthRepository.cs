using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Model.DataLayer;
using TaskManager.Model.Enum;
using TaskManager.Model.Model.BusinessLayer.RequestDTO;
using TaskManager.Model.Model.Common;
using TaskManager.Model.Model.DataLayer;

namespace TaskManager.Data.Repositories.Auth
{
    public class AuthRepository : IAuthRepository
    {
        private readonly TaskManagerDbContext _context;

        public AuthRepository(TaskManagerDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseMessage> Login(LoginRequestDTO loginRequestDTO)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == loginRequestDTO.Username && u.IsActive == true);

                if (user == null)
                {
                    return ResponseMessage.NotFound("User not found");
                }

                return ResponseMessage.Ok(user, "User found");
            }
            catch (Exception ex)
            {
                return ResponseMessage.Error(ex.Message);
            }
        }

        public async Task<ResponseMessage> Signup(SignupRequestDTO signupRequestDTO, string firstName, string lastName)
        {
            try
            {
                var userExists = await _context.Users
                    .AnyAsync(u => u.Email == signupRequestDTO.Email);

                if (userExists)
                {
                    return ResponseMessage.BadRequest("Email already exists");
                }

                var user = new User
                {
                    FullName = signupRequestDTO.FullName,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = signupRequestDTO.Email,
                    Password = signupRequestDTO.Password,
                    RoleId = (int)eRole.User, // Default role for regular users (eRole.User = 2)
                    IsActive = true,
                    CreatedOn = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return ResponseMessage.Created(user, "User created successfully");
            }
            catch (Exception ex)
            {
                return ResponseMessage.Error(ex.Message);
            }
        }
    }
}