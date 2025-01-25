using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.CheDoAn
{
    public class CheDoAnViewModel : BaseViewModel
    {
        public string Ten { get; set; }
        public string KyHieu { get; set; }
        public string MoTa { get; set; }
        public bool? IsDisabled { get; set; }
    }
}
