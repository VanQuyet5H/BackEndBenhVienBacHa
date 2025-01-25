using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.BenhNhan
{
    public class BenhNhanMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.BenhNhans.BenhNhan>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.BenhNhans.BenhNhan> builder)
        {
            builder.ToTable(MappingDefaults.BenhNhanTable);

            builder.HasOne(m => m.NgheNghiep)
                .WithMany(u => u.BenhNhans)
                .HasForeignKey(m => m.NgheNghiepId);

            builder.HasOne(m => m.QuocTich)
                .WithMany(u => u.QuocTichBenhNhans)
                .HasForeignKey(m => m.QuocTichId);

            builder.HasOne(m => m.DanToc)
                .WithMany(u => u.DanTocBenhNhans)
                .HasForeignKey(m => m.DanTocId);

            builder.HasOne(m => m.PhuongXa)
                .WithMany(u => u.PhuongXaBenhNhans)
                .HasForeignKey(m => m.PhuongXaId);

            builder.HasOne(m => m.QuanHuyen)
                .WithMany(u => u.QuanHuyenBenhNhans)
                .HasForeignKey(m => m.QuanHuyenId);

            builder.HasOne(m => m.TinhThanh)
                .WithMany(u => u.TinhThanhBenhNhans)
                .HasForeignKey(m => m.TinhThanhId);

            builder.HasOne(m => m.NguoiLienHeQuanHeNhanThan)
                .WithMany(u => u.NguoiLienHeQuanHeNhanThanBenhNhans)
                .HasForeignKey(m => m.NguoiLienHeQuanHeNhanThanId);

            builder.HasOne(m => m.NguoiLienHePhuongXa)
                .WithMany(u => u.NguoiLienHePhuongXaBenhNhans)
                .HasForeignKey(m => m.NguoiLienHePhuongXaId);

            builder.HasOne(m => m.NguoiLienHeQuanHuyen)
                .WithMany(u => u.NguoiLienHeQuanHuyenBenhNhans)
                .HasForeignKey(m => m.NguoiLienHeQuanHuyenId);

            builder.HasOne(m => m.NguoiLienHeTinhThanh)
                .WithMany(u => u.NguoiLienHeTinhThanhBenhNhans)
                .HasForeignKey(m => m.NguoiLienHeTinhThanhId);

            //builder.HasOne(x => x.BhytgiayMienCungChiTra)
            //    .WithMany(y => y.BenhNhan)
            //    .HasForeignKey(x => x.BHYTGiayMienCungChiTraId);

            //builder.HasOne(x => x.BHTNCongTyBaoHiem)
            //    .WithMany(y => y.BenhNhan)
            //    .HasForeignKey(x => x.BHTNCongTyBaoHiemId);

            builder.HasOne(m => m.BHYTgiayMienCungChiTra)
                .WithMany(u => u.BenhNhans)
                .HasForeignKey(m => m.BHYTGiayMienCungChiTraId);

            //builder.HasOne(m => m.BHYTNoiDangKy)
            //    .WithMany(u => u.BHYTNoiDangKyBenhNhans)
            //    .HasForeignKey(m => m.BHYTNoiDangKyId);

            builder.HasMany(m => m.BenhNhanCongTyBaoHiemTuNhans)
                .WithOne(u => u.BenhNhan)
                .HasForeignKey(m => m.BenhNhanId)
                .IsRequired();
            base.Configure(builder);
        }
    }
}