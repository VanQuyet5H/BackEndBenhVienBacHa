using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DieuTriNoiTrus
{
    public class NoiTruHoSoKhacMap : CaminoEntityTypeConfiguration<NoiTruHoSoKhac>
    {
        public override void Configure(EntityTypeBuilder<NoiTruHoSoKhac> builder)
        {
            builder.ToTable(MappingDefaults.NoiTruHoSoKhacTable);

            builder.HasOne(rf => rf.YeuCauTiepNhan)
                .WithMany(r => r.NoiTruHoSoKhacs)
                .HasForeignKey(rf => rf.YeuCauTiepNhanId);

            builder.HasOne(rf => rf.NhanVienThucHien)
                .WithMany(r => r.NoiTruHoSoKhacs)
                .HasForeignKey(rf => rf.NhanVienThucHienId);

            builder.HasOne(rf => rf.NoiThucHien)
                .WithMany(r => r.NoiTruHoSoKhacs)
                .HasForeignKey(rf => rf.NoiThucHienId);

            base.Configure(builder);
        }
    }
}
