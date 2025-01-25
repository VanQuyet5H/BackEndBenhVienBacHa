using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Camino.Core.Audit;

namespace Camino.Core.Domain.Entities
{
    public abstract class BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        public long Id { get; set; }

        [AuditIgnore]
        public long LastUserId { get; set; }

        public long CreatedById { get; set; }

        [NotMapped]
        public bool WillDelete { get; set; }

        public DateTime? CreatedOn { get; set; }

        [AuditIgnore]
        public DateTime? LastTime { get; set; }
        [AuditIgnore]
        [Timestamp]
        public virtual byte[] LastModified { get; set; }
    }
}
