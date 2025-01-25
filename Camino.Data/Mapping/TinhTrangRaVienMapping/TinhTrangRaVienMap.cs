using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.TinhTrangRaVienMapping
{
    public class TinhTrangRaVienMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.TinhTrangRaVienHoSoKhacs.TinhTrangRaVienHoSoKhac>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.TinhTrangRaVienHoSoKhacs.TinhTrangRaVienHoSoKhac> builder)
        {
            builder.ToTable(MappingDefaults.TinhTrangRaVienHoSoKhacTable);
            
        }
    }
}
