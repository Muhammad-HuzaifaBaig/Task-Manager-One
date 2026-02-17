using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Model.Model.BusinessLayer.RequestDTO
{
    public class UpdateTaskRequestDTO
    {
        public string? TaskTitle { get; set; }
        public string? TaskDescription { get; set; }
        public int? AssignedUserId { get; set; }
        public int? StatusId { get; set; }
        public int? PriorityId { get; set; }
        public DateTime? DueDate { get; set; }
        public string? AssignedTo { get; set; }
        public string? Tags { get; set; }
    }
}
