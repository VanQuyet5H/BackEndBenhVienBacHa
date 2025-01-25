using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.YeuCauKhamBenhMapping
{
    public class YeuCauDichVuKyThuatKhamSangLocTiemChungMap : CaminoEntityTypeConfiguration<YeuCauDichVuKyThuatKhamSangLocTiemChung>
    {
        public override void Configure(EntityTypeBuilder<YeuCauDichVuKyThuatKhamSangLocTiemChung> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauDichVuKyThuatKhamSangLocTiemChungTable);

            builder.HasOne(rf => rf.YeuCauDichVuKyThuat)
                .WithOne(r => r.KhamSangLocTiemChung)
                .HasForeignKey<YeuCauDichVuKyThuatKhamSangLocTiemChung>(c => c.Id);
                        
            builder.HasOne(rf => rf.NhanVienTheoDoiSauTiem)
                .WithMany()
                .HasForeignKey(rf => rf.NhanVienTheoDoiSauTiemId);

            builder.HasOne(rf => rf.NhanVienHoanThanhKhamSangLoc)
                .WithMany()
                .HasForeignKey(rf => rf.NhanVienHoanThanhKhamSangLocId);

            builder.HasOne(rf => rf.NoiTheoDoiSauTiem)
                .WithMany()
                .HasForeignKey(rf => rf.NoiTheoDoiSauTiemId);

            base.Configure(builder);
        }
    }
}
