using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TasksApi.Models;

namespace TasksApi.Data;

public class TaskStatusLTConfiguration: IEntityTypeConfiguration<TaskStatusLT>
{
    public void Configure(EntityTypeBuilder<TaskStatusLT> builder)
    {
        builder.HasData(
            new TaskStatusLT { Id = 1, Name = "None" },
            new TaskStatusLT { Id = 2, Name = "Created" },
            new TaskStatusLT { Id = 3, Name = "In Progress" },
            new TaskStatusLT { Id = 4, Name = "Completed" },
            new TaskStatusLT { Id = 5, Name = "Cancelled" }
        );
    }
}