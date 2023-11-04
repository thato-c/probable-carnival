using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManager.Controllers;
using ProductManager.Data;
using ProductManager.Models;
using ProductManager.ViewModels;

public class LicenceControllerTests : IDisposable
{
    private readonly DbContextOptions<ApplicationDBContext> _options;

    public LicenceControllerTests()
    {
        // Configure DbContextOptions with the SQL Server connection string for your test database
        _options = new DbContextOptionsBuilder<ApplicationDBContext>()
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ProductManager;Trusted_Connection=True;MultipleActiveResultSets=true")
            .Options;

        using (var context = new ApplicationDBContext(_options))
        {
            context.Database.Migrate(); // Ensure the test database is up to date
        }
    }

    public void Dispose()
    {
        // Cleanup: Delete the test database after running the tests
        using (var context = new ApplicationDBContext(_options))
        {
            context.Database.EnsureDeleted();
        }
    }

    [Fact]
    public async Task Index_ReturnsViewWithLicences()
    {
        using (var context = new ApplicationDBContext(_options))
        {
            // Arrange
            // Add sample data to the actual database
            context.Licences.Add(new Licence
            {
                Name = "Sample Licence 1",
                Description = "Description 1",
                Cost = 10,
                ValidityMonths = 1,
            });

            context.Licences.Add(new Licence
            {
                Name = "Sample Licence 2",
                Description = "Description 2",
                Cost = 10,
                ValidityMonths = 6,
            });

            context.SaveChanges();
        }

        using (var context = new ApplicationDBContext(_options))
        {
            var controller = new LicenceController(context);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Licence>>(viewResult.Model);

            Assert.NotEmpty(model);
            Assert.Null(viewResult.ViewData["Message"]);
        }
    }

    [Fact]
    public void Create_ReturnsViewResult()
    {
        using (var context = new ApplicationDBContext(_options))
        {
            // Add any necessary test data to the in-memory database
            context.Licences.Add(new Licence
            {
                Name = "Sample License",
                Description = "Test Description",
                Cost = 10,
                ValidityMonths = 6,
            });
            context.SaveChanges();
        }

        using (var context = new ApplicationDBContext(_options))
        {
            var controller = new LicenceController(context);

            // Act
            var result = controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            // Add any additional assertions as needed
        }
    }

    [Fact]
    public async Task Create_Post_RedirectsToIndexIfModelIsValid()
    {
        using (var context = new ApplicationDBContext(_options))
        {
            // Arrange
            var controller = new LicenceController(context);

            // Create a test model
            var model = new LicenceViewModel
            {
                Name = "Test Licence",
                Description = "Test Description",
                Cost = 10,
                ValidityMonths = 6
            };

            // Act: Simulate a POST request with the model
            var result = await controller.Create(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            // Check if the Licence was added to the database
            var licences = await context.Licences.ToListAsync();
            Assert.Single(licences); // There should be one licence in the database
            var licence = licences[0];
            Assert.Equal(model.Name, licence.Name);
            Assert.Equal(model.Description, licence.Description);
            Assert.Equal(model.Cost, licence.Cost);
            Assert.Equal(model.ValidityMonths, licence.ValidityMonths);
        }
    }

    [Fact]
    public async Task Create_Post_ReturnsViewIfModelIsInvalid()
    {
        using (var context = new ApplicationDBContext(_options))
        {
            // Arrange
            var controller = new LicenceController(context);

            // Create an invalid model (e.g., empty or missing required fields)
            var model = new LicenceViewModel();

            // Add model state errors to simulate an invalid model
            controller.ModelState.AddModelError("Name", "Name is required");

            // Act: Simulate a POST request with the invalid model
            var result = await controller.Create(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName); // Ensure it returns the default view
        }
    }
}