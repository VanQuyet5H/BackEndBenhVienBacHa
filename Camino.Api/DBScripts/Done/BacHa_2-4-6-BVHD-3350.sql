UPDATE Template
SET Body = N'{{Header}}         
<style>   table, th, td    {     font-family: Times New Roman;        }    table tbody td {    vertical-align: top;   }    .footer {          position: fixed;          left: 0;          bottom: 0;          width: 100%;          margin-left: 10px;          text-align: left;      }        .boder-cell {          border: 1px solid black;      }        .margin-center {          margin-left: auto;          margin-right: auto;      }  table.k-table{    border-collapse: collapse;     width:100%;  }     table.k-table td{        border: 1px solid black;        border-collapse: collapse;        width:50px;        height:50px;        margin:1px 1px 1px 1px;  }  </style>
</style>    
<div style="margin-left:2cm">
   <div >
      <div style="width: 100%;">
         <table width="100%">
            <tbody>
               <tr>
                  <td >                          <img src="{{LogoUrl}}" style="height: 2cm;" alt="lo-go"/>                      </td>
                  <td style="padding-left: 50px;">
                     <div style="text-align:center;float:right;">
                        <img style="height: 40px;" src="data:image/png;base64,{{BarCodeImgBase64}}"><br>                                                                                                                         
                        <p style="margin:0;padding:0">{{MaTN}}</p>
                     </div>
                  </td>
               </tr>
            </tbody>
         </table>
      </div>
   </div>
   <table style="padding: 5px;width: 100%;margin-top:2cm">
      <th>          <span style="font-size: 28px;">KẾT QUẢ KHÁM SỨC KHỎE</span><br>      </th>
   </table>
   </br>         
   <table style="width:100%">
      <tbody>
         <tr>
            <td width="40%">Họ tên: <b style="text-transform: uppercase;">{{HoTen}}</b>     </td>
            <td>Giới tính: <b>{{GioiTinh}}</b>     </td>
            <td>Năm sinh: <b>{{NamSinh}}</b>     </td>
         </tr>
         <tr>
            <td width="40%">Mã KH: {{MaKhachHang}}     </td>
            <td colspan="2">Bộ phận: <b>{{KhachHangDoanhNghiep}}</b>     </td>
         </tr>
         <tr>
            <td colspan="3">Đơn vị: <b style="text-transform: uppercase;">{{DonVi}}</b>     </td>
         </tr>
         <tr>
            <td colspan="3">DHST: {{DHST}}     </td>
         </tr>
      </tbody>
   </table>
   {{DanhSachDichVuKham}}  <br>  {{DanhSachDichVuKyThuat}}        
   <table id="dichVuKyThuat" style="width:100%;">
      <tbody>
         <tr>
            <td width="15%"><b style="text-transform: uppercase;">Kết Luận: </b>     </td>
            <td>{{KetLuan}}</b>     </td>
         </tr>
         <tr>
            <td><b style="text-transform: uppercase;">Đề nghị: </b>     </td>
            <td>{{DeNghi}}</td>
         </tr>
         <tr>
            <td><b style="text-transform: uppercase;">Sức khỏe: </b>     </td>
            <td>{{PhanLoaiSucKhoe}}     </td>
         </tr>
      </tbody>
   </table>
   <table style="width:100%;padding-top: 25px;padding-right: 100px;">
      <tr>
         <td></td>
         <td style="text-align: right;" >
            <div style="text-align:center;float:right">
               <p>Ngày {{Ngay}} tháng {{Thang}} năm {{Nam}}</p>
               <p><b>Bác sĩ kết luận hồ sơ</b></p>
               <p style="margin-top:30px;margin-bottom:60px"></p>
               <p><b>{{BacSiKetLuanHoSo}}</b></p>
            </div>
         </td>
      </tr>
   </table>
   <i>Nếu còn điều gì chưa rõ, xin vui lòng liên hệ với chúng tôi để được phục vụ!</i>          
   <div class="footer">
      <ul style="list-style-type:decimal">       *Ghi chú: phân loại khám sức khỏe: Loại 1: rất khỏe; Loại 2: Khỏe; Loại 3: Trung bình; Loại 4: Yếu; Loại 5: Rất yếu.  </i>     </ul>
   </div>
</div>'
WHERE Id=30254;


Update dbo.CauHinh
Set [Value] = '2.4.6' where [Name] = 'CauHinhHeThong.DatabaseVesion'