using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManager.Model.DataLayer;
using TaskManager.Model.Model.Common;
using TaskManager.Model.Model.DataLayer;

namespace TaskManager.Data.Repositories.Configuration
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        private readonly TaskManagerDbContext _context;

        public ConfigurationRepository(TaskManagerDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseMessage> GetPrioritiesAsync()
        {
            try
            {
                var priorities = await _context.TaskPriorities.ToListAsync();

                return ResponseMessage.Ok(priorities);
            }
            catch (Exception ex)
            {
                return ResponseMessage.Error(ex.Message);
            }
        }

        public async Task<ResponseMessage> GetStatusesAsync()
        {
            try
            {
                var statuses = await _context.TaskStatuses.ToListAsync();

                return ResponseMessage.Ok(statuses);
            }
            catch (Exception ex)
            {
                return ResponseMessage.Error(ex.Message);
            }
        }
    }
}
