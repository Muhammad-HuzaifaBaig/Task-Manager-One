using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Model.Model.BusinessLayer.ResponseDTO
{
    public class DashboardResponseDTO
    {
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
        public int InProgressTasks { get; set; }
        public decimal CompletionPercentage { get; set; }
        public decimal GrowthPercentage { get; set; }
        public int PreviousMonthCompletedTasks { get; set; }
        public List<TaskResponseDTO>? Tasks { get; set; }
    }
}
