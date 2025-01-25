
UPDATE template 
set Body = N'{{HeaderMat}}
<head>
  <style>
    table,
    th,
    td {
      border-collapse: collapse;
      font-family: Times New Roman;
      font-size: 20px;
    }

    th,
    td {
      padding: 3px;
    }

    .breakword {
      word-break: break-word;
    }

    .container {
      width: 100%;
      display: table;
    }

    .container .label {
      width: max-content;
    }

    .container .value {
      display: table-cell;
      width: 100%;
      height: 100%;
      vertical-align: top;
      position: relative;
      box-sizing: border-box;
    }

    .container .value .content {
      width: 100%;
      border-bottom: 1px dotted gray;
      margin-top: -5px;
    }

    .containerGD {
      width: 70%;
      display: table;
    }

    .containerGD .label {
      width: max-content;
    }

    .containerGD .value {
      display: table-cell;
      width: 100%;
      height: 100%;
      vertical-align: top;
      position: relative;
      box-sizing: border-box;
    }

    .containerGD .value .content {
      width: 100%;
      border-bottom: 1px dotted gray;
      margin-top: -5px;
    }

    @media print {

      html,
      body {
        font-size: 20px;
        /* changing to 10pt has no impact */
      }
    }
  </style>
</head>
<br>
<br>
<!-- <div style="width: 100%; padding-left:2cm"> -->
<table style="width:100%">
  <tbody>
    <tr>
      <td style="text-align:left;">
        <b>BỆNH VIỆN ĐKQT BẮC HÀ</b>
        <br>KHOA KHÁM BỆNH
      </td>
      <td colspan="2" style="font-size: 24px">
        <b>PHIẾU KHÁM BỆNH</b>
      </td>
      <td style="text-align:center;" width="28%" rowspan="2">
        <img style="height: 40px;" src="data:image/png;base64,{{BarCodeImgBase64}}">
        <br> Mã BN: {{MaBN}}
      </td>
    </tr>
    <tr>
      <td style="text-align:left;"></td>
      <td colspan="2" style="font-size: 18px; padding-left:23px">
        <b>CHUYÊN KHOA MẮT</b>
      </td>
    </tr>
    <tr>
      <td style="text-align:left;" width="33%"></td>
      <td colspan="3">Phòng khám: {{PhongKham}}</td>
    </tr>
  </tbody>
</table>
<br>
<table style="width:100%">
  <tr>
    <td style="font-size: 20px" colspan="3">
      <b>I. HÀNH CHÍNH</b>
    </td>
  </tr>
  <tr>
    <td>1. Họ tên : <b>{{HoTenBenhNhan}}</b>
    </td>
    <td>3. Giới tính: <b>{{GioiTinh}}</b>
    </td>
    <td>5. Dân tộc: {{DanToc}}</td>
  </tr>
  <tr>
    <td>2. Sinh ngày: <b>{{SinhNgay}}</b>
    </td>
    <td>4. Nghề nghiệp: {{NgheNghiep}}</td>
    <td>6. Quốc tịch: {{QuocTich}}</td>
  </tr>
  <tr>
    <td>7. Địa chỉ: {{SoNha}} </td>
    <td colspan="2"> Xã (Phường): {{XaPhuong}} </td>
  </tr>
  <tr>
    <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Huyện(Q.T): {{Huyen}} </td>
    <td colspan="2"> Tỉnh, thành phố: {{TinhThanhPho}} </td>
  </tr>
  <tr>
    <td>8. Nơi làm việc: {{NoiLamViec}} </td>
    <td colspan="2"> 9. Đối tượng: <b>{{DoiTuong}}</b>
    </td>
  </tr>
</table>
<table style="width:100%">
  <tr>
    <td width="55%">10. BHYT giá trị: {{BHYTNgayHetHan}}</td>
    <td> Số thẻ BHYT: <b>{{BHYTMaSoThe}}</b>
    </td>
  </tr>
  <tr>
    <td> 11. Họ tên, địa chỉ người nhà: {{NguoiLienHeQuanHeThanNhan}}</td>
    <td>Số điện thoại: {{SoDienThoaiQHTN}} </td>
  </tr>
  <tr>
    <td colspan="2">12. Đến khám bệnh lúc: {{ThoiDiemTiepNhan}} </td>
  </tr>
</table>
<table style="width:100%">
  <tr>
    <td colspan="3">
      <div class="container">
        <div class="label">13. Chẩn đoán của nơi giới thiệu: </div>
        <div class="value breakword" style="width:70%">
          <div class="content">{{ChanDoanNoiGioiThieu}} </div>
        </div>
      </div>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="font-size: 20px">
      <div class="container">
        <div class="label">
          <b>II. LÍ DO VÀO KHÁM:</b>
        </div>
        <div class="value breakword" style="width:77%;font-size: 19px">
          <div class="content">{{{LyDoVaoKham}}</div>
        </div>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="font-size: 20px">
      <b>III. TIỀN SỬ:</b>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="font-size: 19px">
      <div class="container">
        <div class="label">1.Tiền sử bệnh: </div>
        <div class="value breakword" style="width:85%">
          <div class="content">{{{TienSuBenh}}</div>
        </div>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="font-size: 19px">
      <div class="container">
        <div class="label">2.Tiền sử dị ứng: </div>
        <div class="value breakword" style="width:85%">
          <div class="content">{{{TienSuDiUng}}</div>
        </div>
    </td>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="font-size: 20px">
      <b>IV. KHÁM BỆNH:</b>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="; vertical-align: top;font-size: 19px">
      <div class="container">
        <div class="label">
          <b>1.Qúa trình bệnh lý:</b>
        </div>
        <div class="value breakword" style="width:81%">
          <div class="content">{{QuaTrinhBenhLy}}</div>
        </div>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="font-size: 19px">
      <div class="container">
        <div class="label"></div>
        <div class="value breakword">
          <div class="content">&nbsp;</div>
        </div>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="; vertical-align: top;font-size: 19px">
      <div class="container">
        <div class="label">
          <b>2.Toàn thân:</b>
        </div>
        <div class="value breakword" style="width:87%">
          <div class="content">&nbsp;{{ToanThan}}</div>
        </div>
    </td>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="font-size: 19px">
      <div class="container">
        <div class="label"></div>
        <div class="value breakword">
          <div class="content">&nbsp;</div>
        </div>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="; vertical-align: top;font-size: 19px">
      <b>3.Bệnh chuyên khoa:</b>
      </div>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="font-size: 19px">
      <div class="container">
        <div class="label">
          <b>Khám mắt:</b>
        </div>
        <div class="value breakword" style="width:89%">
          <div class="content">&nbsp;{{KhamMat}}</div>
        </div>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="font-size: 19px">
      <div class="container">
        <div class="label"></div>
        <div class="value breakword">
          <div class="content">&nbsp;</div>
        </div>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="font-size: 19px">
      <div class="container">
        <div class="label"></div>
        <div class="value breakword">
          <div class="content">&nbsp;</div>
        </div>
    </td>
  </tr>
</table>
<table style="width:100%">
  <tr>
    <td style="width:30%;font-size: 19px">
      <b>- Thị lực không kính:</b>
    </td>
    <td style="width:25%;font-size: 19px">
      <div class="containerGD">
        <div class="label">MP: </div>
        <div class="value breakword">
          <div class="content">{{MPKK}}</div>
        </div>
    </td>
    <td style="width:15%;font-size: 19px">
      <b>- Nhãn áp:</b>
    </td>
    <td style="font-size: 19px">
      <div class="containerGD">
        <div class="label">MP: </div>
        <div class="value breakword">
          <div class="content">{{MPNA}}</div>
        </div>
    </td>
  </tr>
  <tr>
    <td>&nbsp;</td>
    <td style="font-size: 19px">
      <div class="containerGD">
        <div class="label">MT: </div>
        <div class="value breakword">
          <div class="content">{{MTKK}}</div>
        </div>
    </td>
    <td>&nbsp;</td>
    <td style="font-size: 19px">
      <div class="containerGD">
        <div class="label">MT: </div>
        <div class="value breakword">
          <div class="content">{{MTNA}}</div>
        </div>
    </td>
  </tr>
  <tr>
    <td style="font-size: 19px">
      <b>- Thị lực có kính:</b>
    </td>
    <td style="font-size: 19px">
      <div class="containerGD">
        <div class="label">MP: </div>
        <div class="value breakword">
          <div class="content">{{MPCK}}</div>
        </div>
    </td>
    <td>&nbsp;</td>
    <td>&nbsp;</td>
  </tr>
  <tr>
    <td>&nbsp;</td>
    <td style="font-size: 19px">
      <div class="containerGD">
        <div class="label">MT: </div>
        <div class="value breakword">
          <div class="content">{{MTCK}}</div>
        </div>
    </td>
    <td>&nbsp;</td>
    <td>&nbsp;</td>
  </tr>
</table>
<table style="width:100%">
  <tr>
    <td colspan="3" style="font-size: 19px">
      <div class="container">
        <div class="label">
          <b>4. Tóm tắt cận lâm sàng: </b>
        </div>
        <div class="value breakword" style="width:77%">
          <div class="content">&nbsp;{{TomTatCLS}}</div>
        </div>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="font-size: 19px">
      <div class="container">
        <div class="label"></div>
        <div class="value breakword">
          <div class="content">&nbsp;</div>
        </div>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="font-size: 19px">
      <div class="container">
        <div class="label">
          <b>5. Chẩn đoán: </b>
        </div>
        <div class="value breakword" style="width:86%">
          <div class="content">&nbsp;{{ChanDoanVaoVien}}</div>
        </div>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="font-size: 19px">
      <div class="container">
        <div class="label"></div>
        <div class="value breakword">
          <div class="content">&nbsp;</div>
        </div>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="font-size: 19px">
      <div class="container">
        <div class="label">
          <b>6. Đã xử lí (thuốc, chăm sóc...):</b>
        </div>
        <div class="value breakword" style="width:71%">
          <div class="content">&nbsp;{{DaXuLi}}</div>
        </div>
    </td>
  </tr>
  <tr>
    <td colspan="3">
      <div class="container">
        <div class="label"></div>
        <div class="value breakword">
          <div class="content">&nbsp;</div>
        </div>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="font-size: 19px">
      <div class="container">
        <div class="label">
          <b>7.Hướng xử lý và lời dặn:</b>
        </div>
        <div class="value breakword" style="width:77%">
          <div class="content">&nbsp; <b>{{HuongXuLi}}</b>
          </div>
        </div>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="font-size: 19px">
      <div class="container">
        <div class="label"></div>
        <div class="value breakword">
          <div class="content">&nbsp;</div>
        </div>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="font-size: 19px">
      <div class="container">
        <div class="label"></div>
        <div class="value breakword">
          <div class="content">&nbsp;</div>
        </div>
    </td>
  </tr>
  <tr>
    <td></td>
    <td style="text-align:center;font-size: 19px">{{NgayThangNam}}</td>
  </tr>
  <tr>
    <td></td>
    <td style="text-align:center;font-size: 19px" width="40%">
      <b>Bác sĩ khám bệnh</b>
    </td>
  </tr>
  <tr>
    <td></td>
    <td style="text-align:center;font-size: 19px">
      <br>
      <br>
      <br>
      <b>{{HoTenBacSi}}</b>
    </td>
  </tr>
</table>
<!-- </div> -->'
where name = N'PhieuKhamBenhVaoVienChuyenKhoaMat'
Go
UPDATE template 
set body = N'{{HeaderTMH}}
<head>
  <style>
    table,
    th,
    td {
      border-collapse: collapse;
      font-family: Times New Roman;
      font-size: 20px;
    }

    th,
    td {
      padding: 3px;
    }

    .breakword {
      word-break: break-word;
    }

    .container {
      width: 100%;
      display: table;
    }

    .container .label {
      width: max-content;
    }

    .container .value {
      display: table-cell;
      width: 100%;
      height: 100%;
      vertical-align: top;
      position: relative;
      box-sizing: border-box;
    }

    .container .value .content {
      width: 100%;
      border-bottom: 1px dotted gray;
      margin-top: -5px;
    }

    .containerGD {
      width: 70%;
      display: table;
    }

    .containerGD .label {
      width: max-content;
    }

    .containerGD .value {
      display: table-cell;
      width: 100%;
      height: 100%;
      vertical-align: top;
      position: relative;
      box-sizing: border-box;
    }

    .containerGD .value .content {
      width: 100%;
      border-bottom: 1px dotted gray;
      margin-top: -5px;
    }

    @media print {

      html,
      body {
        font-size: 20px;
        /* changing to 10pt has no impact */
      }
    }
  </style>
</head>
<br>
<!-- <div style="width: 100%; padding-left:2cm"> -->
<table style="width:100%">
  <tbody>
    <tr>
      <td style="text-align:left;" width="28%">
        <b>BỆNH VIỆN ĐKQT BẮC HÀ</b>
        <br>KHOA KHÁM BỆNH
      </td>
      <td colspan="2" style="font-size: 24px;  padding-left:30px">
        <b>PHIẾU KHÁM BỆNH</b>
      </td>
      <td style="text-align:center;" width="28%" rowspan="2">
        <img style="height: 40px;" src="data:image/png;base64,{{BarCodeImgBase64}}">
        <br> Mã BN: {{MaBN}}
      </td>
    </tr>
    <tr>
      <td style="text-align:left;"></td>
      <td colspan="2" style="font-size: 18px;padding-left:15px">
        <b>CHUYÊN KHOA TAI MŨI HỌNG</b>
      </td>
    </tr>
    <tr>
      <td style="text-align:left;"></td>
      <td colspan="3">Phòng khám: {{PhongKham}}</td>
    </tr>
  </tbody>
</table>
<br>
<table style="width:100%">
  <tr>
    <td style="font-size: 20px" colspan="3">
      <b>I. HÀNH CHÍNH</b>
    </td>
  </tr>
  <tr>
    <td>1. Họ tên : <b>{{HoTenBenhNhan}}</b>
    </td>
    <td>3. Giới tính: <b>{{GioiTinh}}</b>
    </td>
    <td>5. Dân tộc: {{DanToc}}</td>
  </tr>
  <tr>
    <td>2. Sinh ngày: <b>{{SinhNgay}}</b>
    </td>
    <td>4. Nghề nghiệp: {{NgheNghiep}}</td>
    <td>6. Quốc tịch: {{QuocTich}}</td>
  </tr>
  <tr>
    <td>7. Địa chỉ: {{SoNha}} </td>
    <td colspan="2"> Xã (Phường): {{XaPhuong}} </td>
  </tr>
  <tr>
    <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Huyện(Q.T): {{Huyen}} </td>
    <td colspan="2"> Tỉnh, thành phố: {{TinhThanhPho}} </td>
  </tr>
  <tr>
    <td>8. Nơi làm việc: {{NoiLamViec}} </td>
    <td colspan="2"> 9. Đối tượng: <b>{{DoiTuong}}</b>
    </td>
  </tr>
</table>
<table style="width:100%">
  <tr>
    <td width="55%">10. BHYT giá trị: {{BHYTNgayHetHan}}</td>
    <td> Số thẻ BHYT: <b>{{BHYTMaSoThe}}</b>
    </td>
  </tr>
  <tr>
    <td> 11. Họ tên, địa chỉ người nhà: {{NguoiLienHeQuanHeThanNhan}}</td>
    <td>Số điện thoại: {{SoDienThoaiQHTN}} </td>
  </tr>
</table>
<table style="width:100%">
  <tr>
    <td colspan="3">12. Đến khám bệnh lúc: {{ThoiDiemTiepNhan}} </td>
  </tr>
  <tr>
    <td colspan="3">
      <div class="container">
        <div class="label">13. Chẩn đoán của nơi giới thiệu: </div>
        <div class="value breakword" style="width:70%">
          <div class="content">{{ChanDoanNoiGioiThieu}} </div>
        </div>
      </div>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="font-size: 20px">
      <div class="container">
        <div class="label">
          <b>II. LÍ DO VÀO KHÁM:</b>
        </div>
        <div class="value breakword" style="width:77%;font-size: 19px">
          <div class="content">{{{LyDoVaoKham}}</div>
        </div>
      </div>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="font-size: 20px">
      <b>III. TIỀN SỬ:</b>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="font-size: 19px">
      <div class="container">
        <div class="label">1.Tiền sử bệnh: </div>
        <div class="value breakword" style="width:85%">
          <div class="content">{{{TienSuBenh}}</div>
        </div>
      </div>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="font-size: 19px">
      <div class="container">
        <div class="label">2.Tiền sử dị ứng: </div>
        <div class="value breakword" style="width:85%">
          <div class="content">{{{TienSuDiUng}}</div>
        </div>
      </div>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="font-size: 20px">
      <b>IV. KHÁM BỆNH:</b>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="; vertical-align: top;font-size: 19px">
      <div class="container">
        <div class="label">
          <b>1.Qúa trình bệnh lý:</b>
        </div>
        <div class="value breakword" style="width:81%">
          <div class="content">{{QuaTrinhBenhLy}}</div>
        </div>
      </div>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="font-size: 19px">
      <div class="container">
        <div class="label"></div>
        <div class="value breakword">
          <div class="content">&nbsp;</div>
        </div>
      </div>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="; vertical-align: top;font-size: 19px">
      <div class="container">
        <div class="label">
          <b>2.Toàn thân:</b>
        </div>
        <div class="value breakword" style="width:87%">
          <div class="content">&nbsp;{{ToanThan}}</div>
        </div>
      </div>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="font-size: 19px">
      <div class="container">
        <div class="label"></div>
        <div class="value breakword">
          <div class="content">&nbsp;</div>
        </div>
      </div>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="; vertical-align: top;font-size: 19px">
      <b>3.Bệnh chuyên khoa:</b>
    </td>
  </tr>
</table>
<table style="width:100%">
  <!--<tbody>-->
  <tr>
    <td style="width:15%">&nbsp;</td>
    <td style="font-size: 19px">
      <div class="container">
        <div class="label">
          <b>TAI:</b> &nbsp;&nbsp;
        </div>
        <div class="value breakword">
          <div class="content">&nbsp;{{Tai}}</div>
        </div>
      </div>
    </td>
  </tr>
  <!--<tr><td style="width:15%">&nbsp;</td><td><div class="container"><div class="label">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</div><div class="value breakword">&nbsp;</div></td></tr>-->
  <!-- <tr> -->
  <!-- <td style="width:15%">&nbsp;</td> -->
  <!-- <td> -->
  <!-- <div class="container"> -->
  <!-- <div class="label">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</div> -->
  <!-- <div class="value breakword">&nbsp;</div> -->
  <!-- </td> -->
  <!-- </tr> -->
  <tr>
    <td style="width:15%">&nbsp;</td>
    <td style="font-size: 19px">
      <div class="container">
        <div class="label">
          <b>MŨI:</b> &nbsp;&nbsp;
        </div>
        <div class="value breakword" style="width:93%">
          <div class="content">&nbsp;{{Mui}}</div>
        </div>
      </div>
    </td>
  </tr>
  <!--<tr><td style="width:15%">&nbsp;</td><td><div class="container"><div class="label">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</div><div class="value breakword">&nbsp;</div></td></tr>-->
  <!-- <tr> -->
  <!-- <td style="width:15%">&nbsp;</td> -->
  <!-- <td> -->
  <!-- <div class="container"> -->
  <!-- <div class="label">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</div> -->
  <!-- <div class="value breakword">&nbsp;</div> -->
  <!-- </td> -->
  <!-- </tr> -->
  <tr>
    <td style="width:15%">&nbsp;</td>
    <td style="font-size: 19px">
      <div class="container">
        <div class="label">
          <b>HỌNG:</b> &nbsp;&nbsp;
        </div>
        <div class="value breakword" style="width:93%">
          <div class="content">&nbsp;{{Hong}}</div>
        </div>
      </div>
    </td>
  </tr>
  <!--<tr><td style="width:15%">&nbsp;</td><td><div class="container"><div class="label">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</div><div class="value breakword">&nbsp;</div></td></tr>-->
  <!-- <tr> -->
  <!-- <td style="width:15%">&nbsp;</td> -->
  <!-- <td> -->
  <!-- <div class="container"> -->
  <!-- <div class="label">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</div> -->
  <!-- <div class="value breakword">&nbsp;</div> -->
  <!-- </td> -->
  <!-- </tr> -->
  <!--</tbody>-->
</table>
<table style="width:100%">
  <tr>
    <td colspan="3" style="font-size: 19px">
      <div class="container mt-2">
        <div class="label">
          <b>4. Tóm tắt cận lâm sàng: </b>
        </div>
        <div class="value breakword" style="width:77%">
          <div class="content">&nbsp;{{TomTatCLS}}</div>
        </div>
      </div>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="font-size: 19px">
      <div class="container">
        <div class="label"></div>
        <div class="value breakword">
          <div class="content">&nbsp;</div>
        </div>
      </div>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="font-size: 19px">
      <div class="container">
        <div class="label">
          <b>5. Chẩn đoán: </b>
        </div>
        <div class="value breakword" style="width:86%">
          <div class="content">&nbsp;{{ChanDoanVaoVien}}</div>
        </div>
      </div>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="font-size: 19px">
      <div class="container">
        <div class="label"></div>
        <div class="value breakword">
          <div class="content">&nbsp;</div>
        </div>
      </div>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="font-size: 19px">
      <div class="container">
        <div class="label">
          <b>6. Đã xử lí (thuốc, chăm sóc...):</b>
        </div>
        <div class="value breakword" style="width:71%">
          <div class="content">&nbsp;{{DaXuLi}}</div>
        </div>
      </div>
    </td>
  </tr>
  <tr>
    <td colspan="3">
      <div class="container">
        <div class="label"></div>
        <div class="value breakword">
          <div class="content">&nbsp;</div>
        </div>
      </div>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="font-size: 19px">
      <div class="container">
        <div class="label">
          <b>7.Hướng xử lý và lời dặn:</b>
        </div>
        <div class="value breakword" style="width:77%">
          <div class="content">&nbsp; <b>{{HuongXuLi}}</b>
          </div>
        </div>
      </div>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="font-size: 19px">
      <div class="container">
        <div class="label"></div>
        <div class="value breakword">
          <div class="content">&nbsp;</div>
        </div>
      </div>
    </td>
  </tr>
  <tr>
    <td colspan="3" style="font-size: 19px">
      <div class="container">
        <div class="label"></div>
        <div class="value breakword">
          <div class="content">&nbsp;</div>
        </div>
      </div>
    </td>
  </tr>
  <tr>
    <td></td>
    <td style="text-align:center;font-size: 19px">{{NgayThangNam}}</td>
  </tr>
  <tr>
    <td></td>
    <td style="text-align:center;font-size: 19px" width="40%">
      <b>Bác sĩ khám bệnh</b>
    </td>
  </tr>
  <tr>
    <td></td>
    <td style="text-align:center;font-size: 19px">
      <br>
      <br>
      <br>
      <b>{{HoTenBacSi}}</b>
    </td>
  </tr>
</table>
<!-- </div> -->'
where name = N'PhieuKhamBenhVaoVienTaiMuiHong'
Go
UPDATE CauHinh
Set [Value] = '3.4.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'