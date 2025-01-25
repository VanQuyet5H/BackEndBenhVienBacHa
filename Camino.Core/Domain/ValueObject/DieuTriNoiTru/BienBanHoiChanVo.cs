using System;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class BienBanHoiChanVo
    {
        public BienBanHoiChanVo()
        {
            ThanhVienThamGia = new List<string>();
            ThanhVienThamGias = new List<ThanhVienThamGia>();
        }
        
        public DateTime? ThoiGianHoiChan { get; set; }

        public List<string> ThanhVienThamGia { get; set; }

        public string ChanDoan { get; set; }

        public string Summary { get; set; }

        public string DongMau { get; set; }

        public string NhomMau { get; set; }

        public string Ure { get; set; }

        public string Creatinin { get; set; }

        public string Got { get; set; }

        public string Gpt { get; set; }

        public string DienTim { get; set; }

        public string XnKhac { get; set; }

        public string KetLuan { get; set; }

        public string DuKienPhauThuat { get; set; }

        public List<string> NhomPhauThuat { get; set; }

        public List<string> NhomGayMe { get; set; }

        public string PhuongPhapGayMe { get; set; }

        public DateTime? ThoiGianPhauThuat { get; set; }

        public string DuKienKhac { get; set; }

        public DateTime? NgayThucHien { get; set; }

        public string BacSiDieuTri { get; set; }

        public string BacSiGayMe { get; set; }

        public string BacSiTruongKhoa { get; set; }

        public string BacSiDuyetMo { get; set; }

        public string BacSiThucHien { get; set; }

        public string NgayThucHienReadonly { get; set; }

        public long? IdNoiTruHoSo { get; set; }
        public string ThoiGianHoiChanUTC { get; set; }
        public List<ThanhVienThamGia> ThanhVienThamGias { get; set; }
    }
    public class ThanhVienThamGia
    {
        public long KeyId { get; set; }
        public string ViTriCongTac { get; set; }
    }
}
