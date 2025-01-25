using Camino.Core.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.Users
{
    public class RoleMap : CaminoEntityTypeConfiguration<Role>
    {
        public override void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable(MappingDefaults.RoleTable);

            builder.Property(u => u.Name).HasMaxLength(100);

            base.Configure(builder);
        }
    }
}
