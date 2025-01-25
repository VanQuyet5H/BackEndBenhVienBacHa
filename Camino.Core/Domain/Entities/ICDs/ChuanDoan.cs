using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using System.Collections.Generic;

namespace Camino.Core.Domain.Entities.ICDs
{
    public class ChuanDoan : BaseICDEntity
    {
        public long DanhMucChuanDoanId { get; set; }
        public virtual DanhMucChuanDoan DanhMucChuanDoan { get; set; }

        public ICollection<ChuanDoanLienKetICD> _chuanDoanLienKetICDs;
        public virtual ICollection<ChuanDoanLienKetICD> ChuanDoanLienKetICDs
        {
            get => _chuanDoanLienKetICDs ?? (_chuanDoanLienKetICDs = new List<ChuanDoanLienKetICD>());
            protected set => _chuanDoanLienKetICDs = value;
        }


        private ICollection<YeuCauKhamBenhChuanDoan> _yeuCauKhamBenhChuanDoans;
        public virtual ICollection<YeuCauKhamBenhChuanDoan> YeuCauKhamBenhChuanDoans
        {
            get => _yeuCauKhamBenhChuanDoans ?? (_yeuCauKhamBenhChuanDoans = new List<YeuCauKhamBenhChuanDoan>());
            protected set => _yeuCauKhamBenhChuanDoans = value;
        }
        /// <summary>
        /// update 07/04/2020
        /// </summary>
        private ICollection<ToaThuocMau> _toaThuocMaus;
        public virtual ICollection<ToaThuocMau> ToaThuocMaus
        {
            get => _toaThuocMaus ?? (_toaThuocMaus = new List<ToaThuocMau>());
            protected set => _toaThuocMaus = value;
        }
    }
}
