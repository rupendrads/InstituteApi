using Microsoft.EntityFrameworkCore;

namespace InstituteApi.Models;

public class InstituteContext : DbContext
{
    public InstituteContext(DbContextOptions<InstituteContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;

    public DbSet<Admission> Admissions { get; set; } = null!;

    public DbSet<Course> Courses { get; set; } = null!;

    public DbSet<Subject> Subjects { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Course>()
            .HasMany(p => p.Subjects)
            .WithMany(p => p.Courses)
            .UsingEntity(j => j.ToTable("CourseSubjects"));
    }
}