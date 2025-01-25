using Camino.Core.Domain.Entities.HoSoNhanVienDinhKems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.HoSoNhanVienFileDinhKemMapping
{
    public class HoSoNhanVienFileDinhKemMap : CaminoEntityTypeConfiguration<HoSoNhanVienFileDinhKem>
    {
        public override void Configure(EntityTypeBuilder<HoSoNhanVienFileDinhKem> builder)
        {
            builder.ToTable(MappingDefaults.HoSoNhanVienFileDinhKemTable);

            builder.HasOne(rf => rf.NhanVien)
                .WithMany(r => r.HoSoNhanVienFileDinhKems)
                .HasForeignKey(rf => rf.NhanVienId);

            base.Configure(builder);
        }
    }
}
