using HabitFlow.Domain.Entities;

namespace HabitFlow.Infrastructure.Data;

public static class DataSeeder
{
    public static void SeedData(this HabitFlowContext context)
    {
        if (context.Users.Any())
            return;

        var user1 = new User
        {
            Name = "João Silva",
            Email = "joao.silva@email.com",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var user2 = new User
        {
            Name = "Maria Santos",
            Email = "maria.santos@email.com",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Users.AddRange(user1, user2);
        context.SaveChanges();

        var healthChecks = new List<HealthCheck>
        {
            new()
            {
                UserId = user1.Id,
                StressLevel = 7,
                SleepQuality = 6,
                JobSatisfaction = 7,
                MentalHealth = 7,
                PhysicalHealth = 8,
                Notes = "Bom dia, me sinto relativamente bem",
                CheckedAt = DateTime.UtcNow.AddDays(-10)
            },
            new()
            {
                UserId = user1.Id,
                StressLevel = 8,
                SleepQuality = 4,
                JobSatisfaction = 5,
                MentalHealth = 6,
                PhysicalHealth = 7,
                Notes = "Noite agitada, preocupações com projeto",
                CheckedAt = DateTime.UtcNow.AddDays(-5)
            },
            new()
            {
                UserId = user1.Id,
                StressLevel = 6,
                SleepQuality = 7,
                JobSatisfaction = 8,
                MentalHealth = 8,
                PhysicalHealth = 8,
                Notes = "Fim de semana descansou bem",
                CheckedAt = DateTime.UtcNow
            },
            new()
            {
                UserId = user2.Id,
                StressLevel = 5,
                SleepQuality = 8,
                JobSatisfaction = 8,
                MentalHealth = 8,
                PhysicalHealth = 9,
                Notes = "Me sinto ótimo esta semana",
                CheckedAt = DateTime.UtcNow
            }
        };

        context.HealthChecks.AddRange(healthChecks);
        context.SaveChanges();

        var tips = new List<WellnessTip>
        {
            new()
            {
                Title = "Técnica Pomodoro",
                Description = "Trabalhe em blocos de 25 minutos com 5 minutos de pausa. A cada 4 ciclos, faça uma pausa de 15 minutos.",
                Category = "Produtividade",
                MinWellnessScore = 0,
                MaxWellnessScore = 100
            },
            new()
            {
                Title = "Respire Profundamente",
                Description = "Faça respiração profunda por 5 minutos. Inspire por 4 segundos, segure por 4, e expire por 6. Reduz estresse imediatamente.",
                Category = "Estresse",
                MinWellnessScore = 0,
                MaxWellnessScore = 50
            },
            new()
            {
                Title = "Hidratação Adequada",
                Description = "Beba pelo menos 2 litros de água por dia. A desidratação afeta energia e humor.",
                Category = "Saúde Física",
                MinWellnessScore = 0,
                MaxWellnessScore = 100
            },
            new()
            {
                Title = "Exercício Diário",
                Description = "Pratique 30 minutos de atividade física diária. Pode ser caminhada, corrida, ou alongamento.",
                Category = "Saúde Física",
                MinWellnessScore = 0,
                MaxWellnessScore = 100
            },
            new()
            {
                Title = "Higiene do Sono",
                Description = "Estabeleça uma rotina: dormir e acordar no mesmo horário, evitar telas 1 hora antes de dormir, manter quarto escuro e fresco.",
                Category = "Sono",
                MinWellnessScore = 0,
                MaxWellnessScore = 60
            },
            new()
            {
                Title = "Mindfulness",
                Description = "Pratique meditação por 10-15 minutos diariamente. Aplicativos como Calm ou Headspace podem ajudar.",
                Category = "Saúde Mental",
                MinWellnessScore = 0,
                MaxWellnessScore = 70
            },
            new()
            {
                Title = "Intervalo Visual",
                Description = "A cada 20 minutos de tela, olhe para algo distante por 20 segundos. Reduz fadiga ocular (regra 20-20-20).",
                Category = "Saúde Física",
                MinWellnessScore = 0,
                MaxWellnessScore = 100
            },
            new()
            {
                Title = "Conexão Social",
                Description = "Dedique tempo para conversar com colegas ou amigos. Conexões sociais melhoram bem-estar mental.",
                Category = "Saúde Mental",
                MinWellnessScore = 0,
                MaxWellnessScore = 100
            }
        };

        context.WellnessTips.AddRange(tips);
        context.SaveChanges();

        var userWellnessRecords = new List<UserWellness>
        {
            new()
            {
                UserId = user1.Id,
                AverageWellnessScore = 72,
                TotalHealthChecks = 3,
                UnresolvedAlerts = 0,
                LastCheckAt = DateTime.UtcNow,
                Trend = "Melhorando"
            },
            new()
            {
                UserId = user2.Id,
                AverageWellnessScore = 88,
                TotalHealthChecks = 1,
                UnresolvedAlerts = 0,
                LastCheckAt = DateTime.UtcNow,
                Trend = "Estável"
            }
        };

        context.UserWellness.AddRange(userWellnessRecords);
        context.SaveChanges();
    }
}