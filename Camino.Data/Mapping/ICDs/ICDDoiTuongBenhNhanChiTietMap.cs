using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.ICDs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.ICDs
{
    public class ICDDoiTuongBenhNhanChiTietMap : CaminoEntityTypeConfiguration<ICDDoiTuongBenhNhanChiTiet>
    {
        public override void Configure(EntityTypeBuilder<ICDDoiTuongBenhNhanChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.ICDDoiTuongBenhNhanChiTietTable);

            builder.HasOne(rf => rf.ICD)
                .WithMany(r => r.ICDDoiTuongBenhNhanChiTiets)
                .HasForeignKey(rf => rf.ICDId);
            builder.HasOne(rf => rf.ICDDoiTuongBenhNhan)
                .WithMany(r => r.ICDDoiTuongBenhNhanChiTiets)
                .HasForeignKey(rf => rf.ICDDoiTuongBenhNhanId);

            base.Configure(builder);
        }
    }
}
