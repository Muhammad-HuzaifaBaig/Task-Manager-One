using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Model.Model.Common;

namespace TaskManager.Data.Repositories.Configuration
{
    public interface IConfigurationRepository
    {
        Task<ResponseMessage> GetPrioritiesAsync();
        Task<ResponseMessage> GetStatusesAsync();
    }
}
