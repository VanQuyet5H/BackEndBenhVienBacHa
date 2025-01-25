using Camino.Core.Domain.Entities.GoiDichVus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Camino.Data.Mapping.GoiDichVuMapping
{
    //public class GoiDichVuChiTietVatTuMap : CaminoEntityTypeConfiguration<GoiDichVuChiTietVatTu>
    //{
    //    public override void Configure(EntityTypeBuilder<GoiDichVuChiTietVatTu> builder)
    //    {
    //        builder.ToTable(MappingDefaults.GoiDichVuChiTietVatTuTable);

    //        builder.HasOne(rf => rf.GoiDichVu)
    //                  .WithMany(r => r.GoiDichVuChiTietVatTus)
    //                  .HasForeignKey(rf => rf.GoiDichVuId);

    //        builder.HasOne(rf => rf.VatTuBenhVien)
    //                  .WithMany(r => r.GoiDichVuChiTietVatTus)
    //                  .HasForeignKey(rf => rf.VatTuBenhVienId);

    //        base.Configure(builder);
    //    }
    //}
}
