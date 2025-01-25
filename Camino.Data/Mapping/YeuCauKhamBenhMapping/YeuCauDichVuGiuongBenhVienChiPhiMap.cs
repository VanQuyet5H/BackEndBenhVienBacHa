using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauKhamBenhMapping
{
    public class YeuCauDichVuGiuongBenhVienChiPhiMap : CaminoEntityTypeConfiguration<YeuCauDichVuGiuongBenhVienChiPhi>
    {
        public override void Configure(EntityTypeBuilder<YeuCauDichVuGiuongBenhVienChiPhi> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauDichVuGiuongBenhVienChiPhiTable);

            builder.HasOne(rf => rf.YeuCauTiepNhan)
                .WithMany(r => r.YeuCauDichVuGiuongBenhVienChiPhis)
                .HasForeignKey(rf => rf.YeuCauTiepNhanId);

            builder.HasOne(rf => rf.DichVuGiuongBenhVien)
                .WithMany(r => r.YeuCauDichVuGiuongBenhVienChiPhis)
                .HasForeignKey(rf => rf.DichVuGiuongBenhVienId);

            builder.HasOne(rf => rf.NhomGiaDichVuGiuongBenhVien)
                .WithMany(r => r.YeuCauDichVuGiuongBenhVienChiPhis)
                .HasForeignKey(rf => rf.NhomGiaDichVuGiuongBenhVienId);

            builder.HasOne(rf => rf.NhanVienDuyetBaoHiem)
                .WithMany(r => r.NhanVienDuyetBaoHiemYeuCauDichVuGiuongBenhVienChiPhis)
                .HasForeignKey(rf => rf.NhanVienDuyetBaoHiemId);

            builder.HasOne(rf => rf.NhanVienHuyThanhToan)
                .WithMany(r => r.NhanVienHuyThanhToanYeuCauDichVuGiuongBenhVienChiPhis)
                .HasForeignKey(rf => rf.NhanVienHuyThanhToanId);

            base.Configure(builder);
        }
    }
}
