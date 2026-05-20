using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using trycore.valor.ganado.domain.Entities;

namespace trycore.valor.ganado.infrastructure.Persistence.Configurations;

public class ActivityConfiguration : IEntityTypeConfiguration<Activity>
{
    public void Configure(EntityTypeBuilder<Activity> builder)
    {
        builder.ToTable("activities");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(a => a.ProjectId)
            .HasColumnName("project_id")
            .IsRequired();

        builder.Property(a => a.Name)
            .HasColumnName("name")
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(a => a.BudgetAtCompletion)
            .HasColumnName("budget_at_completion")
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(a => a.PlannedProgressPercentage)
            .HasColumnName("planned_progress_percentage")
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(a => a.ActualProgressPercentage)
            .HasColumnName("actual_progress_percentage")
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(a => a.ActualCost)
            .HasColumnName("actual_cost")
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(a => a.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp without time zone")
            .IsRequired();

        builder.Property(a => a.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp without time zone")
            .IsRequired();
    }
}
