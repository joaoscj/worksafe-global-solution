using HabitFlow.Application.DTOs;
using HabitFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HabitFlow.Application.Services;

public interface ITipsService
{
    Task<IEnumerable<WellnessTipDto>> GetTipsForWellnessScoreAsync(int wellnessScore);
    Task<IEnumerable<WellnessTipDto>> GetTipsByCategoryAsync(string category);
}

public class TipsService : ITipsService
{
    private readonly HabitFlowContext _context;

    public TipsService(HabitFlowContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<WellnessTipDto>> GetTipsForWellnessScoreAsync(int wellnessScore)
    {
        var tips = await _context.WellnessTips
            .Where(t => t.IsActive && wellnessScore >= t.MinWellnessScore && wellnessScore <= t.MaxWellnessScore)
            .Select(t => new WellnessTipDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Category = t.Category,
                MinWellnessScore = t.MinWellnessScore,
                MaxWellnessScore = t.MaxWellnessScore
            })
            .ToListAsync();

        return tips;
    }

    public async Task<IEnumerable<WellnessTipDto>> GetTipsByCategoryAsync(string category)
    {
        var tips = await _context.WellnessTips
            .Where(t => t.IsActive && t.Category == category)
            .Select(t => new WellnessTipDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Category = t.Category,
                MinWellnessScore = t.MinWellnessScore,
                MaxWellnessScore = t.MaxWellnessScore
            })
            .ToListAsync();

        return tips;
    }
}
