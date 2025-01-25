update Template
set Body=N'<style>
    table,
    th,
    td {
        border-collapse: collapse;
        font-family: Times New Roman;
    }

    th,
    td {
        padding: 3px;
        font-size: 16px;
    }

    .breakword {
        word-break: break-all;
    }

    .container {
        width: 100%;
        display: table;
    }

    .container .label {
        width: max-content;
        white-space: nowrap;
    }

    .container .value {
        display: table-cell;
        width: 100%;
        height: 100%;
        vertical-align: top;
        position: relative;
        box-sizing: border-box;
        border-bottom: 1px dotted black;
    }

    .container .values {
        display: table-cell;
        width: 100%;
        height: 100%;
        vertical-align: top;
        position: relative;
        box-sizing: border-box;
    }
</style>
<table width="100%" style="margin-bottom:0;">
    <tr>
        <td style="vertical-align: top;font-size: 15px;"> BV ĐKQT BẮC HÀ <br> <b>{{KhoaDangIn}}</b></td>
        <td style="text-align: center;font-weight: bold;font-size:15px;vertical-align: top;"> <span
                style="padding-top: 5px;"><b>PHIẾU SƠ KẾT 15 NGÀY ĐIỀU
                    TRỊ</b><br> <span style="font-size: 13px;font-weight: normal;"><i> Từ ngày:{{TuNgay}}- đến ngày:{{DenNgay}}</i></span>
        </td>
        <td style="text-align:center;float:right;vertical-align: top;font-size: 15px;"> <img
                style="height:45px;width:162px;" src="data:image/png;base64,{{BarCodeImgBase64}}"><br>
            <p style="margin:0;padding:0;">Mã TN:{{MaTN}}</p>
        </td>
    </tr>
</table> <br>
<table width="100%">
    <tr>
        <td style="width: 60%;">
            {{HoTenNgBenh}}
        </td>
        <td style="width: 25%;">
            {{TuoiNgBenh}}
        </td>
        <td style="width: 15%;">
            {{GT}}
        </td>
    </tr>
    <tr>
        <td colspan="3" style="width: 100%;">
            {{DiaChi}}
        </td>
    </tr>
    <tr>
        <td style="width: 60%;">
            {{Khoa}}
        </td>
        <td style="width: 20%;">
            {{Buong}}
        </td>
        <td style="width: 20%;">
            {{Giuong}}
        </td>
    </tr>
    <tr>
        <td colspan="3" style="width: 100%;">
            {{ChanDoan}}
        </td>
    </tr>
    <tr>
        <td colspan="3" style="width: 100%;">
            {{DienBienLS}}
        </td>
    </tr>
    <tr>
        <td colspan="3" style="width: 100%;">
            {{XetNghiemCLS}}
        </td>
    </tr>
    <tr>
        <td colspan="3" style="width: 100%;">
            {{QuaTrinhDieuTri}}
        </td>
    </tr>
    <tr>
        <td colspan="3" style="width: 100%;">
            {{DanhGiaKetQua}}
        </td>
    </tr>
    <tr>
        <td colspan="3" style="width: 100%;">
            {{HuongDieuTriTiep}}
        </td>
    </tr>
</table> <br>
<table width="100%">
    <tr>
        <td style="text-align:center;" width="50%"><i>Ngày {{NgayK}} tháng {{ThangK}} năm {{NamK}}</i></td>
        <td></td>
        <td style="text-align:center;" width="45%"><i>Ngày {{Ngay}} tháng {{Thang}} năm {{Nam}}</i></td>
    </tr>
    <tr>
        <td style="text-align:center;" style="font-size: 18px;text-transform: uppercase;"><b>Trưởng khoa</b></td>
        <td></td>
        <td style="font-size: 18px; text-align:center;text-transform: uppercase;"><b>Bác sĩ điều trị</b></td>
    </tr>
    <tr>
        <td style="font-size: 18px;font-style: italic;text-align:center">(Ký và ghi rõ họ tên)</td>
        <td></td>
        <td style="font-size: 18px; text-align:center;font-style: italic;">(Ký và ghi rõ họ tên)</td>
    </tr>
    <tr>
        <td style="font-size: 18px; text-align:center;"><br><br><br><br><b>{{HoTenTruongKhoa}}</b></td>
        <td></td>
        <td style="font-size: 18px; text-align:center;"><br><br><br><br><b>{{HoTenBacSi}}</b></td>
    </tr>
</table>'

where Name=N'PhieuSoKet15NgayDieuTri'

Update dbo.CauHinh
Set [Value] = '3.8.8' where [Name] = 'CauHinhHeThong.DatabaseVesion'