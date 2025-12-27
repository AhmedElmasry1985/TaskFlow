using Microsoft.EntityFrameworkCore;
using TasksApi.Data;

namespace TasksApi.Migrations;

public static class Migrator
{
    public static void Migrate(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var pendingMigrations = db.Database.GetPendingMigrations().ToArray();
        if (pendingMigrations.Length == 0)
        {
            Console.WriteLine("--> No pending migrations");
            return;
        }

        var isMigrated = false;
        var trials = 0;
        var error = string.Empty;
        var maxTrials = 5;
        while (trials < maxTrials)
            try
            {
                db.Database.Migrate();
                isMigrated = true;
                break;
            }
            catch (Exception ex)
            {
                trials++;
                if (trials == maxTrials)
                    error = ex.Message;
            }

        Console.WriteLine(isMigrated
            ? "--> Migrations applied successfully"
            : $"--> Migration failed: {error}");
    }
}