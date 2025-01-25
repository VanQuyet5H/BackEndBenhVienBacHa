using Camino.Core.Domain.Entities.DichVuKyThuats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.DichVuKyThuat
{
    public class DichVuKyThuatBenhVienTiemChungMap : CaminoEntityTypeConfiguration<DichVuKyThuatBenhVienTiemChung>
    {
        public override void Configure(EntityTypeBuilder<DichVuKyThuatBenhVienTiemChung> builder)
        {
            builder.ToTable(MappingDefaults.DichVuKyThuatBenhVienTiemChungTable);

            builder.HasOne(rf => rf.DichVuKyThuatBenhVien)
                .WithMany(r => r.DichVuKyThuatBenhVienTiemChungs)
                .HasForeignKey(rf => rf.DichVuKyThuatBenhVienId);

            builder.HasOne(rf => rf.DuocPhamBenhVien)
                .WithMany(r => r.DichVuKyThuatBenhVienTiemChungs)
                .HasForeignKey(rf => rf.DuocPhamBenhVienId);

            base.Configure(builder);
        }
    }
}
