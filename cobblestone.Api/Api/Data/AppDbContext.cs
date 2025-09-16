using Microsoft.EntityFrameworkCore;
using Api.Domain;

namespace Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Block> Blocks => Set<Block>();
    public DbSet<HarvestPlan> HarvestPlans => Set<HarvestPlan>();
    public DbSet<HarvestContractor> HarvestContractors => Set<HarvestContractor>();
    public DbSet<Pool> Pools => Set<Pool>();
    public DbSet<CommodityClass> Commodities => Set<CommodityClass>();
    public DbSet<ScoutReportWithBlock> ScoutReportsWithBlock => Set<ScoutReportWithBlock>();
    public DbSet<ProcessProductionRun> ProcessProductionRuns => Set<ProcessProductionRun>();
    public DbSet<PlaceholderGrower> PlaceholderGrowers => Set<PlaceholderGrower>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<HarvestContractor>(b =>
        {
            b.ToTable("HarvestContractors", "dbo");   // <— change table name if different
            b.HasKey(x => x.id);
            b.Property(x => x.id).ValueGeneratedOnAdd();   // identity(bigint)

            b.Property(x => x.name)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false); // varchar

            b.Property(x => x.primary_contact_name).HasMaxLength(50).IsUnicode(false);
            b.Property(x => x.primary_contact_phone).HasMaxLength(20).IsUnicode(false);
            b.Property(x => x.office_phone).HasMaxLength(20).IsUnicode(false);
            b.Property(x => x.mailing_address).HasMaxLength(100).IsUnicode(false);

            // bit columns map to bool/bool? automatically
        });

        modelBuilder.Entity<HarvestPlan>(b =>
        {
            b.ToTable("HarvestPlanEntry", "dbo"); // <- change if your table name differs
            b.HasKey(x => x.id);


            b.Property(x => x.grower_block_source_database)
            .HasMaxLength(20)
            .IsUnicode(true);

            b.Property(x => x.placeholder_grower_id);

            b.Property(x => x.deliver_to).HasMaxLength(30).IsUnicode(false);
            b.Property(x => x.packed_by).HasMaxLength(30).IsUnicode(false);
            b.Property(x => x.notes_general).IsUnicode(false); // varchar(MAX)


            // precise numeric types
            b.Property(x => x.harvesting_rate).HasColumnType("decimal(18,4)");
            b.Property(x => x.hauling_rate).HasColumnType("decimal(18,4)");
            b.Property(x => x.forklift_rate).HasColumnType("decimal(18,4)");


            // optional FKs (no relationships yet)
            b.Property(x => x.contractor_id);
            b.Property(x => x.hauler_id);
            b.Property(x => x.forklift_contractor_id);


            // If your column name is literally "date", map explicitly to avoid provider-specific issues
            b.Property(x => x.date).HasColumnName("date").HasColumnType("date");
        });
        modelBuilder.Entity<Pool>(b =>
        {
            b.ToTable("Pools", "dbo");
            b.HasKey(x => x.POOLIDX);

            b.Property(x => x.ID).HasMaxLength(12).IsUnicode(true);
            b.Property(x => x.DESCR).HasMaxLength(40).IsUnicode(true);

            b.Property(x => x.ICCLOSEDFLAG).HasMaxLength(1).IsFixedLength();
            b.Property(x => x.POOLTYPE).HasMaxLength(1).IsFixedLength();

            b.Property(x => x.source_database).HasMaxLength(20).IsUnicode(true);
            b.Property(x => x.SyncStatus).HasMaxLength(1).IsFixedLength().IsUnicode(true);

            b.Property(x => x.RowVer).IsRowVersion().IsConcurrencyToken();
        });
        modelBuilder.Entity<CommodityClass>(b =>
        {
            b.ToTable("Commodity", "dbo");
            b.Property(x => x.source_database);
            b.HasKey(x => x.CommodityIDx);

            b.Property(x => x.InvoiceCommodity).HasMaxLength(50).IsUnicode(true);
            b.Property(x => x.Commodity).HasMaxLength(50).IsUnicode(true);
        });
        modelBuilder.Entity<ScoutReportWithBlock>(entity =>
            {
                entity.HasNoKey(); // keyless entity type because it's a view
                entity.ToView("VW_ScoutReportWithBlock", "dbo");

                entity.Property(e => e.ScoutReportId).HasColumnName("ScoutReportId");
                entity.Property(e => e.BlockId).HasColumnName("BlockId");
            });

        modelBuilder.Entity<ProcessProductionRun>(b =>
        {
            b.ToTable("ProcessProductionRuns", "dbo");
            b.HasKey(x => x.id);

            b.Property(x => x.source_database)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            b.Property(x => x.location)
                .HasMaxLength(100)
                .IsUnicode(false);

            b.Property(x => x.pool)
                .HasMaxLength(50)
                .IsUnicode(false);

            b.Property(x => x.notes)
                .IsUnicode(false);

            b.Property(x => x.run_status)
                .HasMaxLength(50)
                .IsUnicode(false);

            b.Property(x => x.batch_id)
                .HasMaxLength(50)
                .IsUnicode(false);

            // Configure date columns
            b.Property(x => x.run_date).HasColumnType("date");
            b.Property(x => x.pick_date).HasColumnType("date");
            b.Property(x => x.time_started).HasColumnType("datetime2");
            b.Property(x => x.time_completed).HasColumnType("datetime2");

            // Composite foreign key to Block
            b.HasOne(x => x.Block)
                .WithMany()
                .HasForeignKey(x => new { x.source_database, x.GABLOCKIDX })
                .HasPrincipalKey(x => new { x.source_database, x.GABLOCKIDX })
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            b.HasIndex(x => new { x.source_database, x.GABLOCKIDX });
            b.HasIndex(x => x.run_date);
            b.HasIndex(x => x.pick_date);
            b.HasIndex(x => x.run_status);
            b.HasIndex(x => x.batch_id);
        });

        // PlaceholderGrower configuration
        modelBuilder.Entity<PlaceholderGrower>(b =>
        {
            b.ToTable("PlaceholderGrowers", "dbo");
            b.HasKey(x => x.id);
            b.Property(x => x.grower_name)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            b.Property(x => x.commodity_name)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            b.Property(x => x.created_at)
                .IsRequired()
                .HasColumnType("datetime2");
            b.Property(x => x.updated_at)
                .HasColumnType("datetime2");
            b.Property(x => x.is_active)
                .IsRequired()
                .HasDefaultValue(true);
            b.Property(x => x.notes)
                .HasMaxLength(500)
                .IsUnicode(false);

            // Indexes
            b.HasIndex(x => x.grower_name);
            b.HasIndex(x => x.commodity_name);
            b.HasIndex(x => x.is_active);
            b.HasIndex(x => x.created_at);
        });
    }
}
