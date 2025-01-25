using Camino.Core.Domain.Entities.VatTuBenhViens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.VatTuBenhViens
{
    //public class VatTuBenhVienGiaBaoHiemMap : CaminoEntityTypeConfiguration<VatTuBenhVienGiaBaoHiem>
    //{
    //    public override void Configure(EntityTypeBuilder<VatTuBenhVienGiaBaoHiem> builder)
    //    {
    //        builder.ToTable(MappingDefaults.VatTuBenhVienGiaBaoHiemTable);

    //        builder.HasOne(rf => rf.VatTuBenhVien)
    //            .WithMany(r => r.VatTuBenhVienGiaBaoHiems)
    //            .HasForeignKey(rf => rf.VatTuBenhVienId);

    //        base.Configure(builder);
    //    }
    //}
}
