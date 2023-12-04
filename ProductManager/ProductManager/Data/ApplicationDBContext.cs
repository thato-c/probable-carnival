using Microsoft.EntityFrameworkCore;
using ProductManager.Models;

namespace ProductManager.Data
{
    public class ApplicationDBContext:DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) 
        { 
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Licence> Licences { get; set; }
        public DbSet<LicencePurchase> LicencePurchases { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>().ToTable("Company");
            modelBuilder.Entity<Licence>().ToTable("Licence");
            modelBuilder.Entity<LicencePurchase>().ToTable("LicencePurchase");
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<UserRole>().ToTable("UserRole");
            modelBuilder.Entity<Role>().ToTable("Role");

            // Configure the relationship between Licence and LicencePurchase
            modelBuilder.Entity<Licence>()
                .HasMany(licence => licence.LicencePurchases)
                .WithOne(purchase => purchase.Licence)
                .HasForeignKey(purchase => purchase.LicenceId);

            // Configure the relationship between Company and LicencePurchase
            modelBuilder.Entity<Company>()
                .HasMany(company => company.LicencePurchases)
                .WithOne(purchase => purchase.Company)
                .HasForeignKey(purchase => purchase.CompanyId);

            // Configure the relationship between Company and User
            modelBuilder.Entity<Company>()
                .HasMany(company => company.Users)
                .WithOne(user => user.Company)
                .HasForeignKey(user => user.CompanyId);

            // Configure the relationship between User and UserRole
            modelBuilder.Entity<User>()
                .HasMany(user => user.UserRoles)
                .WithOne(userRole => userRole.User)
                .HasForeignKey(userRole => userRole.UserId);

            // Configure the relationship between Role and UserRole
            modelBuilder.Entity<Role>()
                .HasMany(role => role.UserRoles)
                .WithOne(userRole => userRole.Role)
                .HasForeignKey(userRole => userRole.RoleId);

            
        }
    }
}
