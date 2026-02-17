using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Model.Model.DataLayer;

namespace TaskManager.Business.Helpers
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}
