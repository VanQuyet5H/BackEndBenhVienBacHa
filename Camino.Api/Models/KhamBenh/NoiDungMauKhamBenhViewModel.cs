using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.KhamBenh
{
    public class NoiDungMauKhamBenhViewModel : BaseViewModel
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string NoiDung { get; set; }
        public long? BacSiId { get; set; }
    }
}
