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

        public DbSet<Project> Projects { get; set; }

        public DbSet<UserProjectAssignment> UserProjectsAssignments { get; set; }

        public DbSet<UserProjectRole> UserProjectRoles { get; set; }

        public DbSet<Document> Documents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>().ToTable("Company");
            modelBuilder.Entity<Licence>().ToTable("Licence");
            modelBuilder.Entity<LicencePurchase>().ToTable("LicencePurchase");
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<UserRole>().ToTable("UserRole");
            modelBuilder.Entity<Role>().ToTable("Role");
            modelBuilder.Entity<UserProjectRole>().ToTable("UserProjectRole");
            modelBuilder.Entity<Document>().ToTable("Document");

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

            // Configure the relationship between Company and Project
            modelBuilder.Entity<Company>()
                .HasMany (company => company.Projects)
                .WithOne(project => project.Company)
                .HasForeignKey (project => project.CompanyId);

            // Configure the relationship between Project and UserProjectAssignment
            modelBuilder.Entity<Project>()
                .HasMany(project => project.UserProjectAssignments)
                .WithOne(userProjectAssignment => userProjectAssignment.Project)
                .HasForeignKey(userProjectAssignment => userProjectAssignment.ProjectId);

            // Configure the relationship between User and UserProjectAssignment
            modelBuilder.Entity<User>()
                .HasMany(user => user.UserProjectAssignments)
                .WithOne(userProjectAssignment => userProjectAssignment.User)
                .HasForeignKey(userProjectAssignment => userProjectAssignment.UserId);

            // Configure the relationship between Role and UserProjectRole
            modelBuilder.Entity<Role>()
                .HasMany(role => role.UserProjectRoles)
                .WithOne(userProjectRole => userProjectRole.Role)
                .HasForeignKey(UserProjectRole => UserProjectRole.RoleId);

            // Configure the relationship between UserProjectAssignment and UserProjectRole
            modelBuilder.Entity<UserProjectAssignment>()
                .HasMany(assignment => assignment.UserProjectRoles)
                .WithOne(userProjectRole => userProjectRole.UserProjectAssignment)
                .HasForeignKey(UserProjectRole => UserProjectRole.AssignmentId);

            // Configure the relationship between Project and Document
            modelBuilder.Entity<Project>()
                .HasMany(project => project.Documents)
                .WithOne(document => document.Project)
                .HasForeignKey(doccument => doccument.ProjectId);
        }
    }
}
