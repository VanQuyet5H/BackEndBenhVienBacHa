using Camino.Core.Domain.Entities.DoiTuongUuDais;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Camino.Data.Mapping.DoiTuongUuDais
{

    public class DoiTuongUuDaiDichVuKhamBenhBenhVienMap : CaminoEntityTypeConfiguration<DoiTuongUuDaiDichVuKhamBenhBenhVien>
    {
        public override void Configure(EntityTypeBuilder<DoiTuongUuDaiDichVuKhamBenhBenhVien> builder)
        {
            builder.ToTable(MappingDefaults.DoiTuongUuDaiDichVuKhamBenhBenhVienTable);
                    builder.HasOne(rf => rf.DichVuKhamBenhBenhVien)
                            .WithMany(r => r.DoiTuongUuDaiDichVuKhamBenhBenhViens)
                            .HasForeignKey(rf => rf.DichVuKhamBenhBenhVienId);


                    builder.HasOne(rf => rf.DoiTuongUuDai)
                              .WithMany(r => r.DoiTuongUuDaiDichVuKhamBenhBenhViens)
                              .HasForeignKey(rf => rf.DoiTuongUuDaiId);
            base.Configure(builder);
        }
    }
}
