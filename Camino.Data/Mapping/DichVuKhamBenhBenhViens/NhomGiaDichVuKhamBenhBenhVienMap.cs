using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.DichVuKhamBenhBenhViens
{
    public class NhomGiaDichVuKhamBenhBenhVienMap : CaminoEntityTypeConfiguration<NhomGiaDichVuKhamBenhBenhVien>
    {
        public override void Configure(EntityTypeBuilder<NhomGiaDichVuKhamBenhBenhVien> builder)
        {
            builder.ToTable(MappingDefaults.NhomGiaDichVuKhamBenhBenhVienTable);
            builder.HasMany(rf => rf.DichVuKhamBenhBenhVienGiaBenhViens)
               .WithOne(r => r.NhomGiaDichVuKhamBenhBenhVien)
               .HasForeignKey(rf => rf.NhomGiaDichVuKhamBenhBenhVienId);
            base.Configure(builder);
        }
    }
}
