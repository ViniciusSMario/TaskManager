using Microsoft.EntityFrameworkCore;
using TaskManager.Domain;

namespace TaskManager.Infrastructure;

public class TaskDbContext : DbContext
{
    public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options) { }

    public DbSet<TaskManager.Domain.Task> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskManager.Domain.Task>(entity =>
        {
            entity.Property(t => t.Titulo)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(t => t.Descricao)
                .HasMaxLength(500);

            entity.Property(t => t.DataCriacao)
                .HasDefaultValueSql("GETDATE()");
        });
    }
}
