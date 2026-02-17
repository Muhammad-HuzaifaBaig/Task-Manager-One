using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Model.Model.Common;

namespace TaskManager.Data.Repositories.Task
{
    public interface ITaskRepository
    {
        Task<ResponseMessage> CreateAsync(Model.Model.DataLayer.Task task);
        Task<ResponseMessage> GetDashboardDataAsync(int userId);
        Task<ResponseMessage> GetUserByIdAsync(int userId);
        Task<ResponseMessage> GetByIdAsync(int taskId);
        Task<ResponseMessage> GetTasksByUserIdAsync(int userId, int? statusId);
        Task<ResponseMessage> GetAllTasksAsync(int? statusId);
        Task<ResponseMessage> DeleteTaskByIdAsync(int taskId);
        Task<ResponseMessage> UpdateTaskAsync(Model.Model.DataLayer.Task task);
        Task<ResponseMessage> GetCompletedTasksCountByMonthAsync(int userId, int previousYear, int previousMonth);
    }
}
