using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using trycore.valor.ganado.domain.Entities;

namespace trycore.valor.ganado.infrastructure.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("projects");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasColumnName("description")
            .HasMaxLength(1000);

        builder.Property(p => p.CutoffDate)
            .HasColumnName("cutoff_date")
            .HasColumnType("timestamp without time zone")
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp without time zone")
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp without time zone")
            .IsRequired();

        builder.HasMany(p => p.Activities)
            .WithOne(a => a.Project)
            .HasForeignKey(a => a.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
