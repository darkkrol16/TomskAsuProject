using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomskASUProject.HallOfFame.API.Models;

namespace TomskASUProject.HallOfFame.API.Infrastructure.EntityConfigurations
{
    public class SkillEntityTypeConfiguration : IEntityTypeConfiguration<Skill>
    {
        public void Configure(EntityTypeBuilder<Skill> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                .IsRequired();

            builder.Property(e => e.Level)
                .IsRequired();

            builder.HasOne(e => e.Person)
                .WithMany(e => e.Skills)
                .HasForeignKey(e => e.PersonId);
        }
    }
}
