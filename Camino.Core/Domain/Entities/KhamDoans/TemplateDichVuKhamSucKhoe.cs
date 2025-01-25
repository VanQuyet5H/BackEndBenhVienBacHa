using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.KhamDoans
{
    public class TemplateDichVuKhamSucKhoe : BaseEntity
    {
        public Enums.ChuyenKhoaKhamSucKhoe? ChuyenKhoaKhamSucKhoe { get; set; }
        public string Ten { get; set; }
        public string TieuDe { get; set; }
        public string ComponentDynamics { get; set; }
    }
}
