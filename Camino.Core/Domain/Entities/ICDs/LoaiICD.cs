using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.ICDs
{
    public class LoaiICD: BaseICDEntity
    {
        public long NhomICDId { get; set; }

        public virtual NhomICD NhomICD { get; set; }

        public ICollection<ICD> _iCDs;
        public virtual ICollection<ICD> ICDs
        {
            get => _iCDs ?? (_iCDs = new List<ICD>());
            protected set => _iCDs = value;
        }
    }
}
