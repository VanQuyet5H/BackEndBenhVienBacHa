using System;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        bool KiemTraThoiGianXayRaTaiNanVoiHienTai(DateTime? thoiGianXayRaTaiNan);
        bool KiemTraThoiGianDenCapCuuVoiHienTai(DateTime? thoiGianDenCapCuu);
        bool KiemTraThoiGianDenCapCuuVoiThoiGianXayRaTaiNan(DateTime? thoiGianXayRaTaiNan, DateTime? thoiGianDenCapCuu);
    }
}