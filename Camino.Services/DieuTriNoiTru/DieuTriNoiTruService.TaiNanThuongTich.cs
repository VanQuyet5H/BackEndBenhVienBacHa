using System;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        public bool KiemTraThoiGianXayRaTaiNanVoiHienTai(DateTime? thoiGianXayRaTaiNan)
        {
            if(thoiGianXayRaTaiNan == null)
            {
                return true;
            }

            return thoiGianXayRaTaiNan < DateTime.Now;
        }

        public bool KiemTraThoiGianDenCapCuuVoiHienTai(DateTime? thoiGianDenCapCuu)
        {
            if(thoiGianDenCapCuu == null)
            {
                return true;
            }

            return thoiGianDenCapCuu < DateTime.Now;
        }

        public bool KiemTraThoiGianDenCapCuuVoiThoiGianXayRaTaiNan(DateTime? thoiGianXayRaTaiNan, DateTime? thoiGianDenCapCuu)
        {
            if (thoiGianXayRaTaiNan == null || thoiGianDenCapCuu == null)
            {
                return true;
            }

            return thoiGianXayRaTaiNan < thoiGianDenCapCuu;
        }
    }
}