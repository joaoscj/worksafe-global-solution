// dotnet ef migrations add InitialCreate
// dotnet ef database update
// Comandos para executar migrations

using HabitFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HabitFlow.Infrastructure.Migrations;

/// <summary>
/// Esta é a migration inicial que cria todas as tabelas do banco de dados
/// 
/// Passos para executar:
/// 1. Abra o Package Manager Console
/// 2. Execute: dotnet ef migrations add InitialCreate
/// 3. Execute: dotnet ef database update
/// 
/// Alternativamente, use o CLI:
/// dotnet ef migrations add InitialCreate
/// dotnet ef database update
/// </summary>
public static class MigrationExtensions
{
    public static void ApplyMigrations(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<HabitFlowContext>();
            dbContext.Database.Migrate();
        }
    }
}
