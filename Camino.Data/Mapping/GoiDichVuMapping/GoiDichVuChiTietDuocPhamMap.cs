using Camino.Core.Domain.Entities.GoiDichVus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.GoiDichVuMapping
{
    //public class GoiDichVuChiTietDuocPhamMap : CaminoEntityTypeConfiguration<GoiDichVuChiTietDuocPham>
    //{
    //    public override void Configure(EntityTypeBuilder<GoiDichVuChiTietDuocPham> builder)
    //    {
    //        builder.ToTable(MappingDefaults.GoiDichVuChiTietDuocPhamTable);

    //            builder.HasOne(rf => rf.DuocPhamBenhVien)
    //                      .WithMany(r => r.GoiDichVuChiTietDuocPhams)
    //                      .HasForeignKey(rf => rf.DuocPhamBenhVienId)
    //                      .IsRequired();

    //            builder.HasOne(rf => rf.GoiDichVu)
    //                     .WithMany(r => r.GoiDichVuChiTietDuocPhams)
    //                     .HasForeignKey(rf => rf.GoiDichVuId)
    //                     .IsRequired();

    //        base.Configure(builder);
    //    }
    //}
}
