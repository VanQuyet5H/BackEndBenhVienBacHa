using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.BenhVien
{
    public class BenhVienMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.BenhVien.BenhVien>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.BenhVien.BenhVien> builder)
        {
            builder.ToTable(MappingDefaults.BenhVienTable);

            //builder.HasOne(rf => rf.CapQuanLyBenhVien)
            //   .WithMany(r => r.BenhViens)
            //   .HasForeignKey(rf => rf.CapQuanLyBenhVienId);

            builder.HasOne(rf => rf.LoaiBenhVien)
              .WithMany(r => r.BenhViens)
              .HasForeignKey(rf => rf.LoaiBenhVienId);

            builder.HasOne(rf => rf.DonViHanhChinh)
             .WithMany(r => r.BenhViens)
             .HasForeignKey(rf => rf.DonViHanhChinhId);

            base.Configure(builder);
        }
    }
}
