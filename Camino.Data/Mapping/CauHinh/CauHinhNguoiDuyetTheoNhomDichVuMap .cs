using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.CauHinh
{
    public class CauHinhNguoiDuyetTheoNhomDichVuMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.CauHinhs.CauHinhNguoiDuyetTheoNhomDichVu>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.CauHinhs.CauHinhNguoiDuyetTheoNhomDichVu> builder)
        {
            builder.ToTable(MappingDefaults.CauHinhNguoiDuyetTheoNhomDichVuTable);

            builder.HasOne(rf => rf.NhanVien)
                .WithMany(r => r.CauHinhNguoiDuyetTheoNhomDichVus)
                .HasForeignKey(rf => rf.NhanVienId);

            builder.HasOne(rf => rf.NhomDichVuBenhVien)
                .WithMany(r => r.CauHinhNguoiDuyetTheoNhomDichVus)
                .HasForeignKey(rf => rf.NhomDichVuBenhVienId);

            base.Configure(builder);
        }
    }
}
