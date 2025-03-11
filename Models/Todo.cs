using System.ComponentModel.DataAnnotations.Schema;
namespace TodoApp.Models;
public class Todo
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public Status Status { get; set; }

    public Priority Priority { get; set; }


    public DateTime? DueDate { get; set; }
    public virtual  TodoList? List
    {
        get; set;
    }
    [ForeignKey(nameof(TodoList))]
    public int? ListId { get; set; }

    [ForeignKey(nameof(ApplicationUser))]
    public string? UserId { get; set; }
    public virtual ApplicationUser? User { get; set; }
    [NotMapped]
    public bool IsOverDue => this.Status != Status.Cancelled
        && this.Status != Status.Completed
        && DueDate is not null && DueDate < DateTime.Now;
}
