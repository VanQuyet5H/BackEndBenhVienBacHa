


INSERT INTO template  ([Name], Title, Body, TemplateType,[Language],[Description],[Version],IsDisabled,CreatedById,LastUserId,LastTime,CreatedOn)
VALUES (N'GiayCamKetSuDungThuocNgoaiBHYT', N'Giấy cam kết sử dụng thuốc ngoài BHYT', N'<style>
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
                BỆNH VIỆN ĐKQT BẮC HÀ <br><b>{{KhoaCreate}}</b> </td>
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
                <td style="font-size: 18px;text-align: center;"> <b>GIẤY CAM KẾT <br><span style="margin-top: 10px;">SỬ
                            DỤNG THUỐC NGOÀI
                            BẢO HIỂM Y TẾ </span></b> </td>
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
            <td colspan="2" width="100%" class="xuongDong"> {{DiaChi}} </td>
        </tr>
        <tr>
            <td colspan="2" width="100%" class="xuongDong"> {{ChanDoan}} </td>
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
                Sau khi nghe nhân viên y tế giải thích về vấn đề sử dụng thuốc tự nguyện (thuốc không có trong danh
                mục BHYT chi
                trả) để đáp ứng
                cho việc điều trị bệnh. Tôi hoàn toàn đồng ý, tự nguyện sử dụng thuốc
                ngoài danh mục BHYT trong suốt thời gian
                điều trị. Tôi
                không có thắc mắc, khiếu kiện gì và chịu trách
                nhiệm hoàn toàn với quyết định này.

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
            <td style="width: 50%;text-align:center"></td>
            <td style="width: 50%;text-align:center"><b>NGƯỜI BỆNH/ĐẠI DIỆN NGƯỜI BỆNH</b></td>
        </tr>
        <tr>
            <td style="width: 50%;text-align:center;font-style:italic;"></td>
            <td style="width: 50%;text-align:center;font-style:italic;">(Ký và ghi rõ họ tên)</td>
        </tr>
    </table> <br> <br> <br> <br>
    <table style="width: 100%;padding-top:80px;">
        <tr>
            <td style="width: 50%;text-align:center"></b></td>
            <td style="width: 50%;text-align:center"><b>{{NguoiDaiDien}}</b></td>
        </tr>
    </table>
</div>', 1,1,N'Giấy cam kết sử dụng thuốc ngoài BHYT',1,0,1,1,GETDATE(),GETDATE());


update template 
set Body=N'<style>
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
                BỆNH VIỆN ĐKQT BẮC HÀ <br><b>{{KhoaCreate}}</b> </td>
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
                <td style="font-size: 18px;text-align: center;"> <b>GIẤY CAM KẾT <br><span style="margin-top: 10px;">SỬ
                            DỤNG THUỐC NGOÀI
                            BẢO HIỂM Y TẾ </span></b> </td>
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
            <td colspan="2" width="100%" class="xuongDong"> {{DiaChi}} </td>
        </tr>
        <tr>
            <td colspan="2" width="100%" class="xuongDong"> {{ChanDoan}} </td>
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
                Sau khi nghe nhân viên y tế giải thích về vấn đề sử dụng thuốc tự nguyện (thuốc không có trong danh
                mục BHYT chi
                trả) để đáp ứng
                cho việc điều trị bệnh. Tôi hoàn toàn đồng ý, tự nguyện sử dụng thuốc
                ngoài danh mục BHYT trong suốt thời gian
                điều trị. Tôi
                không có thắc mắc, khiếu kiện gì và chịu trách
                nhiệm hoàn toàn với quyết định này.

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
            <td style="width: 50%;text-align:center"></td>
            <td style="width: 50%;text-align:center"><b>NGƯỜI BỆNH/ĐẠI DIỆN NGƯỜI BỆNH</b></td>
        </tr>
        <tr>
            <td style="width: 50%;text-align:center;font-style:italic;"></td>
            <td style="width: 50%;text-align:center;font-style:italic;">(Ký và ghi rõ họ tên)</td>
        </tr>
    </table> <br> <br> <br> <br>
    <table style="width: 100%;padding-top:80px;">
        <tr>
            <td style="width: 50%;text-align:center"></b></td>
            <td style="width: 50%;text-align:center"><b>{{NguoiDaiDien}}</b></td>
        </tr>
    </table>
</div>'

where Name='GiayCamKetSuDungThuocNgoaiBHYT'




Update dbo.CauHinh
Set [Value] = '3.7.8' where [Name] = 'CauHinhHeThong.DatabaseVesion'