ALTER TABLE [dbo].[NoiTruChiDinhDuocPham]
ADD
	[ThoiGianDienBien] [datetime] NULL
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat]
ADD
	[ThoiGianDienBien] [datetime] NULL
GO
ALTER TABLE [dbo].[YeuCauKhamBenh]
ADD
	[ThoiGianDienBien] [datetime] NULL
GO
ALTER TABLE [dbo].[YeuCauTruyenMau]
ADD
	[ThoiGianDienBien] [datetime] NULL
GO
INSERT [dbo].[CauHinh] ([Name], [DataType], [Value], [Description], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) 
  VALUES (N'CauHinhChung.DuocPhamBenhVienPhanNhomSinhPhamIds',3, N'50;115', N'Nhóm Sinh Phẩm', 1, 1,GETDATE(),GETDATE())
GO
update Template set body = N'<style>      table,      th,      td {          border-collapse: collapse;          font-family: Times New Roman;          font-size: 16px;      }        th,      td {          padding: 0px;      }        #victims {          width: 100%;          border: 1px solid black;      }
        #customers {          width: 100%;          border-bottom: 1px solid black;      }        
		#customers td {          border-right: 1px solid black;
        border-left: 1px solid black;      } 
		#customers th {          border: 1px solid black;      }        
		        #victimsborderleft {          border-right: 1px solid black;      }        .container {          width: 100%;          display: table;      }        .container {          width: 100%;          display: table;      }        .container .label {          width: max-content;      }        .container .value {          display: table-cell;          width: 100%;          height: 100%;          vertical-align: top;          position: relative;          box-sizing: border-box;      }        .container .value .content {          width: 100%;          border-bottom: 1px dotted gray;      }        .containerGD {          width: 55%;          display: table;      }        .containerGD .label {          width: max-content;      }        .containerGD .value {          display: table-cell;          width: 100%;          height: 100%;          vertical-align: top;          position: relative;          box-sizing: border-box;      }        .containerGD .value .content {          width: 100%;          border-bottom: 1px dotted gray;      }        span.square {          vertical-align: bottom;          border: solid 1px;          width: 25px;          height: 20px;          margin: 2px;          display: inline-block      }        .container .label {          width: max-content;          white-space: nowrap      }        .contain-grid {          padding: 5px;      }  </style> <br>  <div>      <div style="width: 100%;">          <table width="100%">              <tbody>                  <tr>                      <td style="font-size: 18px;padding-left: 5px; width:40%"> <b style="font-size: 18px;">BỆNH VIỆN ĐKQT                              BẮC HÀ</b>                          <div class="container">                              <div class="label">Khoa: {{Khoa}}</div>                          </div>                      </td>                      <td width="40%">                          <div class="containerGD">                              <div class="label"> <b style="font-size: 22px;"><span>TỜ ĐIỀU TRỊ</span></b> số:</div>                              <div class="value" style="width:15%">                                  <div class="content">&nbsp;{{So}}</div>                              </div>                          </div>                      </td>                      <td width="10%">                          <div class="container">                              <div class="label">MS:39/BV-01</div>                          </div>                          <div class="container">                              <div class="label">Số vào viện:</div>                              <div class="value"><b>&nbsp;{{SoVaoVien}}</b></div>                          </div>                      </td>                  </tr>              </tbody>          </table>      </div>  </div> <br> <br>  <table style="width:100%">      <tr>          <td colspan="2">              <div class="container">                  <div class="label">- Họ tên người bệnh:</div>                  <div class="value">&nbsp;<b>{{HoTen}}</b></div>              </div>          </td>          <td>              <div class="container">                  <div class="label">Tuổi:</div>                  <div class="value">&nbsp;{{Tuoi}}</div>                  <div class="label">{{GioiTinh}}</div>              </div>          </td>      </tr>  </table>  <table style="width:100%">      <tr>          <td style="width: 50%;">              <div class="container">                  <div class="label">- Buồng:</div>                  <div class="value">&nbsp;{{Buong}}</div>              </div>          </td>          <td>              <div class="container" style="width:50%;">                  <div class="label">Giường:</div>                  <div class="value">&nbsp;{{Giuong}}</div>              </div>          </td>      </tr>      <tr>          <td colspan="4">              <div class="container">                  <div class="label">- Chẩn đoán:</div>                  <div class="value">&nbsp;{{ChanDoan}}</div>              </div>          </td>      </tr>  </table>  <table id="customers">      <tr>          <th> <b>NGÀY<br>GIỜ </b> </th>          <th style=" width: 35%"> <b> DIỄN BIẾN BỆNH </b> </th>          <th style=" width: 55%"> <b> Y LỆNH </b> </th>      </tr>      <tbody> {{ToDieuTri}} </tbody>  </table>' 
				where [Name] ='ToDieuTri'
GO
UPDATE CauHinh
Set [Value] = '4.0.8' where [Name] = 'CauHinhHeThong.DatabaseVesion'