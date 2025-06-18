using GlassLewis.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GlassLewis.Infrastructure.Data.Contexts;

/// <summary>
/// Represents the database context for managing Company entities.
/// </summary>
public class CompanyDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompanyDbContext"/> class with the specified options.
    /// </summary>
    /// <param name="options">The options to configure the database context.</param>
    public CompanyDbContext(DbContextOptions<CompanyDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the DbSet for Company entities.
    /// </summary>
    public virtual DbSet<Company> Companies { get; set; }

    /// <summary>
    /// Configures the model for the CompanyDbContext.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to configure the entity mappings.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Company>(entity =>
        {
            // Configure unique constraint on ISIN
            entity.HasIndex(e => e.Isin)
                  .IsUnique()
                  .HasDatabaseName("IX_Companies_Isin");

            // Configure indexes for performance
            entity.HasIndex(e => e.Name)
                  .HasDatabaseName("IX_Companies_Name");

            entity.HasIndex(e => e.StockTicker)
                  .HasDatabaseName("IX_Companies_StockTicker");

            // Configure properties
            entity.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(e => e.StockTicker)
                  .IsRequired()
                  .HasMaxLength(10);

            entity.Property(e => e.Exchange)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(e => e.Isin)
                  .IsRequired()
                  .HasMaxLength(12)
                  .IsFixedLength();

            entity.Property(e => e.Website)
                  .HasMaxLength(500);

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("GETUTCDATE()");
        });

        // Seed sample data
        modelBuilder.Entity<Company>().HasData(
            new Company
            {
                Id = new Guid("8973d1e2-3020-49db-862b-2d454746b42d"),
                Name = "Apple Inc.",
                Exchange = "NASDAQ",
                StockTicker = "AAPL",
                Isin = "US0378331005",
                Website = "http://www.apple.com",
                CreatedAt = new DateTime(2025, 06, 05),
                UpdatedAt = new DateTime(2025, 06, 05)
            },
            new Company
            {
                Id = new Guid("16511881-10fd-4772-9b62-f1b78513d8af"),
                Name = "British Airways Plc",
                Exchange = "Pink Sheets",
                StockTicker = "BAIRY",
                Isin = "US1104193065",
                CreatedAt = new DateTime(2025, 06, 05),
                UpdatedAt = new DateTime(2025, 06, 05)
            },
            new Company
            {
                Id = new Guid("e8146538-3991-4877-9b76-f276b9849d45"),
                Name = "Heineken NV",
                Exchange = "Euronext Amsterdam",
                StockTicker = "HEIA",
                Isin = "NL0000009165",
                CreatedAt = new DateTime(2025, 06, 05),
                UpdatedAt = new DateTime(2025, 06, 05)
            }
        );
    }
}
