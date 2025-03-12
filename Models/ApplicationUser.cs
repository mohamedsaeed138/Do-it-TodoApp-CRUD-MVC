using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TodoApp.Models;

public class ApplicationUser : IdentityUser
{
    [MaxLength(100)]
    public string Name { get; set; }
    public virtual ICollection<TodoList> TodosLists { get; set; } = new List<TodoList>();
    public virtual ICollection<Todo> Todos { get; set; } = new List<Todo>();
}
