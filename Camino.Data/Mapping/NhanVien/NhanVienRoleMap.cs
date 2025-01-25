using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.NhanViens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.NhanVien
{
    public class NhanVienRoleMap : CaminoEntityTypeConfiguration<NhanVienRole>
    {
        public override void Configure(EntityTypeBuilder<NhanVienRole> builder)
        {
            builder.ToTable(MappingDefaults.NhanVienRoleTable);

            builder.HasOne(rf => rf.NhanVien)
                .WithMany(r => r.NhanVienRoles)
                .HasForeignKey(rf => rf.NhanVienId);


            builder.HasOne(rf => rf.Role)
                .WithMany(r => r.NhanVienRoles)
                .HasForeignKey(rf => rf.RoleId);
            
            base.Configure(builder);
        }
    }
}
