using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.CauHinhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.CauHinh
{
    public class CauHinhThuePhongMap : CaminoEntityTypeConfiguration<CauHinhThuePhong>
    {
        public override void Configure(EntityTypeBuilder<CauHinhThuePhong> builder)
        {
            builder.ToTable(MappingDefaults.CauHinhThuePhongTable);

            builder.HasOne(rf => rf.LoaiThuePhongPhauThuat)
                .WithMany(r => r.CauHinhThuePhongs)
                .HasForeignKey(rf => rf.LoaiThuePhongPhauThuatId);
            builder.HasOne(rf => rf.LoaiThuePhongNoiThucHien)
                .WithMany(r => r.CauHinhThuePhongs)
                .HasForeignKey(rf => rf.LoaiThuePhongNoiThucHienId);

            base.Configure(builder);
        }
    }
}
