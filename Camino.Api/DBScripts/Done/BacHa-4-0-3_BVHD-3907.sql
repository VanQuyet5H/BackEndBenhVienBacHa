INSERT INTO template (Name, Title,
Body,TemplateType,Language,Description,Version,IsDisabled,CreatedById,LastUserId,LastTime,CreatedOn)
VALUES (N'GiayCamKetGayTeGiamDauTrongDeSauPhauThuat', N'Giấy cam kết gây tê giảm đau trong đẻ sau phẫu thuật', N'<style>
    table,
    th,
    td {
        border-collapse: collapse;
        font-family: Times New Roman;
        font-size: 16.5px;
    }

    th,
    td {
        padding: 1px;
    }

    #victims {
        width: 100%;
        border: 1px solid black;
    }

    #customers {
        width: 100%;
        border: 1px solid black;
    }

    #customers td,
    #customers th {
        border: 1px solid black;
    }

    #customers th {
        color: black;
    }

    #victimsborderleft {
        border-right: 1px solid black;
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

    /* // */
    .containerGD {
        width: 50%;
        display: table;
    }

    .containerGD .label {
        width: max-content;
        white-space: nowrap;
    }

    .containerGD .value {
        display: table-cell;
        width: 100%;
        height: 100%;
        vertical-align: top;
        position: relative;
        box-sizing: border-box;
        border-bottom: 1px dotted black;
    }

    span.square {
        vertical-align: bottom;
        border: solid 1px;
        width: 25px;
        height: 20px;
        margin: 2px;
        display: inline-block
    }

    .xuongDong {
        word-break: break-work;
    }
</style>
<div style="width: 100%;">
    <table width="100%">
        <tr>
            <td
                style="vertical-align: top;font-size: 16px;padding: 0px 0px 0px 0px;width: 40%;font-family: Times New Roman;">
                BỆNH VIỆN ĐKQT BẮC HÀ <br><b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{{KhoaCreate}}</b> </td>
            <td
                style="vertical-align: top;text-align: center;width: 60%;padding: 0px 0px 0px 0px;font-size: 16px;font-family: Times New Roman;">
                <b>CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM </b> <br><b><u> Độc lập- Tự do- Hạnh phúc</u></b>
            </td>
            <td style="vertical-align: top;text-align: right;width: 15%;padding: 0px 0px 0px 0px;"><img
                    style="height:30px;float: right;padding: 0px;" src="data:image/png;base64,{{BarCodeImgBase64}}">
            </td>
        </tr>
        <tr>
            <td style="font-size: 16px;padding: 0px 0 px 0px 0px;width: 18%;font-family: Times New Roman;">
            </td>
            <td
                style="text-align: center;width: 67%;padding: 0px 0px 0px 0px;font-size: 16px;font-family: Times New Roman;">
            </td>
            <td
                style="text-align: center;width: 5%;padding: 0px 0px 0px 0px;font-size: 13px;font-family: Times New Roman;">
                Mã TN:{{MaTN}}</td>
        </tr>
    </table>
    <table width="100%" style="margin-top: 25px;">
        <tbody>
            <tr>
                <td style="font-size: 18px;text-align: center;"> <b>GIẤY CAM KẾT <br><span style="margin-top: 10px;">GÂY TÊ GIẢM ĐAU TRONG ĐẺ - SAU PHẪU THUẬT</span></b> </td>
            </tr>
        </tbody>
    </table>
    <br>
    <br>
    <table style="width:100%" style="margin-top: 20px;">
        <tr>
            <td colspan="2" style="width: 100%;"> {{TenToiLa}} </td>
        </tr>
        <tr>
            <td style="width: 50%;"> {{NamSinh}} </td>
            <td style="width: 50%;"> {{GioiTinh}} </td>
        </tr>
        <tr>
            <td style="width: 50%;"> {{CMND}} </td>
            <td style="width: 50%;"> {{CoQuanCap}} </td>
        </tr>
        <tr>
            <td style="width: 50%;"> {{DanToc}} </td>
            <td style="width: 50%;"> {{QuocTich}} </td>
        </tr>
        <tr>
            <td style="width: 50%;"> {{NgheNghiep}} </td>
            <td style="width: 50%;"> {{NoiLamViec}} </td>
        </tr>
        <tr>
            <td colspan="2" width="100%" class="xuongDong"> {{DiaChi}} </td>
        </tr>
        <tr>
            <td style="width: 50%;"> {{KhiCanBaoTin}} </td>
            <td style="width: 50%;"> {{NamSinhNguoiThan}} </td>
        </tr>
        <tr>
            <td colspan="2" width="100%" class="xuongDong"> {{DiaChiNguoiThan}} </td>
        </tr>
        <tr>
            <td colspan="2" width="100%" class="xuongDong"> {{CMNDNguoiThan}} </td>
        </tr>
        <tr>
            <td colspan="2" width="100%" class="xuongDong"> {{DTLienLac}} </td>
        </tr>
    </table>
    <table style="width:100%">
        <tr>
            <td width="100%" colpan="2"> {{NguoiThan}} </td>
        </tr>
        <tr>
            <td width="100%" colpan="2">
                {{Khoa}}
            </td>
        </tr>
        <tr>
            <td width="100%" colpan="2" class="xuongDong">
                1. Sau khi được nghe bác sỹ Gây mê hồi sức giải thích về việc sử dụng phương pháp gây tê giảm đau 
                trong đẻ và sau phẫu thuật:<br>
                - Ưu điểm của phương pháp gây tê giảm đau trong đẻ và sau phẫu thuật.<br>
                - Những tai biến có thể xảy ra trong quá trình gây tê giảm đau trong đẻ và sau phẫu thuật.<br>
                2. Tôi đã hiểu và đồng ý để bác sỹ Gây mê hồi sức thực hiện bổ sung thêm phương pháp hoặc thêm <br>
                can thiệp trong gây mê/gây tê để hỗ trợ làm giảm đau trong đẻ và sau phẫu thuật cho tôi/người nhà<br> 
                tôi.<br>
                3. Tôi xin chấp hành đầy đủ nội quy/quy định khám bệnh,chữa bệnh của Bệnh viện, tôi yên tâm điều 
                trị. Tôi/gia đình tôi chịu trách nhiệm hoàn toàn với quyết định này, không có thắc mắc,khiếu kiện gì.

            </td>
        </tr>
    </table>
    <br>
    <br>
    <table style="width: 100%;">
        <tr>
            <td style="width: 50%;text-align:center"></td>
            <td style="width: 100%;text-align:right;font-style:italic;padding: 0px 0px 0px 0px">Bắc Hà,
                {{NgayThangNam}}&nbsp;&nbsp;&nbsp;</b></td>
        </tr>
    </table>
    <table style="width: 100%;">
        <tr>
            <td style="width: 50%;text-align:center"><b>NGƯỜI BỆNH/ĐẠI DIỆN NB</b></td>
            <td style="width: 50%;text-align:center"><b>BÁC SỸ GÂY MÊ HỒI SỨC</b></td>
        </tr>
        <tr>
            <td style="width: 50%;text-align:center;font-style:italic;">(Ký và ghi rõ họ tên)</td>
            <td style="width: 50%;text-align:center;font-style:italic;">(Ký và ghi rõ họ tên)</td>
        </tr>
    </table> <br> <br> <br> <br>
    <table style="width: 100%;padding-top:80px;">
        <tr>
            <td style="width: 50%;text-align:center"><b>{{NguoiDaiDien}}</b></td>
            <td style="width: 50%;text-align:center"><b>{{BSGayMeHoiSuc}}</b></td>
        </tr>
    </table>
</div>',1,1,N'Giấy cam kết gây tê giảm đau trong đẻ sau phẫu thuật',1,0,1,1,GETDATE(),GETDATE())


UPDATE CauHinh
Set [Value] = '4.0.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'