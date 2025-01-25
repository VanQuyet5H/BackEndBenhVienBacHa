using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.LichSuSMS
{
    class LichSuSMSMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.Messages.LichSuSMS>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.Messages.LichSuSMS> builder)
        {
            builder.ToTable(MappingDefaults.LichSuSMS);
            base.Configure(builder);
        }
    }
}
