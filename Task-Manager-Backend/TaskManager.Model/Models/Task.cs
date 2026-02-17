using System;
using System.Collections.Generic;

namespace TaskManager.Model.Models;

public partial class Task
{
    public int TaskId { get; set; }

    public string? TaskTitle { get; set; }

    public string? Description { get; set; }

    public int? TaskStatusId { get; set; }

    public int? TaskPriorityId { get; set; }

    public DateTime? DueDate { get; set; }

    public int? UserId { get; set; }

    public string? Tags { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public int? UpdatedBy { get; set; }
}
