using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Model.Model.Common;

namespace TaskManager.Business.Services.Configuration
{
    public interface IConfigurationService
    {
        Task<ResponseMessage> GetAllPrioritiesAsync();
        Task<ResponseMessage> GetAllStatusesAsync();
    }
}
