using System.Collections.Generic;
using Camino.Core.Domain;

namespace Camino.Api.Models.KhamBenh
{
    public class ListKhamBenhPhauThuatThuThuatViewModel : BaseViewModel
    {
        public ListKhamBenhPhauThuatThuThuatViewModel()
        {
            LuocDoPttts = new List<LuocDoPhauThuatViewModel>();
        }

        public string GhiChuICDSauPhauThuat { get; set; }

        public string GhiChuICDTruocPhauThuat { get; set; }

        public long? ICDSauPhauThuatId { get; set; }

        public Enums.LoaiPhauThuatThuThuat? LoaiPTTTEnum { get; set; }

        public string LoaiPTTTDisplay { get; set; }

        public long? ICDTruocPhauThuatId { get; set; }

        public string LuocDoPttt { get; set; }

        public string PhuongPhapPhauThuatThuThuatKey { get; set; }

        public long? PhuongPhapVoCamId { get; set; }

        public Enums.EnumTaiBienPTTT? TaiBienPttt { get; set; }

        public Enums.EnumTinhHinhPhauThuatThuThuat? TinhHinhPttt { get; set; }

        public string TrinhTuPttt { get; set; }

        public Enums.EnumTuVongPTTTTheoNgay? TuVongPttt { get; set; }

        public List<LuocDoPhauThuatViewModel> LuocDoPttts { get; set; }
    }

    public class LuocDoPhauThuatViewModel : BaseViewModel
    {
        public long IdYeuCauDichVuKyThuat { get; set; }

        public string LuocDoPhauThuat { get; set; }

        public string MoTa { get; set; }
    }
}
