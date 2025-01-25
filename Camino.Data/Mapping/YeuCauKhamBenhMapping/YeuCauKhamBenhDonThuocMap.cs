using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauKhamBenhMapping
{
    public class YeuCauKhamBenhDonThuocMap : CaminoEntityTypeConfiguration<YeuCauKhamBenhDonThuoc>
    {
        public override void Configure(EntityTypeBuilder<YeuCauKhamBenhDonThuoc> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauKhamBenhDonThuocTable);

            
            builder.HasOne(rf => rf.YeuCauKhamBenh)
                .WithMany(r => r.YeuCauKhamBenhDonThuocs)
                .HasForeignKey(rf => rf.YeuCauKhamBenhId);


            builder.HasOne(rf => rf.ToaThuocMau)
                .WithMany(r => r.YeuCauKhamBenhDonThuocs)
                .HasForeignKey(rf => rf.ToaThuocMauId);

            builder.HasOne(rf => rf.NoiKeDon)
             .WithMany(r => r.YeuCauKhamBenhDonThuocNoiKeDons)
             .HasForeignKey(rf => rf.NoiKeDonId);

            builder.HasOne(rf => rf.BacSiKeDon)
                   .WithMany(r => r.YeuCauKhamBenhDonThuocBacSiKeDons)
                   .HasForeignKey(rf => rf.BacSiKeDonId);



            base.Configure(builder);
        }
    }
}
