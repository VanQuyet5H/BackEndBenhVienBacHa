using Camino.Core.Domain.Entities.YeuCauNhapKhoDuocPhams;
using Camino.Core.Domain.Entities.YeuCauNhapKhoVatTus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.YeuCauNhapKhoVatTuMapping
{
    public class YeuCauNhapKhoDuocPhamMap : CaminoEntityTypeConfiguration<YeuCauNhapKhoDuocPham>
    {
        public override void Configure(EntityTypeBuilder<YeuCauNhapKhoDuocPham> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauNhapKhoDuocPhamTable);

            builder.HasOne(rf => rf.Kho)
                .WithMany(r => r.YeuCauNhapKhoDuocPhams)
                .HasForeignKey(rf => rf.KhoId);

            builder.HasOne(rf => rf.NguoiGiao)
                .WithMany(r => r.YeuCauNhapKhoDuocPhamNguoiGiaos)
                .HasForeignKey(rf => rf.NguoiGiaoId);

            builder.HasOne(rf => rf.NguoiNhap)
                .WithMany(r => r.YeuCauNhapKhoDuocPhamNguoiNhaps)
                .HasForeignKey(rf => rf.NguoiNhapId);

            builder.HasOne(rf => rf.NhanVienDuyet)
                .WithMany(r => r.YeuCauNhapKhoDuocPhamNhanVienDuyets)
                .HasForeignKey(rf => rf.NhanVienDuyetId);


            base.Configure(builder);
        }
    }
}
