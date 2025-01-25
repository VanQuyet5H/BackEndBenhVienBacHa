ALTER TABLE  DichVuKyThuatBenhVien ADD DichVuChuyenGoi bit NULL 

CREATE TABLE [dbo].[HDPP](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_HDPP] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE  DichVuXetNghiem ADD LaChuanISO bit NULL 
ALTER TABLE  DichVuXetNghiem ADD HdppId bigint NULL 

select * from Template where Name='PhieuKetQuaXetNghiemNew'

update template 
set Body =N'<head>
    <link href="http://fonts.googleapis.com/css?family=Roboto" rel="stylesheet" type="text/css">
    <style>
        html {
            font-family: Times, serif !important;
        }

        table#phieuInXetNgiem {
            width: 100%
        }

        table#phieuInXetNgiem,
        th,
        td {
            border-collapse: collapse;
            font-family: Times, serif;
            font-size: 17px;
            padding: 2px;
            border: none;
            /* width: 19cm; */
        }

        .footer {
            position: fixed;
            left: 0;
            bottom: 0;
            width: 100%;
            margin-left: 10px;
            text-align: left;
        }

        .font-18 td {
            font-size: 18px;
        }

        table#phieuInXetNgiem .content td,
        table#phieuInXetNgiem .content th {
            padding: 4px;
            border: 1px solid #000;
        }

        p {
            padding: 0;
            margin: 0px;
        }

        @page {
            size: A4;
            margin: 5mm;
        }

        h3.pag {
            display: none;
            position: absolute;
            page-break-before: always;
            page-break-after: always;
            bottom: 0;
            right: 0;
        }

        h3::before {
            position: relative;
            bottom: -15px;
        }

        .pagebreak {
            clear: both;
            page-break-after: always;
        }

        @media print {
            h3.pag {
                display: initial;
                font-size: 12px;
                font-weight: normal;
            }

            .print {
                display: none;
            }

            tfoot {
                display: table-row-group;
            }
        }

        .numberCircle {
            display: inline-block;
            line-height: 0px;
            border-radius: 50%;
            border: 2px solid;
            font-size: 13px;
        }

        .round-sttNhanVien {
            top: 0;
            left: 0;
            position: absolute;
            min-width: 60px;
            max-width: 120px;
            text-decoration: none;
            display: inline-block;
            outline: none;
            cursor: pointer;
            border-style: none;
            border: 2px solid;
            background-color: #fff;
            color: #333;
            font-size: 16px;
            border-radius: 100%;
            overflow: none;
            text-align: center;
            padding: 0;
            font-weight: bold;
        }

        .round-sttNhanVien:before {
            content: "";
            display: inline-block;
            vertical-align: middle;
            padding-top: 100%;
        }
    </style>
</head>
<div style="margin-left: 2cm;">
    <h3 class="pag pag1"></h3>
    <div class="insert"></div>
    <table id="phieuInXetNgiem">
        <thead>
            <tr>
                <td colspan="6">
                    <table width="100%" style="margin-bottom:0;">
                        <tr>
                            <td colspan="4" style="vertical-align: top;"> <img style="height:70px" src="{{LogoUrl}}"
                                    alt="benh-vien-da-khoa-quoc-te-bac-ha" /> {{STT}} </td>
                            <td style="vertical-align: top;"> <img style="height:70px" src="{{LogoBV1}}"
                                        alt="logo-iso1" />  </td>
                            <td style="vertical-align: top;"> <img style="height:70px" src="{{LogoBV2}}"
                                        alt="logo-iso2" /> </td>
                            <td>
                                <div style="width:200px; margin-left: auto;">
                                    <p>Số phiếu: <b>{{SoPhieu}}</b> </p>
                                    <p>Mã TN: <b>{{SoVaoVien}}</b> </p>
                                    <p>Mã NB: <b>{{MaYTe}}</b> </p> <img style="height:30px"
                                        src="data:image/png;base64,{{BarCodeImgBase64}}">
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="7">
                                <h1 style="text-align:center;font-size:25px;margin:0;margin-top:-10px;">PHIẾU KẾT QUẢ
                                    XÉT NGHIỆM</h1>
                            </td>
                        </tr>
                        <tr>
                            <td valign="top" colspan="5">Họ và tên: <b>{{HoTen}}</b> </td>
                            <td valign="top" style="width:150px;">SID: <b>{{Barcode}}</b> </td>
                            <td valign="top" style="width:280px;">Năm sinh: {{NamSinhString}} Giới tính:
                                {{GioiTinhDisplay}}</td>
                        </tr>
                        <tr>
                            <td valign="top" colspan="6">Địa chỉ: {{DiaChi}}</b> </td>
                            <td valign="top">Loại mẫu: {{LoaiMau}} </td>
                        </tr>
                        <tr>
                            <td valign="top" colspan="5">Đối tượng: <b>{{DoiTuong}}</b> </td>
                            <td valign="top" colspan="2">Mã số BHYT: {{MaBHYT}} </td>
                        </tr>
                        <tr>
                            <td valign="top" colspan="5">B/s chỉ định: {{BsChiDinh}} </td>
                            <td valign="top" colspan="2">Khoa/Phòng: {{KhoaPhong}} </td>
                        </tr>
                        <tr>
                            <td valign="top" colspan="5">TG lấy mẫu: {{TgLayMau}} </td>
                            <td valign="top" colspan="2">Người lấy mẫu: {{NguoiLayMau}} </td>
                        </tr>
                        <tr>
                            <td valign="top" colspan="5">TG nhận mẫu: {{TgNhanMau}} </td>
                            <td valign="top" colspan="2">Người nhận mẫu: {{NguoiNhanMau}} </td>
                        </tr>
                        <tr>
                            <td valign="top" colspan="5" style="word-break: break-word;">Chẩn đoán: <b>{{ChanDoan}}</b>
                            </td>
                            <td valign="top" colspan="2" style="word-break: break-word;">Diễn giải: <b>{{DienGiai}}</b>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr class="content font-18">
                <td style="font-weight: bold;width:200px;text-align:center;"> TÊN XÉT NGHIỆM </td>
                <td style="font-weight: bold;width:100px;text-align:center;"> KẾT QUẢ </td>
                <td style="font-weight: bold;width:100px;text-align:center;"> CSBT </td>
                <td style="font-weight: bold;width:100px;text-align:center;"> ĐƠN VỊ </td>
                <td style="font-weight: bold;width:100px;text-align:center;"> MÁY XN </td>
                <td style="font-weight: bold;width:100px;text-align:center;"> HDPP </td>
            </tr>
        </thead>
        <tbody class="content font-18">{{DanhSach}}</tbody>
    </table>
</div>'

where Name='PhieuKetQuaXetNghiemNew'

Update dbo.CauHinh
Set [Value] = '3.6.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'