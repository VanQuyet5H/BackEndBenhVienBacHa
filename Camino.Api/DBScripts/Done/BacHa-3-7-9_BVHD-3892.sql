update template
set Body =N'<style>
    table,
    th,
    td {
        border-collapse: collapse;
        font-family: Times New Roman;
        font-size: 16.5px;
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

<div style="width: 100%;">
    <table width="100%">
        <tr>
            <td
                style="vertical-align: top;font-size: 16px;padding: 0px 0px 0px 0px;width: 40%;font-family: Times New Roman;">
                BỆNH VIỆN ĐKQT BẮC HÀ <br><b>{{KhoaCreate}}</b> </td>
            <td
                style="vertical-align: top;text-align: center;width: 60%;padding: 0px 0px 0px 0px;font-size: 18px;font-family: Times New Roman;">
                <b>PHIẾU TÓM TẮT THÔNG TIN <br> ĐIỀU TRỊ VÀ CÁC
                    DỊCH VỤ</b>
            </td>
            <td style="vertical-align: top;text-align: right;width: 15%;padding: 0px 0px 0px 0px;"><img
                    style="height:40px;width:175px;float: right;padding: 0px;" src="data:image/png;base64,{{BarCodeImgBase64}}" >
                    <br>
                    Mã TN:{{MaTN}} &nbsp;&nbsp;&nbsp;&nbsp;
            </td>
        </tr>
    </table>
</div>
<br>
<table width="100%">
    <tr>
        <td style="width:100%;word-break: break-work;" colspan="3">
            {{HoTenNgBenh}}
        </td>
    </tr>
    <tr>
        <td style="width:50%;word-break: break-work;">
            {{NgayThangNamSinh}}
        </td>
        <td style="width:50%;word-break: break-work;" colspan="2">
            {{GTNgBenh}}
        </td>
    </tr>
    <tr>
        <td style="width:100%;word-break: break-work;" colspan="3">
            {{DiaChi}}
        </td>
    </tr>

    <tr>
        <td style="width:50%;word-break: break-work;">
            {{Khoa}}
        </td>
        <td style="width:35%;word-break: break-work;">
            {{Buong}}
        </td>
        <td style="width:15%;word-break: break-work;">
            {{Giuong}}
        </td>
    </tr>
    {{ChanDoan}}
    <tr>
        <td style="width:100%;word-break: break-work;" colspan="3">
            <div class="container">
                <div class="label">- Dự kiến phương pháp điều trị: <i>(Phải phẫu thuật, thủ thuật thăm dò chức năng,
                        thuốc, vv...)</i> </div>
            </div>
        </td>
    </tr>
    {{DuKienPPDieuTri}}

    {{TienLuong}}
    {{NhungDieuCanLuuY}}
    <tr>
        <td style="width:100%;word-break: break-work;" colspan="3">
            <div class="container">
                <div class="label">- Bệnh nhân sử dụng thẻ BHYT <i>Quyền lợi được BHYT chi trả theo qui định</i> </div>
            </div>
        </td>
    </tr>
    {{ThongTinVeGiaDV}}
    <tr>
        <td style="width:100%;word-break: break-work;" colspan="3">
            <div class="container">
                <div class="label">- Ý kiến của người bệnh/ người nhà người bệnh:</i> </div>
            </div>
        </td>
    </tr>
    <tr>
        <td style="width:100%;word-break: break-work;" colspan="3">
            <div class="container">
                <div class="label">&nbsp;</i> </div>
            </div>
        </td>
    </tr>
    <tr>
        <td style="width:100%;word-break: break-work;" colspan="3">
            <div class="container">
                <div class="label">&nbsp;</i> </div>
            </div>
        </td>
    </tr>
</table>

<table width="100%">
    <tr>
        <td width="25%"></td>
        <td style="text-align:center;" width="25%"><i>Ngày {{Ngay}} tháng {{Thang}} năm {{Nam}}</i></td>
    </tr>
    <tr>
        <td style="text-align:center;text-transform: uppercase;" width="25%"><b>Người bệnh</b></td>
        <td style="text-align:center;text-transform: uppercase;" width="25%"><b>Bác sĩ điều trị</b></td>
    </tr>
    <tr>
        <td style="text-align:center;font-style: italic;">(Ký và ghi rõ họ tên)</td>
        <td style="text-align:center;font-style: italic;">(Ký và ghi rõ họ tên)</td>
    </tr>
    <td style="text-align:center;width: 25;font-weight: bold;"><br><br><br><br>{{NguoiBenh}}</td>
    <td style="text-align:center;width: 25;font-weight: bold;"><br><br><br><br>{{HoTenBacSi}}</td>
    </tr>
</table>'
where Name='PhieuTTTTDieuTriVaCacDichVu'

Update dbo.CauHinh
Set [Value] = '3.7.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'