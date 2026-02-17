using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManager.Model.DataLayer;
using TaskManager.Model.Model.Common;
using TaskManager.Model.Model.DataLayer;
using TaskManager.Model.Enum;

namespace TaskManager.Data.Repositories.Task
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TaskManagerDbContext _context;

        public TaskRepository(TaskManagerDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseMessage> CreateAsync(Model.Model.DataLayer.Task task)
        {
            try
            {
                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();

                return ResponseMessage.Created(task, "Task created successfully");
            }
            catch (Exception ex)
            {
                return ResponseMessage.Error(ex.Message);
            }
        }

        public async Task<ResponseMessage> GetUserByIdAsync(int userId)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive == true);

                if (user == null)
                {
                    return ResponseMessage.NotFound("User not found");
                }

                return ResponseMessage.Ok(user);
            }
            catch (Exception ex)
            {
                return ResponseMessage.Error(ex.Message);
            }
        }

        public async Task<ResponseMessage> GetByIdAsync(int taskId)
        {
            try
            {
                var task = await _context.Tasks
                    .FirstOrDefaultAsync(t => t.TaskId == taskId && t.IsActive == true);

                if (task == null)
                {
                    return ResponseMessage.NotFound("Task not found");
                }

                return ResponseMessage.Ok(task);
            }
            catch (Exception ex)
            {
                return ResponseMessage.Error(ex.Message);
            }
        }

        public async Task<ResponseMessage> GetTasksByUserIdAsync(int userId, int? statusId)
        {
            try
            {
                var tasks = await _context.Tasks
                    .Where(t => t.UserId == userId && t.IsActive == true)
                    .ToListAsync();

                if (statusId == (int)eTaskStatus.Pending)
                {

                    tasks = await _context.Tasks
                        .Where(t => t.UserId == userId && t.TaskStatusId == statusId && t.IsActive == true)
                        .ToListAsync();
                }

                if (statusId == (int)eTaskStatus.InProgress)
                {
                    tasks = await _context.Tasks
                        .Where(t => t.UserId == userId && t.TaskStatusId == statusId && t.IsActive == true)
                        .ToListAsync();
                }

                if (statusId == (int)eTaskStatus.Completed)
                {
                    tasks = await _context.Tasks
                        .Where(t => t.UserId == userId && t.TaskStatusId == statusId && t.IsActive == true)
                        .ToListAsync();
                }

                return ResponseMessage.Ok(tasks);
            }
            catch (Exception ex)
            {
                return ResponseMessage.Error(ex.Message);
            }
        }

        public async Task<ResponseMessage> GetAllTasksAsync(int? statusId)
        {
            try
            {
                var tasks = await _context.Tasks
                    .Where(t => t.IsActive == true)
                    .ToListAsync();


                if (statusId == (int)eTaskStatus.Pending)
                {

                    tasks = await _context.Tasks
                        .Where(t => t.TaskStatusId == statusId && t.IsActive == true)
                        .ToListAsync();
                }

                if (statusId == (int)eTaskStatus.InProgress)
                {
                    tasks = await _context.Tasks
                        .Where(t => t.TaskStatusId == statusId && t.IsActive == true)
                        .ToListAsync();
                }

                if (statusId == (int)eTaskStatus.Completed)
                {
                    tasks = await _context.Tasks
                        .Where(t => t.TaskStatusId == statusId && t.IsActive == true)
                        .ToListAsync();
                }

                return ResponseMessage.Ok(tasks);
            }
            catch (Exception ex)
            {
                return ResponseMessage.Error(ex.Message);
            }
        }

        public async Task<ResponseMessage> DeleteTaskByIdAsync(int taskId)
        {
            try
            {
                var task = await _context.Tasks.FirstOrDefaultAsync(t => t.TaskId == taskId && t.IsActive == true);

                if (task == null)
                {
                    return ResponseMessage.NotFound("Task not found");
                }

                task.IsActive = false;

                _context.Tasks.Update(task);
                await _context.SaveChangesAsync();

                return ResponseMessage.Ok("Task deleted successfully");
            }
            catch (Exception ex)
            {
                return ResponseMessage.Error(ex.Message);
            }
        }

        public async Task<ResponseMessage> UpdateTaskAsync(Model.Model.DataLayer.Task task)
        {
            try
            {
                if (task == null)
                {
                    return ResponseMessage.NotFound("Task not found");
                }

                _context.Tasks.Update(task);
                _context.SaveChanges();

                return ResponseMessage.Ok("Task updated successfully");
            }
            catch (Exception ex)
            {
                return ResponseMessage.Error(ex.Message);
            }
        }

        public async Task<ResponseMessage> GetDashboardDataAsync(int userId)
        {
            try
            {
                var tasks = await _context.Tasks
                    .Include(t => t.TaskStatus)
                    .Include(t => t.TaskPriority)
                    .Where(t => t.UserId == userId && t.IsActive == true)
                    .ToListAsync();

                return ResponseMessage.Ok(tasks);
            }
            catch (Exception ex)
            {
                return ResponseMessage.Error(ex.Message);
            }
        }

        public async Task<ResponseMessage> GetCompletedTasksCountByMonthAsync(int userId, int year, int month)
        {
            try
            {
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1);

                var count = await _context.Tasks
                    .Where(t => t.UserId == userId 
                        && t.TaskStatusId == (int)eTaskStatus.Completed 
                        && t.IsActive == true
                        && t.UpdatedOn >= startDate 
                        && t.UpdatedOn < endDate)
                    .CountAsync();

                return ResponseMessage.Ok(count);
            }
            catch (Exception ex)
            {
                return ResponseMessage.Error(ex.Message);
            }
        }
    }
}