using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
namespace ToDo.Data.Entities
{
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public virtual DbSet<ToDo> ToDoItems { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ToDo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(p => p.Id).ValueGeneratedOnAdd();
                entity.HasOne(d => d.User).WithMany(p => p.ToDos)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(p => p.Id).ValueGeneratedOnAdd();
            });




            modelBuilder.Entity<Users>().HasData(
                new Users
                {
                    Id = 1,
                    Username = "Admin",
                    Email ="nguyenquangquyX@gmail.com",
                    FullName = "Ena Nguyễn",
                    Role ="Admin",
                    Password = "8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92",
                    Status =1,
                    TwoFA = false,
                    SecretKey = string.Empty,
                }
            );
            modelBuilder.Entity<ToDo>().HasData(
                new ToDo
                {
                    Id = 1,
                    Title = "Automapper and FluentValidation",
                    Description = "Learning before Friday",
                    IsCompleted = false,
                    CreatedAt = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(2),
                    UserId = 1,
                },
                new ToDo
                {
                    Id = 2,
                    Title = "Fundamental Minimal API",
                    Description = "Learning before Sunday",
                    IsCompleted = false,
                    CreatedAt = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(4),
                    UserId = 1,
                }
            );
        }

    }
}
