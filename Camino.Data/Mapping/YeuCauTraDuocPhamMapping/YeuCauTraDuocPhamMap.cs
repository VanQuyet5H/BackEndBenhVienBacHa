using Camino.Core.Domain.Entities.YeuCauTraDuocPhams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.YeuCauTraDuocPhamMapping
{
    public class YeuCauTraDuocPhamMap : CaminoEntityTypeConfiguration<YeuCauTraDuocPham>
    {
        public override void Configure(EntityTypeBuilder<YeuCauTraDuocPham> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauTraDuocPhamTable);

            builder.HasOne(rf => rf.KhoXuat)
                .WithMany(r => r.YeuCauTraDuocPhamKhoXuats)
                .HasForeignKey(rf => rf.KhoXuatId);

            builder.HasOne(rf => rf.KhoNhap)
                 .WithMany(r => r.YeuCauTraDuocPhamKhoNhaps)
                 .HasForeignKey(rf => rf.KhoNhapId);

            //builder.HasOne(rf => rf.XuatKhoVatTuChiTiet)
            //    .WithMany(r => r.YeuCauTraDuocPhams)
            //    .HasForeignKey(rf => rf.XuatKhoVatTuChiTietId);

            builder.HasOne(rf => rf.NhanVienYeuCau)
                .WithMany(r => r.YeuCauTraDuocPhamNhanVienYeuCaus)
                .HasForeignKey(rf => rf.NhanVienYeuCauId);

            builder.HasOne(rf => rf.NhanVienDuyet)
                .WithMany(r => r.YeuCauTraDuocPhamNhanVienDuyets)
                .HasForeignKey(rf => rf.NhanVienDuyetId);

            base.Configure(builder);
        }
    }
}
