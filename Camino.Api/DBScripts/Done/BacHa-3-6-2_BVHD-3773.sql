UPDATE Template 
SET Body = N'<style>
  table,
  th,
  td {
    border-collapse: collapse;
    font-family: Times New Roman;
    font-size: 16px;
  }

  th,
  td {
    padding: 2px;
  }

  .breakword {
    word-break: break-word;
  }

  .border-label {
    border: 1px solid black;
    border-radius: 25px;
  }
</style>
<div style="width: 100%;">
  <table style="width:100%">
    <tbody>
      <tr>
        <td style="text-align:center;font-size: 14px" width="24%">
          <b>BV ĐKQT BẮC HÀ</b>
        </td>
        <td style="text-align:center;font-size: 16px" width="54%">
          <b>CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM</b>
        </td>
        <td style="text-align:left;" width="22%">Số hồ sơ: <b>{{MaTN}}</b>
        </td>
      </tr>
      <tr>
        <td style="text-align:center;font-size: 14px; vertical-align:top;">Số: {{STT}}/{{Nam}}/GCT</td>
        <td style="text-align:center;font-size: 16px;">
          <span>
            <b>Độc lập - Tự do - Hạnh phúc</b>
          </span>
          <br> --------------------
        </td>
        <td style="text-align:left;;">Vào sổ chuyển tuyến <br> số: <b>{{TuyenSo}}</b>
        </td>
      </tr>
    </tbody>
  </table>
</div>
<p style="text-align:center;font-size: 16px; margin:10px">
  <b>GIẤY CHUYỂN TUYẾN KHÁM BỆNH, CHỮA BỆNH BẢO HIỂM Y TẾ</b>
</p>
<table style="width:100%">
  <tr>
    <td width="25%"></td>
    <td> Kính gửi: <b>{{BenhVienChuyenDen}}</b>
    </td>
  </tr>
</table>
<table style="width:100%">
  <tr>
    <td colspan="3">Bệnh viện Đa khoa Quốc tế Bắc Hà trân trọng giới thiệu:</td>
  </tr>
  <tr>
    <td width="55%">- Họ và tên người bệnh: <b>{{HoTenBenhNhan}}</b>
    </td>
    <td width="25%">Nam/Nữ: <b>{{GioiTinh}}</b>
    </td>
    <td width="20%">Tuổi: <b>{{Tuoi}}</b>
    </td>
  </tr>
  <tr>
    <td colspan="3">- Địa chỉ: <b>{{DiaChi}}</b>
    </td>
  </tr>
  <tr>
    <td>- Dân tộc: <b>{{DanToc}}</b>
    </td>
    <td colspan="3">Quốc tịch: <b>{{QuocTich}}</b>
    </td>
  </tr>
  <tr>
    <td>- Nghề nghiệp: <b>{{NgheNghiep}}</b>
    </td>
    <td colspan="3">Nơi làm việc: <b>{{NoiLamViec}}</b>
    </td>
  </tr>
  <tr>
    <td colspan="3">- Số thẻ: <b>{{BHYTMaSoThe}}</b>
    </td>
  </tr>
  <tr>
    <td colspan="3"> Hạn sử dụng: <b>{{BHYTNgayHetHan}}</b>
    </td>
  </tr>
  <tr>
    <td colspan="3">Đã được khám bệnh/điều trị: </td>
  </tr> {{DaKhamBenhDieuTri}}
  <tr>
    <td colspan="3" style="font-size: 16px;text-align:center">
      <b>TÓM TẮT BỆNH ÁN </b>
    </td>
  </tr>
  <tr>
    <td colspan="3" class="breakword">- Dấu hiệu lâm sàng: <span>
        <b>{{DauHieuLamSang}}</b>
      </span>
    </td>
  </tr>
  <tr>
    <td colspan="3" class="breakword">- Kết quả xét nghiệm, cận lâm sàng : <span>
        <b>{{KetQuaCLS}}</b>
      </span>
    </td>
  </tr>
  <tr>
    <td colspan="3" class="breakword">- Chẩn đoán: <b>{{ChanDoan}}</b>
    </td>
  </tr>
  <tr>
    <td colspan="3" class="breakword">- Phương pháp, thủ thuật, kỹ thuật, thuốc đã sử dụng trong điều trị: <b>{{PhuongPhap}}</b>
    </td>
  </tr>
  <tr>
    <td colspan="3" class="breakword">- Tình trạng người bệnh lúc chuyển tuyến: <b>{{TinhTrang}}</b>
    </td>
  </tr>
  <tr>
    <td colspan="3">- Lí do chuyển tuyến: Khoanh tròn vào lý do chuyển tuyến phù hợp sau đây: </td>
  </tr>
  <tr>
    <td colspan="3">&nbsp;&nbsp;&nbsp; <span class="classBorderDuDieuKienChuyenTuyen">1. Đủ điều kiện chuyển tuyến.</span>
    </td>
  </tr>
  <tr>
    <td colspan="3">&nbsp;&nbsp;&nbsp; <span class="classBorderTheoYeuCauNguoiBenh">2. Theo yêu cầu của người bệnh hoặc người đại diện hợp pháp của người bệnh.</span>
    </td>
  </tr>
  <tr>
    <td colspan="3" class="breakword">- Hướng điều trị: <span>
        <b>{{HuongDieuTri}}</b>
      </span>
    </td>
  </tr>
  <tr>
    <td colspan="3">- Chuyển tuyến hồi: <span>
        <b>{{ThoiGianChuyenTuyen}}</b>
      </span>
    </td>
  </tr>
  <tr>
    <td colspan="3" class="breakword">- Phương tiện vận chuyển: <span>
        <b>{{PhuongTienVanChuyen}}</b>
      </span>
    </td>
  </tr>
  <tr>
    <td colspan="3">- Họ tên, chức danh, trình độ chuyên môn của người hộ tống: <span>
        <b>{{NguoiHoTong}}</b>
      </span>
    </td>
  </tr>
</table>
<table style="width:100%">
  <tr>
    <td width="50%"></td>
    <td style="text-align:center">{{NgayThangNam}}</td>
  </tr>
  <tr>
    <th width="50%">Y, BÁC SĨ KHÁM, ĐIỀU TRỊ</th>
    <th>NGƯỜI CÓ THẨM QUYỀN CHUYỂN TUYẾN </th>
  </tr>
  <tr>
    <td style="text-align:center">
      <i>(Ký và ghi rõ họ tên)</i>
    </td>
    <td style="text-align:center">
      <i>(Ký tên, đóng dấu)</i>
    </td>
  </tr>
</table>'
where name = N'GiayChuyenVienBHYT'
Go
Update dbo.CauHinh
Set [Value] = '3.6.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'