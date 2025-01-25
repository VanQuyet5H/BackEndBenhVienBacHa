using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.DichVuKhamBenhBenhViens
{
    public class DichVuKhamBenhBenhVienGiaBaoHiemMap : CaminoEntityTypeConfiguration<DichVuKhamBenhBenhVienGiaBaoHiem>
    {
        public override void Configure(EntityTypeBuilder<DichVuKhamBenhBenhVienGiaBaoHiem> builder)
        {
            builder.ToTable(MappingDefaults.DichVuKhamBenhBenhVienGiaBaoHiemTable);
            //builder.HasMany(rf => rf.Di)
            //   .WithMany(r => r.DichVuGiuongBenhViens)
            //   .HasForeignKey(rf => rf.DichVuGiuongId);
                    builder.HasOne(rf => rf.DichVuKhamBenhBenhVien)
                   .WithMany(r => r.DichVuKhamBenhBenhVienGiaBaoHiems)
                   .HasForeignKey(rf => rf.DichVuKhamBenhBenhVienId);
            base.Configure(builder);
        }
    }
}
