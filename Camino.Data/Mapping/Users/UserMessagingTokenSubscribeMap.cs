using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.Users
{
    public class UserMessagingTokenSubscribeMap : CaminoEntityTypeConfiguration<UserMessagingTokenSubscribe>
    {
        public override void Configure(EntityTypeBuilder<UserMessagingTokenSubscribe> builder)
        {
            builder.ToTable(MappingDefaults.UserMessagingTokenSubscribeTable);

            builder.HasOne(rf => rf.UserMessagingToken)
                .WithMany(r => r.UserMessagingTokenSubscribes)
                .HasForeignKey(rf => rf.UserMessagingTokenId);

            base.Configure(builder);
        }
    }
}
