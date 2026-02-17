using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using TaskManager.Data.Repositories.Configuration;
using TaskManager.Model.Model.Common;
using TaskManager.Model.Model.DataLayer;

namespace TaskManager.Business.Services.Configuration
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfigurationRepository _configurationRepository;

        public ConfigurationService(IConfigurationRepository configurationRepository)
        {
            _configurationRepository = configurationRepository;
        }

        public async Task<ResponseMessage> GetAllPrioritiesAsync()
        {
            try
            {
                var response = await _configurationRepository.GetPrioritiesAsync();

                return ResponseMessage.Ok(response, "Success");
            }
            catch (Exception ex)
            {
                return ResponseMessage.Error(ex.Message);
            }
        }

        public async Task<ResponseMessage> GetAllStatusesAsync()
        {
            try
            {
                var response = await _configurationRepository.GetStatusesAsync();

                return ResponseMessage.Ok(response, "Success");
            }
            catch (Exception ex)
            {
                return ResponseMessage.Error(ex.Message);
            }
        }
    }
}
