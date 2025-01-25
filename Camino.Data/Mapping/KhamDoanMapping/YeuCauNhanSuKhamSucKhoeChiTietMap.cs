using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.KhamDoans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.KhamDoanMapping
{
    public class YeuCauNhanSuKhamSucKhoeChiTietMap : CaminoEntityTypeConfiguration<YeuCauNhanSuKhamSucKhoeChiTiet>
    {
        public override void Configure(EntityTypeBuilder<YeuCauNhanSuKhamSucKhoeChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauNhanSuKhamSucKhoeChiTietTable);

            builder.HasOne(x => x.YeuCauNhanSuKhamSucKhoe)
                .WithMany(y => y.YeuCauNhanSuKhamSucKhoeChiTiets)
                .HasForeignKey(x => x.YeuCauNhanSuKhamSucKhoeId);

            builder.HasOne(x => x.NhanVien)
                .WithMany(y => y.YeuCauNhanSuNhanVienKhamSucKhoeChiTiets)
                .HasForeignKey(x => x.NhanVienId);

            builder.HasOne(x => x.NguoiGioiThieu)
                .WithMany(y => y.YeuCauNhanSuNguoiGioiThieuKhamSucKhoeChiTiets)
                .HasForeignKey(x => x.NguoiGioiThieuId);

            builder.HasOne(x => x.NhanSuKhamSucKhoeTaiLieuDinhKem)
                .WithMany(y => y.YeuCauNhanSuKhamSucKhoeChiTiets)
                .HasForeignKey(x => x.NhanSuKhamSucKhoeTaiLieuDinhKemId);
            base.Configure(builder);
        }
    }
}
