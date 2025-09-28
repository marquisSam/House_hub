using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using HouseHub.Models;

namespace HouseHub.AppDataContext
{

    // ItemDbContext class inherits from DbContext
     public class ItemDbContext : DbContext
     {

        // DbSettings field to store the connection string
         private readonly DbSettings _dbsettings;

         // Constructor to inject the DbSettings model
         public ItemDbContext(IOptions<DbSettings> dbSettings)
         {
             _dbsettings = dbSettings.Value;
         }

         // Constructor for Entity Framework with DbContextOptions
         public ItemDbContext(DbContextOptions<ItemDbContext> options, IOptions<DbSettings> dbSettings) : base(options)
         {
             _dbsettings = dbSettings.Value;
         }



        // DbSet property to represent the Todo table
        public DbSet<Todo> Todos { get; set; }

        // DbSet property to represent the Event table
        public DbSet<Event> Events { get; set; }

        // DbSet property to represent the User table
        public DbSet<User> Users { get; set; }

        // DbSet property to represent the TodoUser join table
        public DbSet<TodoUser> TodoUsers { get; set; }


         // Configuring the database provider and connection string

         protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
         {
             optionsBuilder.UseNpgsql(_dbsettings.ConnectionString);
         }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuring the model for the Todo entity
            modelBuilder.Entity<Todo>()
                .ToTable("TodosAPI")
                .HasKey(x => x.Id);

            // Configuring the model for the Event entity
            modelBuilder.Entity<Event>()
                .ToTable("EventsAPI")
                .HasKey(x => x.Id);

            // Configuring the model for the User entity
            modelBuilder.Entity<User>()
                .ToTable("UsersAPI")
                .HasKey(x => x.Id);

            // Ensure email uniqueness
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Configure TodoUser join table
            modelBuilder.Entity<TodoUser>()
                .ToTable("TodoUsersAPI")
                .HasKey(tu => tu.Id);

            // Configure many-to-many relationship through TodoUser
            modelBuilder.Entity<TodoUser>()
                .HasOne(tu => tu.Todo)
                .WithMany(t => t.TodoUsers)
                .HasForeignKey(tu => tu.TodoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TodoUser>()
                .HasOne(tu => tu.User)
                .WithMany(u => u.TodoUsers)
                .HasForeignKey(tu => tu.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ensure unique combination of TodoId and UserId
            modelBuilder.Entity<TodoUser>()
                .HasIndex(tu => new { tu.TodoId, tu.UserId })
                .IsUnique();

            // Configure many-to-many navigation (for convenience)
            modelBuilder.Entity<Todo>()
                .HasMany(t => t.Users)
                .WithMany(u => u.Todos)
                .UsingEntity<TodoUser>();
        }
     }
}