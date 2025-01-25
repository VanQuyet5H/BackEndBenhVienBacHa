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
        <td style="text-align:center;font-size: 14px; vertical-align:top;">
          Số: {{STT}}/{{Nam}}/GCT
        </td>
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
    <td colspan="3">&nbsp;&nbsp;&nbsp; <span class="classBorderDuDieuKienChuyenTuyen">1.</span> Đủ điều kiện chuyển tuyến. </td>
  </tr>
  <tr>
    <td colspan="3">&nbsp;&nbsp;&nbsp; <span class="classBorderTheoYeuCauNguoiBenh">2.</span> Theo yêu cầu của người bệnh hoặc người đại diện hợp pháp của người bệnh. </td>
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
where Name = N'GiayChuyenVienBHYT'
GO

UPDATE Template 
SET Body = N'<style>
  table,
  th,
  td {
    border-collapse: collapse;
    font-family: Times New Roman;
  }

  th,
  td {
    padding: 2px;
  }
</style>
<div style="width: 100%;">
  <table style="width:100%">
    <tbody>
      <tr>
        <td style="text-align:left;font-size: 15px" width="22%">Sở Y tế Hà Nội</td>
        <td style="text-align:center;font-size: 18px" width="56%">
          <b>CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM</b>
        </td>
        <td style="text-align:left;font-size: 15px" width="22%">Số hồ sơ: <b>{{MaTN}}</b>
        </td>
      </tr>
      <tr>
        <td style="text-align:left;font-size: 15px" width="22%">BV: Đa Khoa <br> Quốc Tế Bắc Hà </td>
        <td rowspan="2" style="text-align:center;font-size: 18px;" width="56%">
          <span>
            <b>Độc lập - Tự do - Hạnh phúc</b>
          </span>
          <br> --------------------
        </td>
        <td rowspan="2" style="text-align:left;font-size: 15px;" width="22%">Vào sổ chuyển <br> tuyến số: <b>{{SoChuyenTuyenSo}}</b>
        </td>
      </tr>
      <tr>
        <td style="text-align:left;font-size: 15px" width="22%">
          SỐ: {{STT}}/{{NAM}}/GCT
        </td>
      </tr>
    </tbody>
  </table>
</div>
<p style="text-align:center;font-size: 19; margin:10px">
  <b>GIẤY CHUYỂN TUYẾN</b>
</p>
<br>
<div style="width: 100%;">
  <table style="width:100%">
    <tr>
      <td colspan="2" style="font-size: 16px" width="23%">
        <b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Kính gửi: {{BenhVienChuyenDen}}</b>
      </td>
    </tr>
    <tr>
      <td colspan="2" style="font-size: 16px" width="23%">BỆNH VIỆN ĐKQT BẮC HÀ trân trọng giới thiệu:</td>
    </tr>
    <tr>
      <td colspan="2" style="font-size: 15px" width="23%">- Họ và tên người bệnh: <b>{{HoTenBenhNhan}}</b>
      </td>
    </tr>
    <tr>
      <td style="font-size: 15px" width="23%">- Giới tính: <b>{{GioiTinh}}</b>
      </td>
      <td style="font-size: 15px" width="23%">Sinh ngày: <b>{{SinhNgay}}</b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Tuổi: <b>{{Tuoi}}</b>
      </td>
    </tr>
    <tr>
      <td style="font-size: 15px" width="23%">- Dân tộc: <b>{{DanToc}}</b>
      </td>
      <td style="font-size: 15px" width="23%">Quốc tịch: <b>{{QuocTich}}</b>
      </td>
    </tr>
    <tr>
      <td colspan="2" style="font-size: 15px" width="23%">- Địa chỉ: <b>{{DiaChi}}</b>
      </td>
    </tr>
    <tr>
      <td style="font-size: 15px" width="23%">- Nghề nghiệp: <b>{{NgheNghiep}}</b>
      </td>
      <td style="font-size: 15px" width="23%">Nơi làm việc: <b>{{NoiLamViec}}</b>
      </td>
    </tr>
    <tr>
      <td style="font-size: 15px" width="23%">- BHYT giá trị: <b>{{BHYTNgayHetHan}}</b>
      </td>
      <td style="font-size: 15px" width="23%"> Số thẻ BHYT: <b>{{BHYTMaSoThe}}</b>
      </td>
    </tr>
    <tr>
      <td colspan="2" style="font-size: 15px" width="23%">Đã được khám bệnh/điều trị: </td>
    </tr> {{DaKhamBenhDieuTri}}
    <tr>
      <td colspan="2" width="23%">
        <p style="text-align:center;font-size: 19;">
          <b>TÓM TẮT BỆNH ÁN </b>
        </p>
      </td>
    </tr>
    <tr>
      <td colspan="2" style="font-size: 15px" width="23%">- Dấu hiệu lâm sàng: <span style="font-size: 15px">
          <b>{{DauHieuLamSang}}</b>
        </span>
      </td>
    </tr>
    <tr>
      <td colspan="2" style="font-size: 15px" width="23%">- Kết quả xét nghiệm, cận lâm sàng <sup>3</sup>: <span style="font-size: 15px">
          <b>{{KetQuaCLS}}</b>
        </span>
      </td>
    </tr>
    <tr>
      <td colspan="2" style="font-size: 15px" width="23%">- Chẩn đoán: <b>{{ChanDoan}}</b>
      </td>
    </tr>
    <tr>
      <td colspan="2" style="font-size: 15px" width="23%">- Phương pháp, thủ thuật, kỹ thuật, thuốc đã sử dụng trong điều trị: <b>{{PhuongPhap}}</b>
      </td>
    </tr>
    <tr>
      <td colspan="2" style="font-size: 15px" width="23%">- Tình trạng người bệnh lúc chuyển tuyến: <b>{{TinhTrang}}</b>
      </td>
    </tr>
    <tr>
      <td colspan="2" style="font-size: 15px" width="23%">- Lí do chuyển tuyến: <b>{{TenLoaiLyDoChuyenVien}}</b>
      </td>
    </tr>
    <tr>
      <td colspan="2" style="font-size: 15px" width="23%">- Hướng điều trị <sup>4</sup>: <span style="font-size: 15px">
          <b>{{HuongDieuTri}}</b>
        </span>
      </td>
    </tr>
    <tr>
      <td colspan="2" style="font-size: 15px" width="23%">- Chuyển tuyến hồi: <span style="font-size: 15px">
          <b>{{ThoiGianChuyenTuyen}}</b>
        </span>
      </td>
    </tr>
    <tr>
      <td colspan="2" style="font-size: 15px" width="23%">- Phương tiện vận chuyển: <span style="font-size: 15px">
          <b>{{PhuongTienVanChuyen}}</b>
        </span>
      </td>
    </tr>
    <tr>
      <td colspan="2" style="font-size: 15px" width="23%">- Họ tên, chức danh, trình độ chuyên môn của người hộ tống: <span style="font-size: 15px">
          <b>{{NguoiHoTong}}</b>
        </span>
      </td>
    </tr>
    <tr>
      <td style="font-size: 15px" width="23%"></td>
      <td style="font-size: 15px; text-align:center;" width="23%">
        <i>Ngày {{Ngay}} tháng {{Thang}} năm {{Nam}}</i>
      </td>
    </tr>
    <tr>
      <td style="font-size: 16px; text-align:center;" width="23%">
        <b>Y, BÁC SĨ KHÁM, ĐIỀU TRỊ</b>
        <br>
        <i>(Ký và ghi rõ họ tên)</i>
      </td>
      <td style="font-size: 16px; text-align:center;" width="23%">
        <b>NGƯỜI CÓ THẨM QUYỀN CHUYỂN TUYẾN <sup>5</sup>
        </b>
        <br>
        <i>(Ký tên, đóng dấu)</i>
      </td>
    </tr>
    <tr>
      <td style="font-size: 15px; text-align:center;" width="23%">
        <br>
        <br>
        <br>
        <br>
        <b>{{BacSiThucHien}}</b>
      </td>
      <td style="font-size: 18px; text-align:center;" width="23%">
        <br>
        <br>
        <br>
        <b></b>
      </td>
    </tr>
    <tr>
      <td colspan="2" style="font-size: 15px" width="23%">
        <b>Ghi chú:</b>
      </td>
    </tr>
    <tr>
      <td colspan="2" style="font-size: 15px" width="23%">1. Cơ quan chủ quản: Bộ Y tế/Sở Y tế/Cục Y tế (đối với y tế bộ, ngành)...</td>
    </tr>
    <tr>
      <td colspan="2" style="font-size: 15px" width="23%">2. Cơ sở KB, CB: Bệnh viện/ Phòng khám/ Trạm Y tế... </td>
    </tr>
    <tr>
      <td colspan="2" style="font-size: 15px" width="23%">3. Kết quả xét nghiệm, cận lâm sàng: bao gồm xét nghiệm sinh hóa, huyết học, GPB, thăm dò chức năng, chẩn đoán hình ảnh...</td>
    </tr>
    <tr>
      <td colspan="2" style="font-size: 15px" width="23%">4. Hướng điều trị: đối với trường hợp cơ sở khám bệnh, chữa bệnh tuyến trên chuyển người bệnh về tuyến dưới điều trị.</td>
    </tr>
    <tr>
      <td colspan="2" style="font-size: 15px" width="23%">5. Người có thẩm quyền chuyển tuyến là người đứng đầu cơ sở khám bệnh, chữa bệnh hoặc người chịu trách nhiệm chuyên môn hoặc người được ủy quyền.</td>
    </tr>
  </table>
</div>'
where Name = N'GiayChuyenVienKetLuan'
GO

UPDATE CauHinh
Set [Value] = '3.4.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'