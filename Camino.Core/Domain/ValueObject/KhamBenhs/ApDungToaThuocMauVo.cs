using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.KhamBenhs
{
    public class ApDungToaThuocMauVo
    {
        public long YeuCauKhamBenhHienTaiId { get; set; }
        public long ToaMauId { get; set; }
    }
    public class ApDungToaThuocLichSuKhamBenhVo
    {
        public long YeuCauKhamBenhTruocId { get; set; }
        public long YeuCauKhamBenhHienTaiId { get; set; }
    }
    public class ApDungToaThuocLichSuKhamBenhConfirmVo
    {
        public long YeuCauKhamBenhTruocId { get; set; }
        public long YeuCauKhamBenhHienTaiId { get; set; }
        public List<ApDungToaThuocChiTietVo> ApDungToaThuocChiTietVos { get; set; }
    }
    public class ApDungToaThuocConfirmVo
    {
        public long YeuCauKhamBenhHienTaiId { get; set; }
        public List<ApDungToaThuocChiTietVo> ApDungToaThuocChiTietVos { get; set; }
    }
}
