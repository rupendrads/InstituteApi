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
}