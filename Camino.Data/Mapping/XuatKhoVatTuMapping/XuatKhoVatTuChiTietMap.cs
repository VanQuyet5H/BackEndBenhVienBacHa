using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.XuatKhoVatTuMapping
{
    public class XuatKhoVatTuChiTietMap : CaminoEntityTypeConfiguration<XuatKhoVatTuChiTiet>
    {
        public override void Configure(EntityTypeBuilder<XuatKhoVatTuChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.XuatKhoVatTuChiTietTable);


            builder.HasOne(rf => rf.XuatKhoVatTu)
                .WithMany(r => r.XuatKhoVatTuChiTiets)
                .HasForeignKey(rf => rf.XuatKhoVatTuId);

            builder.HasOne(rf => rf.VatTuBenhVien)
                .WithMany(r => r.XuatKhoVatTuChiTiets)
                .HasForeignKey(rf => rf.VatTuBenhVienId);

            base.Configure(builder);
        }
    }
}
