using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Model.Model.BusinessLayer.ResponseDTO
{
    public class LoginResponseDTO
    {
        public string Token { get; set; } = null!;
        public int UserId { get; set; }
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public int? RoleId { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
