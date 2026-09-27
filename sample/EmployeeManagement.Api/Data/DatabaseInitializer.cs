using EmployeeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Data;

public class DatabaseInitializer(AppDbContext dbContext, IConfiguration configuration)
{
    public async Task InitializeAsync()
    {
        await dbContext.Database.MigrateAsync();

        await SeedUserAsync("DemoAdmin", "Admin");
        await SeedUserAsync("DemoViewer", "Viewer");
        await dbContext.SaveChangesAsync();
    }

    private async Task SeedUserAsync(string configurationSection, string role)
    {
        var username = configuration[$"{configurationSection}:Username"];
        var password = configuration[$"{configurationSection}:Password"];
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return;
        if (await dbContext.AppUsers.AnyAsync(user => user.Username == username))
            return;

        dbContext.AppUsers.Add(new AppUser
        {
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = role
        });
    }
}
