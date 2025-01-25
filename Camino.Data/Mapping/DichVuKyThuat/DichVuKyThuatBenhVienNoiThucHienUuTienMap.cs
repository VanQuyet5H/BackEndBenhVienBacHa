using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DichVuKyThuat
{
    class DichVuKyThuatBenhVienNoiThucHienUuTienMap : CaminoEntityTypeConfiguration<DichVuKyThuatBenhVienNoiThucHienUuTien>
    {
        public override void Configure(EntityTypeBuilder<DichVuKyThuatBenhVienNoiThucHienUuTien> builder)
        {
            builder.ToTable(MappingDefaults.DichVuKyThuatBenhVienNoiThucHienUuTienTable);

            builder.HasOne(x => x.DichVuKyThuatBenhVien)
                .WithMany(y => y.DichVuKyThuatBenhVienNoiThucHienUuTiens)
                .HasForeignKey(x => x.DichVuKyThuatBenhVienId);

            builder.HasOne(x => x.PhongBenhVien)
                .WithMany(y => y.DichVuKyThuatBenhVienNoiThucHienUuTiens)
                .HasForeignKey(x => x.PhongBenhVienId);

            base.Configure(builder);
        }
    }
}
