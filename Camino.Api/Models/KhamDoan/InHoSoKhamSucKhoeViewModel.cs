using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;

namespace Camino.Api.Models.KhamDoan
{
    public class InHoSoKhamSucKhoeViewModel
    {
        public long HopDongKhamSucKhoeNhanVienId { get; set; }
        public string TenFile { get; set; }
        public string HostingName { get; set; }
        public Enums.LoaiHoSoKhamSucKhoe LoaiHoSoKhamSucKhoe { get; set; }
    }
}
