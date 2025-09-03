using Microsoft.EntityFrameworkCore;
using TODOListServer.Models;

namespace TODOListServer.Data
{
    public class AppDbContext : DbContext
    {
        // Конструктор, принимающий DbContextOptions
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Category > Categories { get; set; } // Таблица категорий
        public DbSet<TaskItem> Tasks { get; set; } //Таблица задач

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=TODOListRemastered_BaseData.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskItem>()
                .HasOne<Category>(t => t.Category)
                .WithMany()
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
