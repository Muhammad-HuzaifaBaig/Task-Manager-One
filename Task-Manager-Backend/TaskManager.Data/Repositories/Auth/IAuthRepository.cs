using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Model.Model.BusinessLayer.RequestDTO;
using TaskManager.Model.Model.Common;

namespace TaskManager.Data.Repositories.Auth
{
    public interface IAuthRepository
    {
        Task<ResponseMessage> Login(LoginRequestDTO loginRequestDTO);
        Task<ResponseMessage> Signup(SignupRequestDTO signupRequestDTO, string firstName, string lastName);    
    }
}
