using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace TodoApp.Models;
public class TodoList
{
    public int Id { get; set; }
    public string Title { get; set; }
    public Color Color { get; set; }
    [ForeignKey(nameof(ApplicationUser))]
    public string? UserId { get; set; }
    public virtual ApplicationUser? User { get; set; }
    public virtual ICollection<Todo> Todos { get; set; } = new List<Todo>();

}