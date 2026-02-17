using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Model.Model.BusinessLayer.RequestDTO
{
    public class CreateTaskRequestDTO
    {
        public string TaskTitle { get; set; } = null!;
        public string? Description { get; set; }
        public int TaskStatusId { get; set; }
        public int TaskPriorityId { get; set; }
        public DateTime? DueDate { get; set; }
        public int? AssignedUserId { get; set; }
        public string? Tags { get; set; }
    }
}
