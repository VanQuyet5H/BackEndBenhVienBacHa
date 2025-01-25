using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.NhapKhoVatTuMapping
{
    public class NhapKhoVatTuMap : CaminoEntityTypeConfiguration<NhapKhoVatTu>
    {
        public override void Configure(EntityTypeBuilder<NhapKhoVatTu> builder)
        {
            builder.ToTable(MappingDefaults.NhapKhoVatTuTable);

            builder.HasOne(rf => rf.YeuCauNhapKhoVatTu)
                .WithMany(r => r.NhapKhoVatTus)
                .HasForeignKey(rf => rf.YeuCauNhapKhoVatTuId);

            builder.HasOne(rf => rf.Kho)
                .WithMany(r => r.NhapKhoVatTus)
                .HasForeignKey(rf => rf.KhoId);

            builder.HasOne(rf => rf.YeuCauLinhVatTu)
                .WithMany(r => r.NhapKhoVatTus)
                .HasForeignKey(rf => rf.YeuCauLinhVatTuId);

            builder.HasOne(rf => rf.XuatKhoVatTu)
                .WithMany(r => r.NhapKhoVatTus)
                .HasForeignKey(rf => rf.XuatKhoVatTuId);

            builder.HasOne(rf => rf.NguoiGiao)
                .WithMany(r => r.NhapKhoVatTuNguoiGiaos)
                .HasForeignKey(rf => rf.NguoiGiaoId);

            builder.HasOne(rf => rf.NguoiNhap)
                .WithMany(r => r.NhapKhoVatTuNguoiNhaps)
                .HasForeignKey(rf => rf.NguoiNhapId);

            base.Configure(builder);
        }
    }
}
