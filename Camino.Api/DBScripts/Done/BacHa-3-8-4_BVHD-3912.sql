UPDATE template 
set Body = N'<style>
  * {
    box-sizing: border-box;
  }

  .table td {
    word-break: break-word;
  }
</style>
<div>
  <div style="float: left;width: 70%;padding: 10px;">
    <table>
      <tbody>
        <tr>
          <td>
            <b>BỆNH VIỆN ĐKQT BẮC HÀ</b>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</div>
<table style="width: 100%; text-align: center;padding-left: 35px;">
  <tr>
    <th>
      <span>
        <font size="5">BÁO CÁO DƯỢC PHẨM ĐANG CẢNH BÁO</font>
      </span>
      <br>
    </th>
  </tr>
  <tr>
    <th style="font-weight: normal;"> Kho: {{TenKho}} - Loại cảnh báo: {{LoaiCanhBao}} </th>
  </tr>
</table>
<div>
  <table style="width:100%;border: 1px solid #020000; border-collapse: collapse;">
    <thead>
      <tr style="border: 1px solid black;  border-collapse: collapse;">
        <th width="5%" style="border: 1px solid #020000;">STT</th>
        <th width="10%" style="border: 1px solid #020000; border-collapse: collapse;text-align: center;">MÃ DƯỢC PHẨM</th>
        <th width="15%" style="border: 1px solid #020000; border-collapse: collapse;text-align: center;">DƯỢC PHẨM</th>
        <th width="15%" style="border: 1px solid #020000; border-collapse: collapse; text-align: center;">HOẠT CHẤT</th>
        <th width="10%" style="border: 1px solid #020000; border-collapse: collapse;text-align: center;">HÀM LƯỢNG</th>
        <th width="15%" style="border: 1px solid #020000; border-collapse: collapse;text-align: center;">PHÂN NHÓM</th>
        <th width="10%" style="border: 1px solid #020000; border-collapse: collapse;text-align: center;">ĐƠN VỊ TÍNH</th>
        <th width="10%" style="border: 1px solid #020000; border-collapse: collapse;text-align: center;">SỐ LƯỢNG TỒN </th>
        <th width="10%" style="border: 1px solid #020000; border-collapse: collapse;text-align: center;">CẢNH BÁO</th>
      </tr>
    </thead>
    <tbody> {{TemplateDuocPhamCanhBao}} </tbody>
  </table>
</div>
<div>
  <table style="width:100%;">
    <tr>
      <td style="text-align: center;" colspan="2">
        <b>Người lập</b>
      </td>
      <td style=" text-align: center;" colspan="2">
        <b>Thủ kho</b>
      </td>
      <td style="text-align: center;">
        <b>Kế toán</b>
      </td>
      <td style=" text-align: center;" colspan="2">
        <b>Ngày {{NgayNow}} tháng {{ThangNow}} năm {{NamNow}}</b>
        <br />
        <b>Trưởng bộ phận</b>&nbsp;&nbsp;
      </td>
    </tr>
    <tr>
      <td style="text-align: center;" colspan="2">(Ký , Ghi rõ họ tên)</td>
      <td style=" text-align: center;" colspan="2">(Ký , Ghi rõ họ tên)</td>
      <td style="text-align: center;">(Ký , Ghi rõ họ tên)</td>
      <td style=" text-align: center;" colspan="2">(Ký , Ghi rõ họ tên)</td>
    </tr>
  </table>
</div>'
where Name =N'BaoCaoDuocPhamDangCanhBao'
GO
UPDATE template 
set Body = N'<style>
  * {
    box-sizing: border-box;
  }

  .table td {
    word-break: break-word;
  }
</style>
<div>
  <div style="float: left;width: 70%;padding: 10px;">
    <table>
      <tbody>
        <tr>
          <td>
            <b>BỆNH VIỆN ĐKQT BẮC HÀ</b>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</div>
<table style="width: 100%; text-align: center;padding-left: 35px;">
  <tr>
    <th>
      <span>
        <font size="5">BÁO CÁO DƯỢC PHẨM TỒN KHO</font>
      </span>
      <br>
    </th>
  </tr>
  <tr>
    <th style="font-weight: normal;"> Kho: {{TenKho}} </th>
  </tr>
</table>
<div>
  <table style="width:100%;border: 1px solid #020000; border-collapse: collapse;" class="table">
    <thead>
      <tr style="border: 1px solid black;  border-collapse: collapse;">
        <th width="5%" style="border: 1px solid #020000;">STT</th>
        <th width="10%" style="border: 1px solid #020000; border-collapse: collapse;text-align: center;">MÃ DƯỢC PHẨM</th>
        <th width="15%" style="border: 1px solid #020000; border-collapse: collapse;text-align: center;">DƯỢC PHẨM</th>
        <th width="15%" style="border: 1px solid #020000; border-collapse: collapse;text-align: center;">HOẠT CHẤT</th>
        <th width="10%" style="border: 1px solid #020000; border-collapse: collapse;text-align: center;">HÀM LƯỢNG</th>
        <th width="10%" style="border: 1px solid #020000; border-collapse: collapse;text-align: center;">PHÂN NHÓM</th>
        <th width="10%" style="border: 1px solid #020000; border-collapse: collapse;text-align: center;">ĐƠN VỊ TÍNH</th>
        <th width="10%" style="border: 1px solid #020000; border-collapse: collapse;text-align: center;">SỐ LƯỢNG TỒN <br>
          <span style="color:red">KHẢ DỤNG</span>
          </br>
        </th>
        <th width="15%" style="border: 1px solid #020000; border-collapse: collapse;text-align: center;">GIÁ TRỊ TỒN</th>
      </tr>
    </thead>
    <tbody> {{TemplateDuocPhamTonKho}}
      <tr>
        <td colspan="7" style="border: 1px solid #020000;font-weight:bold;text-align:right;">TỔNG CỘNG</td>
        <td style="border: 1px solid #020000;font-weight:bold;text-align:right;">{{TotalGiaTriSoLuongTon}}</td>
      </tr>
    </tbody>
  </table>
</div>
<div>
  <table style="width:100%;">
    <tr>
      <td style="text-align: center;" colspan="2">
        <b>Người lập</b>
      </td>
      <td style=" text-align: center;" colspan="2">
        <b>Thủ kho</b>
      </td>
      <td style="text-align: center;">
        <b>Kế toán</b>
      </td>
      <td style=" text-align: center;" colspan="2">
        <b>Ngày {{NgayNow}} tháng {{ThangNow}} năm {{NamNow}}</b>
        <br />
        <b>Trưởng bộ phận</b>&nbsp;&nbsp;
      </td>
    </tr>
    <tr>
      <td style="text-align: center;" colspan="2">(Ký , Ghi rõ họ tên)</td>
      <td style=" text-align: center;" colspan="2">(Ký , Ghi rõ họ tên)</td>
      <td style="text-align: center;">(Ký , Ghi rõ họ tên)</td>
      <td style=" text-align: center;" colspan="2">(Ký , Ghi rõ họ tên)</td>
    </tr>
  </table>
</div>'
where Name =N'BaoCaoDuocPhamTonKho'
GO
Update dbo.CauHinh
Set [Value] = '3.8.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'