using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Model.Model.BusinessLayer.RequestDTO;
using TaskManager.Model.Model.Common;

namespace TaskManager.Business.Services.Task
{
    public interface ITaskService
    {
        Task<ResponseMessage> CreateTaskAsync(CreateTaskRequestDTO dto, int userId, int roleId);
        Task<ResponseMessage> GetTaskByIdAsync(int taskId);
        Task<ResponseMessage> GetMyTasksAsync(int userId, int? statusId);
        Task<ResponseMessage> GetAllTasksAsync(int? statusId);
        Task<ResponseMessage> DeleteTaskByIdAsync(int taskId);
        Task<ResponseMessage> UpdateTaskByIdAsync(int taskId, UpdateTaskRequestDTO dto);
        Task<ResponseMessage> GetDashboardAsync(int? userId);
        Task<ResponseMessage> GetUserProfileAsync(int userId);
    }
}
