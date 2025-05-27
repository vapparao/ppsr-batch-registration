using Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<BatchOperation> BatchOperations { get; set; }
    public DbSet<Grantor> Grantors { get; set; }
    public DbSet<InvalidRecord> InvalidRecords { get; set; }
    public DbSet<Registration> Registrations { get; set; }
    public DbSet<Spg> Spgs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BatchOperation>(entity =>
        {
            entity.HasKey(e => e.BatchId);
            entity.Property(e => e.FileName).IsRequired();

            entity.Property(e => e.Status)
                  .IsRequired()
                  .HasConversion<string>() 
                  .HasMaxLength(20);

            entity.ToTable(tb =>
            {
                tb.HasCheckConstraint(
                    "CK_batch_operation_status",
                    "status IN ('PROCESSING', 'COMPLETED', 'FAILED')");
            });


            entity.Property(e => e.FileChecksum).IsRequired();

            entity.HasMany(e => e.Registration)
                 .WithOne(e => e.Batch)
                 .HasForeignKey(e => e.BatchId)
                 .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Grantor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.MiddleNames).HasMaxLength(200);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<InvalidRecord>(entity =>
        {
            entity.HasKey(e => e.InvalidRecordId);
            entity.Property(e => e.RecordData).HasColumnType("nvarchar(max)");
            entity.Property(e => e.ErrorMessages).HasColumnType("nvarchar(max)");

            entity.HasOne(e => e.BatchOperation)
                 .WithMany()
                 .HasForeignKey(e => e.BatchId)
                 .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Registration>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Vin).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Duration).IsRequired().HasMaxLength(50);

            entity.HasOne(e => e.Grantor)
                 .WithMany()
                 .HasForeignKey(e => e.GrantorId)
                 .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Spg)
                 .WithMany()
                 .HasForeignKey(e => e.SpgId)
                 .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Spg>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Acn).IsRequired().HasMaxLength(50);
            entity.Property(e => e.OrganizationName).IsRequired().HasMaxLength(200);
        });
    }
}