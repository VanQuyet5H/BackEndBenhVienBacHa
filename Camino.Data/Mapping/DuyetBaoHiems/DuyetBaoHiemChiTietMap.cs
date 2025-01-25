using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DuyetBaoHiems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DuyetBaoHiems
{
    public class DuyetBaoHiemChiTietMap : CaminoEntityTypeConfiguration<DuyetBaoHiemChiTiet>
    {
        public override void Configure(EntityTypeBuilder<DuyetBaoHiemChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.DuyetBaoHiemChiTietTable);

            builder.HasOne(rf => rf.DuyetBaoHiem)
                .WithMany(r => r.DuyetBaoHiemChiTiets)
                .HasForeignKey(rf => rf.DuyetBaoHiemId);
            builder.HasOne(rf => rf.YeuCauKhamBenh)
                .WithMany(r => r.DuyetBaoHiemChiTiets)
                .HasForeignKey(rf => rf.YeuCauKhamBenhId);
            builder.HasOne(rf => rf.YeuCauDichVuKyThuat)
                .WithMany(r => r.DuyetBaoHiemChiTiets)
                .HasForeignKey(rf => rf.YeuCauDichVuKyThuatId);
            builder.HasOne(rf => rf.YeuCauDuocPhamBenhVien)
                .WithMany(r => r.DuyetBaoHiemChiTiets)
                .HasForeignKey(rf => rf.YeuCauDuocPhamBenhVienId);
            builder.HasOne(rf => rf.YeuCauVatTuBenhVien)
                .WithMany(r => r.DuyetBaoHiemChiTiets)
                .HasForeignKey(rf => rf.YeuCauVatTuBenhVienId);
            builder.HasOne(rf => rf.YeuCauDichVuGiuongBenhVien)
                .WithMany(r => r.DuyetBaoHiemChiTiets)
                .HasForeignKey(rf => rf.YeuCauDichVuGiuongBenhVienId);
            builder.HasOne(rf => rf.DonThuocThanhToanChiTiet)
                .WithMany(r => r.DuyetBaoHiemChiTiets)
                .HasForeignKey(rf => rf.DonThuocThanhToanChiTietId);
            builder.HasOne(rf => rf.YeuCauTruyenMau)
                .WithMany(r => r.DuyetBaoHiemChiTiets)
                .HasForeignKey(rf => rf.YeuCauTruyenMauId);
            builder.HasOne(rf => rf.YeuCauDichVuGiuongBenhVienChiPhiBHYT)
                .WithMany(r => r.DuyetBaoHiemChiTiets)
                .HasForeignKey(rf => rf.YeuCauDichVuGiuongBenhVienChiPhiBHYTId);
            builder.HasOne(rf => rf.YeuCauTiepNhanTheBHYT)
                .WithMany(r => r.DuyetBaoHiemChiTiets)
                .HasForeignKey(rf => rf.YeuCauTiepNhanTheBHYTId);

            base.Configure(builder);
        }
    }
}
