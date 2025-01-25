using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauKhamBenhMapping
{
    public class YeuCauTraDuocPhamTuBenhNhanChiTietMap : CaminoEntityTypeConfiguration<YeuCauTraDuocPhamTuBenhNhanChiTiet>
    {
        public override void Configure(EntityTypeBuilder<YeuCauTraDuocPhamTuBenhNhanChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauTraDuocPhamTuBenhNhanChiTietTable);

            builder.HasOne(rf => rf.YeuCauTraDuocPhamTuBenhNhan)
                .WithMany(r => r.YeuCauTraDuocPhamTuBenhNhanChiTiets)
                .HasForeignKey(rf => rf.YeuCauTraDuocPhamTuBenhNhanId);

            builder.HasOne(rf => rf.YeuCauDuocPhamBenhVien)
                .WithMany(r => r.YeuCauTraDuocPhamTuBenhNhanChiTiets)
                .HasForeignKey(rf => rf.YeuCauDuocPhamBenhVienId);

            builder.HasOne(rf => rf.DuocPhamBenhVien)
                .WithMany(r => r.YeuCauTraDuocPhamTuBenhNhanChiTiets)
                .HasForeignKey(rf => rf.DuocPhamBenhVienId);

            builder.HasOne(rf => rf.KhoTra)
                .WithMany(r => r.YeuCauTraDuocPhamTuBenhNhanChiTiets)
                .HasForeignKey(rf => rf.KhoTraId);

            builder.HasOne(rf => rf.NhanVienYeuCau)
                .WithMany(r => r.YeuCauTraDuocPhamTuBenhNhanChiTiets)
                .HasForeignKey(rf => rf.NhanVienYeuCauId);

            base.Configure(builder);
        }
    }
}
