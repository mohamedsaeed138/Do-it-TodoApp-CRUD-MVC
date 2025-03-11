using System.Drawing;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TodoApp.Models;

namespace TodoApp.Entities;

public class TodoContext : IdentityDbContext<ApplicationUser>
{
    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Todo>().Property(e => e.Status).HasConversion<int>();

        modelBuilder.Entity<Todo>().Property(e => e.Priority).HasConversion<int>();

        modelBuilder
            .Entity<TodoList>()
            .HasIndex(list => new { list.Title, list.UserId })
            .IsUnique();
        //modelBuilder.Entity<TodoList>()
        //                .HasMany(list => list.Todos)
        //                .WithOne(todo => todo.List)
        //                .HasForeignKey(t => t.ListId)
        //                .OnDelete(DeleteBehavior.Cascade);
        modelBuilder
            .Entity<ApplicationUser>()
            .HasMany(user => user.Todos)
            .WithOne(todo => todo.User)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder
            .Entity<ApplicationUser>()
            .HasMany(user => user.TodosLists)
            .WithOne(user => user.User)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        var colorConverter = new ValueConverter<Color, string>(
            v => ColorTranslator.ToHtml(v), // Convert Color to string
            v => ColorTranslator.FromHtml(v)
        ); // Convert string to Color

        modelBuilder.Entity<TodoList>().Property(e => e.Color).HasConversion(colorConverter);
        base.OnModelCreating(modelBuilder);
    }

    public virtual DbSet<Todo> Todos { set; get; }
    public virtual DbSet<TodoList> TodoLists { set; get; }
}
