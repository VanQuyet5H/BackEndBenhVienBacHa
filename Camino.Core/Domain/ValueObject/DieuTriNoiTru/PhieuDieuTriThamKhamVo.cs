using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class PhieuDieuTriThamKhamVo
    {

    }

    public class NgayDieuTri
    {
        public NgayDieuTri()
        {
            LstYear = new List<YearModel>();
            LstYearOrderBy = new List<LstYearOrderByModel>();
        }
        public List<YearModel> LstYear { get; set; }
        public List<LstYearOrderByModel> LstYearOrderBy { get; set; }
    }

    public class LstYearOrderByModel
    {
        public DateTime Date { get; set; }
        public long KhoaId { get; set; }
        public long PhieuDieuTriId { get; set; }
        public bool? LaNgayDieuTriTamThoi { get; set; }
        public bool? LaNgayDieuTriDauTien { get; set; }
        public bool CoDonThuocNoiTru { get; set; }
        public bool CoDonVTYTNoiTru { get; set; }

    }

    public class YearModel
    {
        public YearModel()
        {
            Months = new List<MonthModel>();
        }
        public int Year { get; set; }
        public bool Expands { get; set; }

        public List<MonthModel> Months { get; set; }
    }
    public class MonthModel
    {
        public MonthModel()
        {
            Days = new List<DayModel>();
        }
        public int Month { get; set; }
        public List<DayModel> Days { get; set; }
    }
    public class DayModel
    {
        public int Day { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public DateTime FullDate { get; set; }
        public string FullDateDisplay { get; set; }
        public DateTime FullDateNext { get; set; }
        public string FullDateNextDisplay { get; set; }
        public DateTime FullDatePre { get; set; }
        public string FullDatePreDisplay { get; set; }
        public bool? LaNgayDieuTriTamThoi { get; set; }

        public long KhoaId { get; set; }
        public string TenKhoa { get; set; }
        public long PhieuDieuTriId { get; set; }
        public bool LaNgayDieuTriDauTien { get; set; }
        public bool CoDonThuocNoiTru { get; set; }
        public bool CoDonVTYTNoiTru { get; set; }


    }

    public class CreateNewDateModel
    {
        public List<DateTime> Dates { get; set; }
        public long yeuCauTiepNhanId { get; set; }
        public long phieuDieuTriId { get; set; }
        public long? KhoaId { get; set; }
        //public List<DateTime> DateAdds { get; set; }
    }

    public class ICDPhieuDieuTriTemplateVo
    {
        public long KeyId { get; set; }

        public string DisplayName { get; set; }

        public string Ma { get; set; }

        public string Ten { get; set; }
    }
}
