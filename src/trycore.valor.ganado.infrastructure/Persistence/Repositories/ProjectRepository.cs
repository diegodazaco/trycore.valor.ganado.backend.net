using Microsoft.EntityFrameworkCore;
using trycore.valor.ganado.application.Interfaces;
using trycore.valor.ganado.domain.Entities;

namespace trycore.valor.ganado.infrastructure.Persistence.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _context;

    public ProjectRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Project>> GetAllAsync() =>
        await _context.Projects.AsNoTracking().ToListAsync();

    public async Task<Project?> GetByIdAsync(Guid id) =>
        await _context.Projects.FindAsync(id);

    public async Task<Project?> GetByIdWithActivitiesAsync(Guid id) =>
        await _context.Projects
            .Include(p => p.Activities)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task AddAsync(Project project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Project project)
    {
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project is null)
            return false;

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        return true;
    }
}
