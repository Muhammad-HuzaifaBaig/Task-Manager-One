    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using TaskManager.Data.Repositories.Task;
    using TaskManager.Model.Enum;
    using TaskManager.Model.Model.BusinessLayer.RequestDTO;
    using TaskManager.Model.Model.BusinessLayer.ResponseDTO;
    using TaskManager.Model.Model.Common;
    using TaskManager.Model.Model.DataLayer;

    namespace TaskManager.Business.Services.Task
    {
        public class TaskService : ITaskService
        {
            private readonly ITaskRepository _taskRepository;

            public TaskService(ITaskRepository taskRepository)
            {
                _taskRepository = taskRepository;
            }

            public async Task<ResponseMessage> CreateTaskAsync(CreateTaskRequestDTO dto, int userId, int roleId)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(dto.TaskTitle))
                    {
                        return ResponseMessage.BadRequest("Task title is required");
                    }

                    if (dto.AssignedUserId <= 0)
                    {
                        return ResponseMessage.BadRequest("Valid assigned user ID is required");
                    }

                    if (roleId == (int)Model.Enum.eRole.Admin)
                    {
                        if (dto.AssignedUserId == userId)
                        {
                            return ResponseMessage.BadRequest("Admin cannot assign task to themselves");
                        }
                    }
                    else
                    {
                        if (dto.AssignedUserId != userId)
                        {
                            return ResponseMessage.Unauthorized("Users can only create tasks for themselves");
                        }
                    }

                    ResponseMessage userResponse = null!;

                    if (dto.AssignedUserId.HasValue)
                    {
                        userResponse = await _taskRepository.GetUserByIdAsync(dto.AssignedUserId.Value);
                    }

                    if (!userResponse.success)
                    {
                        return ResponseMessage.NotFound("Assigned user not found or inactive");
                    }

                    var assignedUser = userResponse.data as User;
                    if (assignedUser == null)
                    {
                        return ResponseMessage.NotFound("Assigned user not found");
                    }

                    var task = new Model.Model.DataLayer.Task
                    {
                        TaskTitle = dto.TaskTitle.Trim(),
                        Description = dto.Description?.Trim(),
                        TaskStatusId = dto.TaskStatusId,
                        TaskPriorityId = dto.TaskPriorityId,
                        DueDate = dto.DueDate,
                        UserId = dto.AssignedUserId ?? null,
                        Tags = dto.Tags?.Trim(),
                        IsActive = true,
                        CreatedOn = DateTime.UtcNow,
                        CreatedBy = userId
                    };

                    var response = await _taskRepository.CreateAsync(task);
                    return response;
                }
                catch (Exception ex)
                {
                    return ResponseMessage.Error(ex.Message);
                }
            }

            public async Task<ResponseMessage> GetTaskByIdAsync(int taskId)
            {
                try
                {
                    if (taskId <= 0)
                    {
                        return ResponseMessage.BadRequest("Valid task ID is required");
                    }

                    var response = await _taskRepository.GetByIdAsync(taskId);

                    var task = response.data as Model.Model.DataLayer.Task;

                    if (task == null)
                    {
                        return ResponseMessage.NotFound("Task not found");
                    }

                    var taskDTO = new TaskResponseDTO();

                    taskDTO.TaskId = task.TaskId;
                    taskDTO.TaskTitle = task.TaskTitle;
                    taskDTO.Description = task.Description;
                    taskDTO.StatusId = task.TaskStatusId;
                    taskDTO.StatusName = task.TaskStatus?.TaskStatusName;
                    taskDTO.PriorityId = task.TaskPriorityId;
                    taskDTO.PriorityName = task.TaskPriority?.TaskPriorityName;
                    taskDTO.DueDate = task.DueDate;
                    taskDTO.Tags = task.Tags;

                    return ResponseMessage.Ok(taskDTO, "Success");
                }
                catch (Exception ex)
                {
                    return ResponseMessage.Error(ex.Message);
                }
            }

            public async Task<ResponseMessage> GetMyTasksAsync(int userId, int? statusId)
            {
                try
                {
                    if (userId <= 0)
                    {
                        return ResponseMessage.BadRequest("Valid user ID is required");
                    }

                    var response = await _taskRepository.GetTasksByUserIdAsync(userId, statusId);

                    var tasks = response.data as List<Model.Model.DataLayer.Task>;

                    if (tasks == null)
                    {
                        return ResponseMessage.NotFound("No tasks found");
                    }

                    var list = new List<TaskResponseDTO>();

                    foreach (var task in tasks)
                    {
                        var taskDTO = new TaskResponseDTO();

                        taskDTO.TaskId = task.TaskId;
                        taskDTO.TaskTitle = task.TaskTitle ?? "";
                        taskDTO.Description = task.Description ?? "";
                        taskDTO.StatusId = task.TaskStatusId;
                        taskDTO.StatusName = task.TaskStatus?.TaskStatusName ?? "";
                        taskDTO.PriorityId = task.TaskPriorityId;
                        taskDTO.PriorityName = task.TaskPriority?.TaskPriorityName ?? "";
                        taskDTO.DueDate = task.DueDate;
                        taskDTO.Tags = task.Tags ?? "";

                        list.Add(taskDTO);
                    }

                    return ResponseMessage.Ok(list, "Success");
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
                    var response = await _taskRepository.GetAllTasksAsync(statusId);

                    var tasks = response.data as List<Model.Model.DataLayer.Task>;

                    if (tasks == null)
                    {
                        return ResponseMessage.NotFound("No tasks found");
                    }

                    // Fetch all assigned users for the tasks
                    var userIds = tasks.Where(t => t.UserId.HasValue).Select(t => t.UserId.Value).Distinct().ToList();
                    var userDict = new Dictionary<int, string>();
                
                    foreach (var uid in userIds)
                    {
                        var userResponse = await _taskRepository.GetUserByIdAsync(uid);
                        if (userResponse.success && userResponse.data != null)
                        {
                            var user = userResponse.data as User;
                            if (user != null)
                            {
                                userDict[uid] = user.FullName ?? user.Email ?? "Unknown";
                            }
                        }
                    }

                    var list = new List<TaskResponseDTO>();

                    foreach (var task in tasks)
                    {
                        var taskDTO = new TaskResponseDTO();

                        taskDTO.TaskId = task.TaskId;
                        taskDTO.TaskTitle = task.TaskTitle ?? "";
                        taskDTO.Description = task.Description ?? "";
                        taskDTO.StatusId = task.TaskStatusId;
                        taskDTO.StatusName = task.TaskStatus?.TaskStatusName ?? "";
                        taskDTO.PriorityId = task.TaskPriorityId;
                        taskDTO.PriorityName = task.TaskPriority?.TaskPriorityName ?? "";
                        taskDTO.DueDate = task.DueDate;
                        taskDTO.Tags = task.Tags ?? "";
                        taskDTO.AssignedUserId = task.UserId;
                        taskDTO.AssignedUserName = task.UserId.HasValue && userDict.ContainsKey(task.UserId.Value) ? userDict[task.UserId.Value] : null;

                        list.Add(taskDTO);
                    }

                    return ResponseMessage.Ok(list, "Success");
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
                    if (taskId <= 0)
                    {
                        return ResponseMessage.BadRequest("Valid task ID is required");
                    }

                    var response = await _taskRepository.DeleteTaskByIdAsync(taskId);

                    if (!response.success)
                    {
                        return ResponseMessage.Error("Task deletion failed");

                    }

                    return ResponseMessage.Ok(response);
                }
                catch (Exception ex)
                {
                    return ResponseMessage.Error(ex.Message);
                }
            }

            public async Task<ResponseMessage> UpdateTaskByIdAsync(int taskId, UpdateTaskRequestDTO dto)
            {
                try
                {
                    if (taskId <= 0)
                    {
                        return ResponseMessage.BadRequest("Valid task ID is required");
                    }
                    var taskresponse = await _taskRepository.GetByIdAsync(taskId);

                    var task = taskresponse.data as Model.Model.DataLayer.Task;

                    if (task == null)
                    {
                        return ResponseMessage.NotFound("Task not found");
                    }

                    if (dto == null)
                    {
                        return ResponseMessage.BadRequest("Update data is required");
                    }

                    task.TaskTitle = dto.TaskTitle ?? task.TaskTitle;
                    task.Description = dto.TaskDescription ?? task.Description;
                    task.TaskStatusId = dto.StatusId ?? task.TaskStatusId;
                    task.TaskPriorityId = dto.PriorityId ?? task.TaskPriorityId;
                    task.DueDate = dto.DueDate ?? task.DueDate;
                    task.Tags = dto.Tags ?? task.Tags;

                    var response = await _taskRepository.UpdateTaskAsync(task);

                    return ResponseMessage.Ok(response);
                }
                catch (Exception ex)
                {
                    return ResponseMessage.Error(ex.Message);
                }
            }

            public async Task<ResponseMessage> GetDashboardAsync(int? userId)
            {
                try
                {
                    // If userId is null, get all users' data (admin view)
                    // If userId is provided, get specific user's data (normal user view)
                    ResponseMessage response;
                
                    if (userId.HasValue)
                    {
                        if (userId.Value <= 0)
                        {
                            return ResponseMessage.BadRequest("Valid user ID is required");
                        }
                        response = await _taskRepository.GetDashboardDataAsync(userId.Value);
                    }
                    else
                    {
                        // Admin view - get all tasks
                        response = await _taskRepository.GetAllTasksAsync(null);
                    }

                    if (!response.success)
                    {
                        return response;
                    }

                    var tasks = response.data as List<Model.Model.DataLayer.Task>;

                    if (tasks == null)
                    {
                        return ResponseMessage.NotFound("No tasks found");
                    }

                    var currentMonth = DateTime.UtcNow.Month;
                    var currentYear = DateTime.UtcNow.Year;

                    var previousMonth = currentMonth == 1 ? 12 : currentMonth - 1;
                    var previousYear = currentMonth == 1 ? currentYear - 1 : currentYear;

                    // For growth calculation
                    int previousMonthCompletedTasks = 0;
                
                    if (userId.HasValue)
                    {
                        var previousMonthResponse = await _taskRepository.GetCompletedTasksCountByMonthAsync(userId.Value, previousYear, previousMonth);
                        if (previousMonthResponse.success && previousMonthResponse.data != null)
                        {
                            previousMonthCompletedTasks = (int)previousMonthResponse.data;
                        }
                    }
                    else
                    {
                        // For admin, calculate manually from all tasks
                        var allTasksResponse = await _taskRepository.GetAllTasksAsync(null);
                        var allTasks = allTasksResponse.data as List<Model.Model.DataLayer.Task>;
                        if (allTasks != null)
                        {
                            previousMonthCompletedTasks = allTasks.Count(t => 
                                t.TaskStatusId == (int)Model.Enum.eTaskStatus.Completed &&
                                t.CreatedOn.HasValue &&
                                t.CreatedOn.Value.Year == previousYear &&
                                t.CreatedOn.Value.Month == previousMonth);
                        }
                    }

                    var totalTasks = tasks.Count;
                    var completedTasks = tasks.Count(t => t.TaskStatusId == (int)Model.Enum.eTaskStatus.Completed);
                    var pendingTasks = tasks.Count(t => t.TaskStatusId == (int)Model.Enum.eTaskStatus.Pending);
                    var inProgressTasks = tasks.Count(t => t.TaskStatusId == (int)Model.Enum.eTaskStatus.InProgress);

                    var completionPercentage = totalTasks > 0 ? (decimal)completedTasks / totalTasks * 100 : 0;

                    var growthPercentage = 0m;
                    if (previousMonthCompletedTasks > 0)
                    {
                        growthPercentage = ((decimal)(completedTasks - previousMonthCompletedTasks) / previousMonthCompletedTasks) * 100;
                    }
                    else if (completedTasks > 0)
                    {
                        growthPercentage = 100;
                    }

                    // Fetch all assigned users for the tasks
                    var userIds = tasks.Where(t => t.UserId.HasValue).Select(t => t.UserId.Value).Distinct().ToList();
                    var userDict = new Dictionary<int, string>();
                
                    foreach (var uid in userIds)
                    {
                        var userResponse = await _taskRepository.GetUserByIdAsync(uid);
                        if (userResponse.success && userResponse.data != null)
                        {
                            var user = userResponse.data as User;
                            if (user != null)
                            {
                                userDict[uid] = user.FullName ?? user.Email ?? "Unknown";
                            }
                        }
                    }

                    var taskDtos = tasks.Select(t => new TaskResponseDTO
                    {
                        TaskId = t.TaskId,
                        TaskTitle = t.TaskTitle,
                        Description = t.Description,
                        StatusId = t.TaskStatusId,
                        StatusName = t.TaskStatus?.TaskStatusName,
                        PriorityId = t.TaskPriorityId,
                        PriorityName = t.TaskPriority?.TaskPriorityName,
                        DueDate = t.DueDate,
                        Tags = t.Tags,
                        AssignedUserId = t.UserId,
                        AssignedUserName = t.UserId.HasValue && userDict.ContainsKey(t.UserId.Value) ? userDict[t.UserId.Value] : null
                    }).ToList();

                    var dashboardDto = new DashboardResponseDTO
                    {
                        TotalTasks = totalTasks,
                        CompletedTasks = completedTasks,
                        PendingTasks = pendingTasks,
                        InProgressTasks = inProgressTasks,
                        CompletionPercentage = Math.Round(completionPercentage, 2),
                        GrowthPercentage = Math.Round(growthPercentage, 2),
                        PreviousMonthCompletedTasks = previousMonthCompletedTasks,
                        Tasks = taskDtos
                    };

                    return ResponseMessage.Ok(dashboardDto, "Dashboard data retrieved successfully");
                }
                catch (Exception ex)
                {
                    return ResponseMessage.Error(ex.Message);
                }
            }

            public async Task<ResponseMessage> GetUserProfileAsync(int userId)
            {
                try
                {
                    if (userId <= 0)
                    {
                        return ResponseMessage.BadRequest("Valid user ID is required");
                    }
                    var response = await _taskRepository.GetUserByIdAsync(userId);
                    var user = response.data as User;
                    if (user == null)
                    {
                        return ResponseMessage.NotFound("User not found");
                    }

                    var tasks = await _taskRepository.GetTasksByUserIdAsync(userId, null);

                    var taskList = tasks.data as List<Model.Model.DataLayer.Task> ?? new();

                    int totalTasks = taskList.Count;

                    int completedTasks = taskList.Count(t => t.TaskStatusId == (int)eTaskStatus.Completed);

                    decimal completionRate = totalTasks == 0
                        ? 0
                        : Math.Round((completedTasks * 100m) / totalTasks, 2);

                    var userProfileDto = new UserProfileResponseDTO
                    {
                        UserId = user.UserId,
                        UserName = user.FullName ?? string.Empty,
                        Email = user.Email ?? string.Empty,
                        RoleId = user.RoleId ?? 0,
                        RoleName = user.Role?.RoleName ?? string.Empty,

                        TotalTasks = totalTasks,
                        CompletedTasks = completedTasks,
                        CompletionRate = completionRate
                    };


                    return ResponseMessage.Ok(userProfileDto, "User profile retrieved successfully");
                }
                catch (Exception ex)
                {
                    return ResponseMessage.Error(ex.Message);
                }
            }
        }
    }