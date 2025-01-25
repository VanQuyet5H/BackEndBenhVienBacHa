using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.KhoDuocPhamMapping
{
    public class DuocPhamVaVatTuBenhVienMap : CaminoEntityTypeConfiguration<DuocPhamVaVatTuBenhVien>
    {
        public override void Configure(EntityTypeBuilder<DuocPhamVaVatTuBenhVien> builder)
        {
            builder.ToTable(MappingDefaults.DuocPhamVaVatTuBenhVienTable);
            
            base.Configure(builder);
        }
    }
}
