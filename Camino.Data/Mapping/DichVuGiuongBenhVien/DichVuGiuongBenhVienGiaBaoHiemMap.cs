using Camino.Core.Domain.Entities.DichVuGiuongBenhViens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.DichVuGiuongBenhVien
{
    public class DichVuGiuongBenhVienGiaBaoHiemMap : CaminoEntityTypeConfiguration<DichVuGiuongBenhVienGiaBaoHiem>
    {
        public override void Configure(EntityTypeBuilder<DichVuGiuongBenhVienGiaBaoHiem> builder)
        {
            builder.ToTable(MappingDefaults.DichVuGiuongBenhVienGiaBaoHiemTable);
          
            builder.HasOne(rf => rf.DichVuGiuongBenhVien)
           .WithMany(r => r.DichVuGiuongBenhVienGiaBaoHiems)
           .HasForeignKey(rf => rf.DichVuGiuongBenhVienId);
            base.Configure(builder);
        }
    }
}
