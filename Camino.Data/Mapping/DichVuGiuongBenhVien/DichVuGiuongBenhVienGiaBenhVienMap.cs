using Camino.Core.Domain.Entities.DichVuGiuongBenhViens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.DichVuGiuongBenhVien
{
    public class DichVuGiuongBenhVienGiaBenhVienMap : CaminoEntityTypeConfiguration<DichVuGiuongBenhVienGiaBenhVien>
    {
        public override void Configure(EntityTypeBuilder<DichVuGiuongBenhVienGiaBenhVien> builder)
        {
            builder.ToTable(MappingDefaults.DichVuGiuongBenhVienGiaBenhVienTable);
            //builder.HasMany(rf => rf.Di)
            //   .WithMany(r => r.DichVuGiuongBenhViens)
            //   .HasForeignKey(rf => rf.DichVuGiuongId);

            builder.HasOne(rf => rf.DichVuGiuongBenhVien)
           .WithMany(r => r.DichVuGiuongBenhVienGiaBenhViens)
           .HasForeignKey(rf => rf.DichVuGiuongBenhVienId);

            builder.HasOne(rf => rf.NhomGiaDichVuGiuongBenhVien)
           .WithMany(r => r.DichVuGiuongBenhVienGiaBenhViens)
           .HasForeignKey(rf => rf.NhomGiaDichVuGiuongBenhVienId);


            base.Configure(builder);
        }
    }
}
