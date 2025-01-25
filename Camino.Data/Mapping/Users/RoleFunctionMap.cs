using Camino.Core.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.Users
{
    public class RoleFunctionMap : CaminoEntityTypeConfiguration<RoleFunction>
    {
        public override void Configure(EntityTypeBuilder<RoleFunction> builder)
        {
            builder.ToTable(MappingDefaults.RoleFunctionTable);

            builder.HasOne(rf => rf.Role)
                .WithMany(r => r.RoleFunctions)
                .HasForeignKey(rf => rf.RoleId)
                .IsRequired();

            base.Configure(builder);
        }
    }
}
