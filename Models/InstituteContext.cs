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

    public DbSet<Institute> Institutes { get; set; } = null!;

    public DbSet<RoyaltyLevel> RoyaltyLevels { get; set; } = null!;

    public DbSet<RoyaltyLevelDetail> RoyaltyLevelDetails { get; set; } = null!;

    public DbSet<RoyaltyDistribution> RoyaltyDistributions { get; set; } = null!;

    public DbSet<RoyaltyDistributionDetail> RoyaltyDistributionDetails { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Course>()
            .HasMany(p => p.Subjects)
            .WithMany(p => p.Courses)
            .UsingEntity(j => j.ToTable("CourseSubjects"));

        modelBuilder.Entity<Institute>()
        .HasOne(a => a.RoyaltyLevel)
        .WithOne(b => b.Institute)
        .HasForeignKey<RoyaltyLevel>(b => b.Id);
    }
}