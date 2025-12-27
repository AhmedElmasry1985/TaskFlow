using System.Xml;
using Microsoft.EntityFrameworkCore;
using TasksApi.Models;
using Task = TasksApi.Models.Task;

namespace TasksApi.Data;

public class AppDbContext:DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
    {
        
    }
    
    public DbSet<Task> Tasks { get; set; }
    public DbSet<TaskStatusLT> TaskStatusLT { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Note> Notes { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TaskStatusLTConfiguration());
        //Tasks to Users
        modelBuilder.Entity<Task>()
            .HasOne(t=>t.CreatorUser)
            .WithMany(u=>u.CreatedTasks)
            .HasForeignKey(t=>t.CreatorUserId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<Task>()
            .HasOne(t=>t.AssignedUser)
            .WithMany(u=>u.AssignedTasks)
            .HasForeignKey(t=>t.AssignedUserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        //Tasks to TaskStatusLT
        modelBuilder.Entity<Task>()
            .HasOne(t=>t.Status)
            .WithMany(ts=>ts.Tasks)
            .HasForeignKey(t=>t.StatusId)
            .OnDelete(DeleteBehavior.Restrict);
        
        //Tasks to Notes
        modelBuilder.Entity<Task>()
            .HasMany(t=>t.Notes)
            .WithOne(n=>n.Task)
            .HasForeignKey(n=>n.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    
}