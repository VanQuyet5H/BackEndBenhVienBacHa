







update Template
set body=N'<style>
    table,
    th,
    td {
        /* border-collapse: collapse; */
        font-family: Times New Roman;
        height: 5px;
    }

    th,
    td {
        padding: 0px;
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
        border-bottom: 0.5px dotted black;
        font-size: 16px;
        font-family: Times New Roman;
        border-bottom: 1px dotted black;
    }

    .container .values {
        display: table-cell;
        width: 100%;
        height: 100%;
        vertical-align: top;
        position: relative;
        box-sizing: border-box;
        font-size: 16px;
        font-family: Times New Roman;
    }

    span.square {
        border: solid 1px;
        width: 16px;
        height: 20px;
        text-align: center;
        margin: 2px;
        display: inline-block;
        margin-top: 2px;
    }
</style>
<div style="width: 100%;">
    <div style="float:left;width: 100%;">
        <div style="vertical-align: top;font-size: 15px;width: 25%;float: left;">BV ĐKQT BẮC HÀ <br>
            {{KhoaDangIn}}</div>
        <div style="text-align: center;font-weight: bold;font-size:15px;vertical-align: top;width: 50%;float: left">
            <b>CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM</b><br> <b><u>Độc lập - Tự
                    do - Hạnh phúc</u></b> </div>
        <div style="text-align:right;;vertical-align: top;font-size: 15px;width: 25%;float: left"><img
                style="height:45px;" src="data:image/png;base64,{{BarCodeImgBase64}}"><br>
            <p style="margin:0;padding:0;text-align: center;"><span style="margin-left: 31px;"></span>Mã TN: {{MaTN}} <br>
                    &nbsp;&nbsp;MaBN: {{MaBN}}</p>
        </div>

    </div>

    <table width="100%" style="margin-top: 25px;margin-bottom:0;">
        <tr>
            <td style="float: right;font-style: italic;font-size: 16px;">{{NgayThangNamPhieu}}</td>
        </tr>
    </table>
    <table width="100%" style="margin-top: 11px;margin-bottom:0;">
        <tr>
            <td style="float: right;font-style: normal;font-size: 16px;">{{SoVaoVien}}</td>
        </tr>
    </table>
    <div style="width:100%;text-align: center;height: 50px;font-weight: bold;font-size:18px;"> BIÊN BẢN HỘI CHẨN CHUYÊN
        MÔN </div>
    <table style="width:100%">
        <tr>
            <td colspan="2" style="width: 100%;font-size:16px;font-family: Times New Roman;"> {{HoTenNguoiBenh}} </td>
        </tr>
    </table>
    <table style="width:100%">
        <tr>
            <td style="width: 50%;font-size:16px;font-family: Times New Roman;"> {{NgayThangNamSinh}} </td>
            <td style="width: 50%;font-size:16px;font-family: Times New Roman;"> {{GioiTinh}} </td>
        </tr>
    </table>
    <table style="width:100%">
        <tr>
            <td style="width: 50%;font-size:16px;font-family: Times New Roman;"> {{DaDieuTriTuNgay}} </td>
            <td style="width: 50%;font-size:16px;font-family: Times New Roman;"> {{DaDieuTriDenNgay}} </td>
        </tr>
    </table>
    <table style="width:100%">
        <tr>
            <td style="width: 20%;font-size:16px;font-family: Times New Roman;"> {{Giuong}} </td>
            <td style="width:30%;font-size:16px;font-family: Times New Roman;"> {{Buong}} </td>
            <td style="width: 50%;font-size:16px;font-family: Times New Roman;"> {{Khoa}} </td>
        </tr>
        <tr>
            <td colspan="3" style="width: 100%;font-size:16px;font-family: Times New Roman;"> {{ChanDoan}} </td>
        </tr>
        <tr>
            <td colspan="3" style="width: 100%;font-size:16px;font-family: Times New Roman;">
                <div class="container">
                    <div class="label">- Hội chẩn thông qua mổ lúc: {{HoiChanThongQuaMo}}</div>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="3" style="width: 100%;font-size:16px;font-family: Times New Roman;"> {{ChuToa}} </td>
        </tr>
        <tr>
            <td colspan="3" style="width: 100%;font-size:16px;font-family: Times New Roman;"> {{ThuKy}} </td>
        </tr> {{ThanhVien}} {{TomTat}} {{CacXNCoBan}} {{XNKhac}} <tr>
            <td colspan="3" style="width: 100%;font-size:16px;font-family: Times New Roman;"> {{KetLuan}} </td>
        </tr>
        <tr>
            <td colspan="3" style="width: 100%;font-size:16px;font-family: Times New Roman;"> {{DuKienPhauThuat}} </td>
        </tr>
        <tr>
            <td colspan="3" style="width: 100%;font-size:16px;font-family: Times New Roman;"> {{NhomPhauThuat}} </td>
        </tr>
        <tr>
            <td colspan="3" style="width: 100%;font-size:16px;font-family: Times New Roman;"> {{NhomGayMe}} </td>
        </tr>
        <tr>
            <td colspan="3" style="width: 100%;font-size:16px;font-family: Times New Roman;"> {{PhuongPhapGayMe}} </td>
        </tr> {{NgayGioPhauThuat}} <tr>
            <td colspan="3" style="width: 100%;font-size:16px;font-family: Times New Roman;"> {{DuKienKhac}} </td>
        </tr>
    </table>
    <table style="width:100%;margin-top: 20px;">
        <tr>
            <td
                style="font-size:16px;font-family: Times New Roman;text-transform: uppercase;text-align: center;font-weight: bold;">
                Thành viên </td>
            <td
                style="font-size:16px;font-family: Times New Roman;text-transform: uppercase;text-align: center;font-weight: bold;">
                THƯ KÝ </td>
            <td
                style="font-size:16px;font-family: Times New Roman;text-transform: uppercase;text-align: center;font-weight: bold;">
                CHỦ TOẠ </td>
        </tr>
        <tr>
            <td style="font-size:16px;font-family: Times New Roman;font-style: italic;text-align: center;"> (Ký và ghi
                rõ họ tên) </td>
            <td style="font-size:16px;font-family: Times New Roman;font-style: italic;text-align: center;"> (Ký và ghi
                rõ họ tên) </td>
            <td style="font-size:16px;font-family: Times New Roman;font-style: italic;text-align: center;"> (Ký và ghi
                rõ họ tên) </td>
        </tr>
        <tr>
            <td style="font-size:16px;font-family: Times New Roman;font-weight: bold;text-align: center;"> <br> <br>
                <br> <br> <br>
            </td>
            <td style="font-size:16px;font-family: Times New Roman;font-weight: bold;text-align: center;"> <br> <br>
                <br> <br> <br>
            </td>
            <td style="font-size:16px;font-family: Times New Roman;font-weight: bold;text-align: center;"> <br> <br>
                <br> <br> <br>
            </td>
        </tr>
        <tr>
            <td style="font-size:16px;font-family: Times New Roman;font-weight: bold;text-align: center;">
                <div style="word-break: break-work;font-size: 16px;">{{ChuKyThanhVien}}</div>
            </td>
            <td style="font-size:16px;font-family: Times New Roman;font-weight: bold;text-align: center;">
                <div style="word-break: break-work;">{{ChuKyThuKy}}</div>
            </td>
            <td style="font-size:16px;font-family: Times New Roman;font-weight: bold;text-align: center;">
                <div style="word-break: break-work;">{{ChuKyChuToa}}</div>
            </td>
        </tr>
    </table>'
where Name='BienBanHoiChanChuyenMon'



Update dbo.CauHinh
Set [Value] = '3.4.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'