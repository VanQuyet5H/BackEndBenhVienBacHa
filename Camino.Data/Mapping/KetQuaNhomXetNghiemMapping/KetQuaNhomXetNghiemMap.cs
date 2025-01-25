using Camino.Core.Domain.Entities.KetQuaNhomXetNghiems;
using Camino.Core.Domain.Entities.KetQuaSinhHieus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.KetQuaSinhHieuMapping
{
    public class KetQuaNhomXetNghiemMap : CaminoEntityTypeConfiguration<KetQuaNhomXetNghiem>
    {
        public override void Configure(EntityTypeBuilder<KetQuaNhomXetNghiem> builder)
        {
            builder.ToTable(MappingDefaults.KetQuaNhomXetNghiemTable);

            builder.HasOne(rf => rf.YeuCauTiepNhan)
                .WithMany(r => r.KetQuaNhomXetNghiems)
                .HasForeignKey(rf => rf.YeuCauTiepNhanId);          

            //builder.HasOne(rf => rf.FileKetQuaCanLamSang)
            //   .WithMany(r => r.KetQuaNhomXetNghiems)
            //   .HasForeignKey(rf => rf.FileKetQuaCanLamSangId);

            builder.HasOne(rf => rf.NhomDichVuBenhVien)
               .WithMany(r => r.KetQuaNhomXetNghiems)
               .HasForeignKey(rf => rf.NhomDichVuBenhVienId);


            base.Configure(builder);
        }
    }
}
