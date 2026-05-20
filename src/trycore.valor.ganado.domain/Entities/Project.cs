namespace trycore.valor.ganado.domain.Entities;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CutoffDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<Activity> Activities { get; set; } = new List<Activity>();
}
