using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.KhamDoans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.KhamDoanMapping
{
    public class YeuCauNhanSuKhamSucKhoeMap : CaminoEntityTypeConfiguration<YeuCauNhanSuKhamSucKhoe>
    {
        public override void Configure(EntityTypeBuilder<YeuCauNhanSuKhamSucKhoe> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauNhanSuKhamSucKhoeTable);

            builder.HasOne(x => x.HopDongKhamSucKhoe)
                .WithMany(y => y.YeuCauNhanSuKhamSucKhoes)
                .HasForeignKey(x => x.HopDongKhamSucKhoeId);

            builder.HasOne(x => x.NhanVienGuiYeuCau)
                .WithMany(y => y.YeuCauNhanSuNhanVienGuiYeuCauKhamSucKhoes)
                .HasForeignKey(x => x.NhanVienGuiYeuCauId);
            builder.HasOne(x => x.NhanVienKHTHDuyet)
                .WithMany(y => y.YeuCauNhanSuNhanVienKHTHDuyetKhamSucKhoes)
                .HasForeignKey(x => x.NhanVienKHTHDuyetId);
            builder.HasOne(x => x.NhanVienNhanSuDuyet)
                .WithMany(y => y.YeuCauNhanSuNhanVienNhanSuDuyetKhamSucKhoes)
                .HasForeignKey(x => x.NhanVienNhanSuDuyetId);
            builder.HasOne(x => x.GiamDoc)
                .WithMany(y => y.YeuCauNhanSuGiamDocKhamSucKhoes)
                .HasForeignKey(x => x.GiamDocId);
            base.Configure(builder);
        }
    }
}
