using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.DonViMaus
{
    public class DonViMau : BaseEntity
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public bool IsDefault { get; set; }

        private ICollection<NoiGioiThieu.NoiGioiThieu> _noiGioiThieus;
        public virtual ICollection<NoiGioiThieu.NoiGioiThieu> NoiGioiThieus
        {
            get => _noiGioiThieus ?? (_noiGioiThieus = new List<NoiGioiThieu.NoiGioiThieu>());
            protected set => _noiGioiThieus = value;
        }
    }
}
