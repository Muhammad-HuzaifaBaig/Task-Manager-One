using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Model.Model.BusinessLayer.ResponseDTO
{
    public class TaskResponseDTO
    {
        public int? TaskId { get; set; }
        public string? TaskTitle { get; set; }
        public string? Description { get; set; }
        public int? StatusId { get; set; }
        public string? StatusName { get; set; }
        public int? PriorityId { get; set; }
        public string? PriorityName { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Tags { get; set; }
        public int? AssignedUserId { get; set; }
        public string? AssignedUserName { get; set; }
    }
}
