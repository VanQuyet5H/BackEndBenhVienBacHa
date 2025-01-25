using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauKhamBenhMapping
{
    public class YeuCauDichVuGiuongBenhVienChiPhiBHYTMap : CaminoEntityTypeConfiguration<YeuCauDichVuGiuongBenhVienChiPhiBHYT>
    {
        public override void Configure(EntityTypeBuilder<YeuCauDichVuGiuongBenhVienChiPhiBHYT> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauDichVuGiuongBenhVienChiPhiBHYTTable);

            builder.HasOne(rf => rf.YeuCauTiepNhan)
                .WithMany(r => r.YeuCauDichVuGiuongBenhVienChiPhiBHYTs)
                .HasForeignKey(rf => rf.YeuCauTiepNhanId);

            builder.HasOne(rf => rf.DichVuGiuongBenhVien)
                .WithMany(r => r.YeuCauDichVuGiuongBenhVienChiPhiBHYTs)
                .HasForeignKey(rf => rf.DichVuGiuongBenhVienId);

            builder.HasOne(rf => rf.GiuongBenh)
                .WithMany(r => r.YeuCauDichVuGiuongBenhVienChiPhiBHYTs)
                .HasForeignKey(rf => rf.GiuongBenhId);
            builder.HasOne(rf => rf.PhongBenhVien)
                .WithMany(r => r.YeuCauDichVuGiuongBenhVienChiPhiBHYTs)
                .HasForeignKey(rf => rf.PhongBenhVienId);
            builder.HasOne(rf => rf.KhoaPhong)
                .WithMany(r => r.YeuCauDichVuGiuongBenhVienChiPhiBHYTs)
                .HasForeignKey(rf => rf.KhoaPhongId);

            builder.HasOne(rf => rf.NhanVienDuyetBaoHiem)
                .WithMany(r => r.YeuCauDichVuGiuongBenhVienChiPhiBHYTs)
                .HasForeignKey(rf => rf.NhanVienDuyetBaoHiemId);
            builder.HasOne(rf => rf.ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVien)
                .WithMany(r => r.YeuCauDichVuGiuongBenhVienChiPhiBHYTs)
                .HasForeignKey(rf => rf.ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVienId);
            builder.HasOne(rf => rf.YeuCauTiepNhanTheBHYT)
                .WithMany(r => r.YeuCauDichVuGiuongBenhVienChiPhiBHYTs)
                .HasForeignKey(rf => rf.YeuCauTiepNhanTheBHYTId);

            base.Configure(builder);
        }
    }
}
