using System;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.Entities.TrieuChungDanhMucChuanDoans;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Core.Domain.Entities.TrieuChungs
{
    public class TrieuChung : BaseEntity
    {
        public string Ten { get; set; }
        public long? TrieuChungChaId { get; set; }
        public int CapNhom { get; set; }

        private ICollection<TrieuChungDanhMucChuanDoan> _trieuChungDanhMucChuanDoan;
        public virtual ICollection<TrieuChungDanhMucChuanDoan> TrieuChungDanhMucChuanDoans
        {
            get => _trieuChungDanhMucChuanDoan ?? (_trieuChungDanhMucChuanDoan = new List<TrieuChungDanhMucChuanDoan>());
            protected set => _trieuChungDanhMucChuanDoan = value;
        }

        private ICollection<YeuCauKhamBenhTrieuChung> _yeuCauKhamBenhTrieuChung;
        public virtual ICollection<YeuCauKhamBenhTrieuChung> YeuCauKhamBenhTrieuChungs
        {
            get => _yeuCauKhamBenhTrieuChung ?? (_yeuCauKhamBenhTrieuChung = new List<YeuCauKhamBenhTrieuChung>());
            protected set => _yeuCauKhamBenhTrieuChung = value;
        }
        public virtual TrieuChung TrieuChungCha { get; set; }

        private ICollection<TrieuChung> _trieuChung;
        public virtual ICollection<TrieuChung> TrieuChungs
        {
            get => _trieuChung ?? (_trieuChung = new List<TrieuChung>());
            protected set => _trieuChung = value;
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
