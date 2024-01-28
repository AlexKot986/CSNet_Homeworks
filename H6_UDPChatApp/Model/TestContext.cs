using Microsoft.EntityFrameworkCore;
using System.Threading.Channels;

namespace H6_UDPChatApp.Model
{
    public class TestContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Message> Messages { get; set; }

        public TestContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder//.LogTo(Console.WriteLine)
                          .UseLazyLoadingProxies()
                          .UseNpgsql("Host=localhost;Port=5433;Username=postgres;Password=example;Database=ChatDb2");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("users_pkey");
                entity.ToTable("users");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name)
                      .HasMaxLength(255)
                      .HasColumnName("name");

            });
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(x => x.Id).HasName("message_pkey");
                entity.ToTable("messages");
                entity.Property(x => x.Id).HasColumnName("id");
                entity.Property(x => x.Text).HasColumnName("text");
                entity.Property(e => e.FromUserId).HasColumnName("from_user_id");
                entity.Property(e => e.ToUserId).HasColumnName("to_user_id");

                entity.HasOne(d => d.FromUser)
                      .WithMany(p => p.FromMessages)
                      .HasForeignKey(d => d.FromUserId)
                      .HasConstraintName("messages_from_user_id_fkey");

                entity.HasOne(d => d.ToUser)
                      .WithMany(p => p.ToMessages)
                      .HasForeignKey(d => d.ToUserId)
                      .HasConstraintName("messages_to_user_id_fkey");

            });
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
