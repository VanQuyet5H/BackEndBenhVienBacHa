using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.LichSuSMS
{
    public class LichSuSMSViewModel : BaseViewModel
    {
        public string GoiDen { get; set; }
        public string NoiDung { get; set; }
        public string TenTrangThai { get; set; }
        public DateTime NgayGui { get; set; }
        public Core.Domain.Enums.TrangThaiLishSu TrangThai { get; set; }
    }
}
