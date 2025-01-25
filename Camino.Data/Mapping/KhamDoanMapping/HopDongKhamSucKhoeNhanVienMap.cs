using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.KhamDoans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.KhamDoanMapping
{
    public class HopDongKhamSucKhoeNhanVienMap : CaminoEntityTypeConfiguration<HopDongKhamSucKhoeNhanVien>
    {
        public override void Configure(EntityTypeBuilder<HopDongKhamSucKhoeNhanVien> builder)
        {
            builder.ToTable(MappingDefaults.HopDongKhamSucKhoeNhanVienTable);

            builder.HasOne(x => x.HopDongKhamSucKhoe)
                .WithMany(y => y.HopDongKhamSucKhoeNhanViens)
                .HasForeignKey(x => x.HopDongKhamSucKhoeId);

            builder.HasOne(x => x.BenhNhan)
                .WithMany(y => y.HopDongKhamSucKhoeNhanViens)
                .HasForeignKey(x => x.BenhNhanId);

            builder.HasOne(x => x.NgheNghiep)
                .WithMany(y => y.HopDongKhamSucKhoeNhanViens)
                .HasForeignKey(x => x.NgheNghiepId);

            builder.HasOne(x => x.QuocTich)
                .WithMany(y => y.HopDongKhamSucKhoeNhanViens)
                .HasForeignKey(x => x.QuocTichId);

            builder.HasOne(x => x.DanToc)
                .WithMany(y => y.HopDongKhamSucKhoeNhanViens)
                .HasForeignKey(x => x.DanTocId);

            builder.HasOne(x => x.PhuongXa)
                .WithMany(y => y.HopDongKhamSucKhoePhuongXaNhanViens)
                .HasForeignKey(x => x.PhuongXaId);
            builder.HasOne(x => x.QuanHuyen)
                .WithMany(y => y.HopDongKhamSucKhoeQuanHuyenNhanViens)
                .HasForeignKey(x => x.QuanHuyenId);
            builder.HasOne(x => x.TinhThanh)
                .WithMany(y => y.HopDongKhamSucKhoeTinhThanhNhanViens)
                .HasForeignKey(x => x.TinhThanhId);
            builder.HasOne(x => x.GoiKhamSucKhoe)
                .WithMany(y => y.HopDongKhamSucKhoeNhanViens)
                .HasForeignKey(x => x.GoiKhamSucKhoeId);


            builder.HasOne(x => x.HoKhauPhuongXa)
               .WithMany(y => y.HopDongKhamSucKhoeHoKhauPhuongXaNhanViens)
               .HasForeignKey(x => x.HoKhauPhuongXaId);
            builder.HasOne(x => x.HoKhauQuanHuyen)
                .WithMany(y => y.HopDongKhamSucKhoeHoKhauQuanHuyenNhanViens)
                .HasForeignKey(x => x.HoKhauQuanHuyenId);
            builder.HasOne(x => x.HoKhauTinhThanh)
                .WithMany(y => y.HopDongKhamSucKhoeHoKhauTinhThanhNhanViens)
                .HasForeignKey(x => x.HoKhauTinhThanhId);
           
            base.Configure(builder);
        }
    }
}
