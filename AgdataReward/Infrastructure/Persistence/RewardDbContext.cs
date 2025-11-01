using Domain.Entities.Event;
using Domain.Entities.Product;
using Domain.Entities.Redemption;
using Domain.Entities.Reward;
using Domain.Entities.User;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;

namespace Infrastructure.Persistence
{
    public class RewardDbContext : DbContext
    {
        public RewardDbContext(DbContextOptions<RewardDbContext> options) : base(options) { }

        // Users
        public DbSet<UserProfile> UserProfiles { get; set; } = null!;
        public DbSet<UserAccount> UserAccounts { get; set; } = null!;

        // Rewards & Transactions
        public DbSet<RewardPoints> RewardPoints { get; set; } = null!;
        public DbSet<RewardTransaction> RewardTransactions { get; set; } = null!;

        // Events
        public DbSet<EventDefinition> EventDefinitions { get; set; } = null!;
        public DbSet<EventInstance> EventInstances { get; set; } = null!;
        public DbSet<EventRewardRule> EventRewardRules { get; set; } = null!;

        // Products & Inventory
        public DbSet<ProductInformation> ProductInformations { get; set; } = null!;
        public DbSet<ProductInventory> ProductInventories { get; set; } = null!;

        // Redemptions
        public DbSet<RedemptionRecord> RedemptionRecords { get; set; } = null!;
        public DbSet<RedemptionRequest> RedemptionRequests { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ====================
            // UserProfile
            // ====================
            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.ToTable("UserProfiles");
                entity.HasKey(u => u.Id);

                entity.Property(u => u.EmployeeId)
                       .HasConversion(
                           eid => eid.Value,        // Store the string in DB
                           str => new EmployeeId(str) // Convert back to value object
                       )
                       .IsRequired()
                       .HasMaxLength(50);

                entity.HasIndex(u => u.EmployeeId).IsUnique();

                entity.Property(u => u.FirstName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(u => u.LastName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(u => u.Email)
                      .HasConversion(
                          e => e.Value,            // Store string in DB
                          str => new Email(str)    // Convert back to Email VO
                      )
                      .IsRequired()
                      .HasMaxLength(255);

                entity.HasIndex(u => u.Email).IsUnique();

                // Role stored as int
                entity.Property(u => u.Role)
                      .HasConversion<int>()
                      .IsRequired();

                // One-to-one relation configured on UserAccount side (FK: UserAccount.UserId)
                entity.Navigation(u => u.Account).AutoInclude(false);
            });

            // ====================
            // UserAccount
            // ====================
            modelBuilder.Entity<UserAccount>(entity =>
            {
                entity.ToTable("UserAccounts");
                entity.HasKey(a => a.Id);

                // UserId is FK to UserProfile.Id (one-to-one)
                entity.Property(a => a.UserId).IsRequired();

                entity.Property(a => a.RewardBalance)
                      .IsRequired()
                      .HasDefaultValue(0);

                entity.Property(a => a.Status)
                      .HasConversion<int>()
                      .IsRequired()
                      .HasDefaultValue(AccountStatus.Active);

                // One-to-one FK
                entity.HasOne(a => a.User)
                      .WithOne(u => u.Account)
                      .HasForeignKey<UserAccount>(a => a.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Backing field for transactions: private readonly List<RewardTransaction> _transactions
                // Configure one-to-many for transactions using UserAccount => RewardTransaction (FK on RewardTransaction.UserId)
                entity.HasMany(a => a.Transactions)
                      .WithOne(t => t.UserAccount)
                      .HasForeignKey(t => t.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Metadata.FindNavigation(nameof(UserAccount.Transactions))
                      ?.SetField("_transactions");

            });

            // ====================
            // RewardPoints
            // ====================
            modelBuilder.Entity<RewardPoints>(entity =>
            {
                entity.ToTable("RewardPoints");
                entity.HasKey(rp => rp.Id);

                entity.Property(rp => rp.PointsValue)
                      .IsRequired();
            });

            // ====================
            // RewardTransaction
            // ====================
            modelBuilder.Entity<RewardTransaction>(entity =>
            {
                entity.ToTable("RewardTransactions");
                entity.HasKey(t => t.Id);

                // Points delta required (positive or negative)
                entity.Property(t => t.PointsDelta)
                      .IsRequired();

                entity.Property(t => t.Notes)
                      .IsRequired()
                      .HasMaxLength(250);

                entity.Property(t => t.TransactionType)
                      .HasConversion<int>()
                      .IsRequired();

                entity.Property(t => t.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                // Relationship to UserAccount (UserId FK on RewardTransaction maps to UserAccount.Id)
                //entity.Property(t => t.UserId).IsRequired();
                //entity.HasOne(t => t.UserAccount)
                //      .WithMany("_transactions")
                //      .HasForeignKey(t => t.UserId)
                //      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(t => t.UserId).IsRequired();

                // Optional relation to EventInstance
                entity.HasOne(t => t.EventInstance)
                      .WithMany()
                      .HasForeignKey(t => t.EventId)
                      .OnDelete(DeleteBehavior.SetNull);

                // Optional relation to RedemptionRequest
                entity.HasOne(t => t.RedemptionRequest)
                      .WithMany()
                      .HasForeignKey(t => t.RedemptionId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // ====================
            // Events: Definition, Instance, RewardRule
            // ====================
            modelBuilder.Entity<EventDefinition>(entity =>
            {
                entity.ToTable("EventDefinitions");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Code)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.HasIndex(e => e.Code).IsUnique();

                entity.Property(e => e.Title)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Navigation(e => e.Instances).UsePropertyAccessMode(PropertyAccessMode.Field);
                entity.Navigation(e => e.RewardRules).UsePropertyAccessMode(PropertyAccessMode.Field);
            });

            modelBuilder.Entity<EventInstance>(entity =>
            {
                entity.ToTable("EventInstances");
                entity.HasKey(ei => ei.Id);

                entity.Property(ei => ei.EventId).IsRequired();
                entity.Property(ei => ei.Rank);

                entity.HasOne(ei => ei.Event)
                      .WithMany(ed => ed.Instances)
                      .HasForeignKey(ei => ei.EventId)
                      .OnDelete(DeleteBehavior.Cascade);

                // WinnerUser is optional; if the winner user is deleted, set WinnerUserId = null
                entity.HasOne(ei => ei.WinnerUser)
                      .WithMany()
                      .HasForeignKey(ei => ei.WinnerUserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<EventRewardRule>(entity =>
            {
                entity.ToTable("EventRewardRules");
                entity.HasKey(r => r.Id);

                entity.Property(r => r.EventId).IsRequired();
                entity.Property(r => r.Rank).IsRequired();

                entity.HasOne(r => r.Event)
                      .WithMany(ed => ed.RewardRules)
                      .HasForeignKey(r => r.EventId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.RewardPoints)
                      .WithMany()
                      .HasForeignKey(r => r.RewardPointsId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ====================
            // Products & Inventory
            // ====================
            modelBuilder.Entity<ProductInformation>(entity =>
            {
                entity.ToTable("ProductInformations");
                entity.HasKey(p => p.Id);

                entity.Property(p => p.SKU)
                      .HasConversion(
                          sku => sku.Value,   // store SKU as string
                          str => new SKU(str) // materialize SKU from string
                      )
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(p => p.Name)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(p => p.RewardPointsId).IsRequired();

                entity.HasOne(p => p.RewardPoints)
                      .WithMany()
                      .HasForeignKey(p => p.RewardPointsId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ProductInventory>(entity =>
            {
                entity.ToTable("ProductInventories");
                entity.HasKey(pi => pi.Id);

                entity.Property(pi => pi.ProductId).IsRequired();
                entity.Property(pi => pi.StockQuantity).IsRequired();
                entity.Property(pi => pi.IsActive).IsRequired();

                // One-to-one (or one-to-many depending on your design). We'll treat inventory entry as one-to-one per product:
                entity.HasOne(pi => pi.Product)
                      .WithMany()
                      .HasForeignKey(pi => pi.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ====================
            // Redemptions
            // ====================
            modelBuilder.Entity<RedemptionRecord>(entity =>
            {
                entity.ToTable("RedemptionRecords");
                entity.HasKey(rr => rr.Id);

                entity.Property(rr => rr.UserId).IsRequired();
                entity.Property(rr => rr.ProductId).IsRequired();
                entity.Property(rr => rr.RedeemedAt).IsRequired();

                entity.HasOne(rr => rr.User)
                      .WithMany()
                      .HasForeignKey(rr => rr.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rr => rr.Product)
                      .WithMany()
                      .HasForeignKey(rr => rr.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<RedemptionRequest>(entity =>
            {
                entity.ToTable("RedemptionRequests");
                entity.HasKey(rrq => rrq.Id);

                entity.Property(rrq => rrq.RedemptionId).IsRequired();
                entity.Property(rrq => rrq.PointsUsed).IsRequired();
                entity.Property(rrq => rrq.Status)
                      .HasConversion<int>()
                      .IsRequired();
                entity.Property(rrq => rrq.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                // Link to RedemptionRecord (RedemptionId FK)
                entity.HasOne<RedemptionRecord>()
                      .WithMany()
                      .HasForeignKey(rrq => rrq.RedemptionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ====================
            // Generic configurations / conventions
            // ====================
            // Ensure GUIDs are required where appropriate:
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Convert CLR enum properties to int by default (we've already handled many explicitly).
                foreach (var prop in entityType.GetProperties().Where(p => p.ClrType.IsEnum))
                {
                    prop.SetColumnType("int");
                }
            }
        }
    }
}
