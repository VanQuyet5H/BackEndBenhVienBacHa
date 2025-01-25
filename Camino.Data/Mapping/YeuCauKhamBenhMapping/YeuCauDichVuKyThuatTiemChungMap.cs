using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauKhamBenhMapping
{
    public class YeuCauDichVuKyThuatTiemChungMap : CaminoEntityTypeConfiguration<YeuCauDichVuKyThuatTiemChung>
    {
        public override void Configure(EntityTypeBuilder<YeuCauDichVuKyThuatTiemChung> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauDichVuKyThuatTiemChungTable);

            builder.HasOne(rf => rf.YeuCauDichVuKyThuat)
                .WithOne(r => r.TiemChung)
                .HasForeignKey<YeuCauDichVuKyThuatTiemChung>(c => c.Id);

            builder.HasOne(rf => rf.DuocPhamBenhVien)
                .WithMany()
                .HasForeignKey(rf => rf.DuocPhamBenhVienId);
            builder.HasOne(rf => rf.DuongDung)
                .WithMany()
                .HasForeignKey(rf => rf.DuongDungId);
            builder.HasOne(rf => rf.DonViTinh)
                .WithMany()
                .HasForeignKey(rf => rf.DonViTinhId);
            builder.HasOne(rf => rf.HopDongThauDuocPham)
                .WithMany()
                .HasForeignKey(rf => rf.HopDongThauDuocPhamId);
            builder.HasOne(rf => rf.NhaThau)
                .WithMany()
                .HasForeignKey(rf => rf.NhaThauId);
            builder.HasOne(rf => rf.XuatKhoDuocPhamChiTiet)
                .WithMany(r=>r.YeuCauDichVuKyThuatTiemChungs)
                .HasForeignKey(rf => rf.XuatKhoDuocPhamChiTietId);
            builder.HasOne(rf => rf.NhanVienTiem)
                .WithMany()
                .HasForeignKey(rf => rf.NhanVienTiemId);

            base.Configure(builder);
        }
    }
}
