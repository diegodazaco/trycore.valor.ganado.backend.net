namespace trycore.valor.ganado.domain.Entities;

public class Activity
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal BudgetAtCompletion { get; set; }
    public decimal PlannedProgressPercentage { get; set; }
    public decimal ActualProgressPercentage { get; set; }
    public decimal ActualCost { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Project Project { get; set; } = null!;
}
