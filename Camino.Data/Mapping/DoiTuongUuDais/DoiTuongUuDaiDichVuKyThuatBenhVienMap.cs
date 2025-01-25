using Camino.Core.Domain.Entities.DoiTuongUuDais;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;



namespace Camino.Data.Mapping.DoiTuongUuDais
{
    public class DoiTuongUuDaiDichVuKyThuatBenhVienMap : CaminoEntityTypeConfiguration<DoiTuongUuDaiDichVuKyThuatBenhVien>
    {
        public override void Configure(EntityTypeBuilder<DoiTuongUuDaiDichVuKyThuatBenhVien> builder)
        {
            builder.ToTable(MappingDefaults.DoiTuongUuDaiDichVuKyThuatBenhVienTable);

            builder.HasOne(rf => rf.DichVuKyThuatBenhVien)
                      .WithMany(r => r.DoiTuongUuDaiDichVuKyThuatBenhViens)
                      .HasForeignKey(rf => rf.DichVuKyThuatBenhVienId);


            builder.HasOne(rf => rf.DoiTuongUuDai)
                      .WithMany(r => r.DoiTuongUuDaiDichVuKyThuatBenhViens)
                      .HasForeignKey(rf => rf.DoiTuongUuDaiId);

            base.Configure(builder);
        }
    }
}
