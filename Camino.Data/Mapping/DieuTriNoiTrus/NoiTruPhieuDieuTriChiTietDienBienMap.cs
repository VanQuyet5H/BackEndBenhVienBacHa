using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DieuTriNoiTrus
{
    public class NoiTruPhieuDieuTriChiTietDienBienMap : CaminoEntityTypeConfiguration<NoiTruPhieuDieuTriChiTietDienBien>
    {
        public override void Configure(EntityTypeBuilder<NoiTruPhieuDieuTriChiTietDienBien> builder)
        {
            builder.ToTable(MappingDefaults.NoiTruPhieuDieuTriChiTietDienBienTable);

            //builder.HasOne(rf => rf.NoiTruPhieuDieuTri)
            //    .WithMany(r => r.NoiTruPhieuDieuTriChiTietDienBiens)
            //    .HasForeignKey(rf => rf.NoiTruPhieuDieuTriId);

            builder.HasOne(rf => rf.NhanVienCapNhat)
                .WithMany(r => r.NoiTruPhieuDieuTriChiTietDienBiens)
                .HasForeignKey(rf => rf.NhanVienCapNhatId);

            #region //BVHD-3312
            builder.HasOne(rf => rf.NoiTruBenhAn)
                .WithMany(r => r.NoiTruPhieuDieuTriChiTietDienBiens)
                .HasForeignKey(rf => rf.NoiTruBenhAnId);


            #endregion

            base.Configure(builder);
        }
    }
}
