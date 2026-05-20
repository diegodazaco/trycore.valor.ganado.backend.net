using Microsoft.EntityFrameworkCore;
using trycore.valor.ganado.application.Interfaces;
using trycore.valor.ganado.domain.Entities;

namespace trycore.valor.ganado.infrastructure.Persistence.Repositories;

public class ActivityRepository : IActivityRepository
{
    private readonly AppDbContext _context;

    public ActivityRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Activity>> GetByProjectIdAsync(Guid projectId) =>
        await _context.Activities
            .AsNoTracking()
            .Where(a => a.ProjectId == projectId)
            .ToListAsync();

    public async Task<Activity?> GetByIdAsync(Guid id) =>
        await _context.Activities.FindAsync(id);

    public async Task AddAsync(Activity activity)
    {
        _context.Activities.Add(activity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Activity activity)
    {
        _context.Activities.Update(activity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var activity = await _context.Activities.FindAsync(id);
        if (activity is null)
            return false;

        _context.Activities.Remove(activity);
        await _context.SaveChangesAsync();
        return true;
    }
}
