using Camino.Core.Domain.Entities.DichVuGiuongBenhViens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.DichVuGiuongBenhVien
{
    public class NhomGiaDichVuGiuongBenhVienMap : CaminoEntityTypeConfiguration<NhomGiaDichVuGiuongBenhVien>
    {
        public override void Configure(EntityTypeBuilder<NhomGiaDichVuGiuongBenhVien> builder)
        {
            builder.ToTable(MappingDefaults.NhomGiaDichVuGiuongBenhVienTable);
            builder.HasMany(rf => rf.DichVuGiuongBenhVienGiaBenhViens)
               .WithOne(r => r.NhomGiaDichVuGiuongBenhVien)
               .HasForeignKey(rf => rf.NhomGiaDichVuGiuongBenhVienId);
            base.Configure(builder);
        }
    }
}
