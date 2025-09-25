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



        // DbSet property to represent the Todo table
        public DbSet<Todo> Todos { get; set; }

        // DbSet property to represent the Event table
        public DbSet<Event> Events { get; set; }

        // DbSet property to represent the User table
        public DbSet<User> Users { get; set; }


         // Configuring the database provider and connection string

         protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
         {
             optionsBuilder.UseSqlServer(_dbsettings.ConnectionString);
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
        }
     }
}