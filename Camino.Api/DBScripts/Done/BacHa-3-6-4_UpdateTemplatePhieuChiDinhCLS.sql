update Template set Body=N'<style>      .footer {          position: fixed;          left: 0;          bottom: 0;          width: 100%;          padding-bottom: 0px      }        .container {          width: 100%;          display: table;      }        .container .label {          width: max-content;      }        .container .value {          display: table-cell;          width: 100%;          height: 100%;          vertical-align: top;          position: relative;          box-sizing: border-box;      }        .container .value .content {          width: 100%;          border-bottom: 1px dotted gray;          margin-top: 14px;      }  </style>
<table id="showHeader" style="display:none;"></table>
<div>
   <div style="width: 100%;">
      <table width="100%">
         <tbody>
            <tr>
               <td> <img src="{{LogoUrl}}" style="height: 60px;" alt="blog-cong-dong" /> </td>
               <td style="padding-left: 50px;">
                  <div style="text-align:center;float:right;">
                     <img style="height: 40px;"                                  src="data:image/png;base64,{{BarCodeImgBase64}}"><br>                              
                     <p style="margin:0;padding:0">Mã TN: {{MaTN}}</p>
                  </div>
               </td>
            </tr>
         </tbody>
      </table>
   </div>
</div>
<table style="padding: 5px;width: 100%;">
   <th> <span style="font-size: 28px;">PHIẾU CHỈ ĐỊNH CẬN LÂM SÀNG</span><br> <span id="laCapCuu"              style="text-transform: uppercase;">{{CapCuu}}</span></th>
</table>
<div>
   </br>      
   <table style="width:100%">
      <tbody>
         <tr>
            <td colspan="2" width="50%">Tên bệnh nhân: <b>{{HoTen}}</b> </td>
            <td colspan="2" width="50%" align="right">Năm sinh: <b>{{NamSinh}}</b>&nbsp;&nbsp;&nbsp; Giới tính: <b>{{GioiTinhString}}</b>                  </td>
         </tr>
         <tr id="NguoiGiamHo">
            <td colspan="2">Người giám hộ: <b>{{NguoiGiamHo}}</b> </td>
            <td colspan="2" align="center">Quan hệ: <b>{{TenQuanHeThanNhan}}</b> </td>
         </tr>
         <tr>
            <td colspan="4"> Địa chỉ: <b>{{DiaChi}}</b> </td>
         </tr>
         <tr>
            <td colspan="2">Đối tượng: <b>{{DoiTuong}}</b></td>
            <td colspan="2">Số BHYT: <b>{{SoTheBHYT}}</b> </td>
         </tr>
         <tr>
            <td colspan="4">Nơi yêu cầu: <b>{{NoiYeuCau}}</b> </td>
         </tr>
         <tr>
            <td colspan="2" valign="top"> Chẩn đoán sơ bộ: <b>{{ChuanDoanSoBo}}</b> </td>
            <td  olspan="2"> Diễn giải: <b>{{DienGiai}}</b> </td>
         </tr>
         <tr>
            <td colspan="2">
               <div class="container">
                  <div class="label">Người lấy mẫu:&nbsp; </div>
                  <div class="value breakword" style="width:78%">
                     <div class="content">{{NguoiLayMau}}</div>
                  </div>
               </div>
            </td>
            <td colspan="2">
               <div class="container">
                  <div class="label">Thời gian lấy mẫu:&nbsp; </div>
                  <div class="value breakword" style="width:78%">
                     <div class="content">{{ThoiGianLayMau}}</div>
                  </div>
               </div>
            </td>
         </tr>
         <tr>
            <td colspan="4"> Ghi chú: <b>{{GhiChuCanLamSang}}</b> </td>
         </tr>
      </tbody>
   </table>
</div>
{{DanhSachDichVu}} 
<table style="width:100%;padding-top: 25px;padding-right: 100px;">
   <tr>
      <td></td>
      <td style="text-align: right;">
         <div style="text-align:center;float:right">
            <p> Ngày {{Ngay}} tháng {{Thang}} năm {{Nam}}</p>
            <p><b>Bác sĩ chỉ định</b></p>
            <p style="margin-top:30px;margin-bottom:90"></p>
            <p><b>{{NguoiChiDinh}}</b></p>
         </div>
      </td>
   </tr>
</table>
<div class="footer"> {{NgayThangNam}} </div>' where Name=N'PhieuChiDinh';
Go
Update dbo.CauHinh
Set [Value] = '3.6.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'