update template 
set Body=N'<style>
    table,
    th,
    td {
        border-collapse: collapse;
        font-family: Times New Roman;
        font-size: 14px;
    }

    th,
    td {
        padding: 5px;
    }

    .container {
        width: 100%;
        display: table;
    }

    .container .label {
        width: max-content;
        white-space: nowrap
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
</style>
<div style="width: 100%;">
    <div style="width: 100%;">
        <div> <br> <br>
            <div style="width: 100%;">
                <table width="100%">
                    <tr>
                        <td
                            style="vertical-align: top;font-size: 14px;padding: 0px 0px 0px 0px;width: 40%;font-family: Times New Roman;">
                            BỆNH VIỆN ĐKQT BẮC HÀ <br><b>{{KhoaCreate}}</b> </td>
                        <td
                            style="vertical-align: top;text-align: center;width: 60%;padding: 0px 0px 0px 0px;font-size: 16px;font-family: Times New Roman;">
                        </td>
                        <td style="vertical-align: top;text-align: right;width: 15%;padding: 0px 0px 0px 0px;"><img
                                style="height:30px;float: right;padding: 0px;"
                                src="data:image/png;base64,{{BarCodeImgBase64}}"> </td>
                    </tr>
                    <tr>
                        <td style="font-size: 13px;padding: 0px 0 px 0px 0px;width: 18%;font-family: Times New Roman;">
                        </td>
                        <td
                            style="text-align: center;width: 67%;padding: 0px 0px 0px 0px;font-size: 14px;font-family: Times New Roman;">
                        </td>
                        <td
                            style="text-align: right;width: 5%;padding: 0px 0px 0px 0px;font-size: 14px;font-family: Times New Roman;">
                            Mã TN:{{MaTN}}&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
                    </tr>
                </table>
                <table width="100%">
                    <tbody>
                        <tr>
                            <td style="font-size: 16px;text-align: center;"> <b>PHIẾU THỬ PHẢN ỨNG THUỐC </b> </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div> <br>
        <table style="width:100%">
            <tr>
                <td style="width: 50%;"> {{TenToiLa}} </td>
                <td style="width: 50%;"> {{NamSinh}} &nbsp; &nbsp;{{GioiTinh}}</td>
            </tr>
            <tr>
                <td width="50%"> {{SoGiuong}} </td>
                <td width="50%" colspan="2"> {{SoBuong}} </td>
            </tr>
            <tr>
                <td width="100%" colspan="2"> {{ChanDoan}} </td>
            </tr>
            <tr>
                <td width="100%" colspan="2"> {{BSChinhDinh}} </td>
            </tr>
        </table>
        <table style="width:100%;text-align: center;">
            <tr>
                <td style="width: 15%;"></td>
                <td style="width: 70%;">
                    <div style="border: 1px dotted black;height: 95px;text-align: justify;"> {{TenThuoc}} {{NuocSX}}
                        {{SoLo}}
                        <!-- <div style="padding-top: 13px;"><b>&nbsp;&nbsp;&nbsp;TÊN THUỐC: </b></div>                                                  <div style="padding-top: 13px;">&nbsp;&nbsp;&nbsp;Thuốc sản xuất tại nước: </div>                                                 <div style="padding-top: 13px;">&nbsp;&nbsp;&nbsp;Số lô sản xuất (in trên nhãn lọ): </div> -->
                    </div>
                </td>
                <td style="width: 15%;"></td>
            <tr>
        </table> <br>
        <table style="width:100%;text-align: center;">
            <tr>
                <td colspan="3" style="border: 1px solid black;border-bottom: none;"><b>KẾT QUẢ</b></td>
            <tr>
            <tr>
                <td colspan="3" style="border: 1px solid black;border-top: none;font-style: italic;">(Ghi rõ bằng chữ: Âm tính/Dương
                    tính/Nghi ngờ)</td>
            <tr>
            <tr>
                <td style="border: 1px solid black;border-bottom: none;font-weight: bold;">KHÔNG</td>
                <td style="border: 1px solid black;border-bottom: none;font-weight: bold;">CÓ</td>
                <td style="border: 1px solid black;border-bottom: none;font-weight: bold;">NGHI NGỜ</td>
            </tr>
            <tr>
                <td style="border: 1px solid black;border-top: none;font-weight: bold;">Phản ứng</td>
                <td style="border: 1px solid black;border-top: none;font-weight: bold;">Phản ứng</td>
                <td style="border: 1px solid black;border-top: none;;"></td>
            </tr>
            <tr>
                <td style="border: 1px solid black;height: 30px;">{{KetQuaPhanUng1}}</td>
                <td style="border: 1px solid black;">{{KetQuaPhanUng2}}</td>
                <td style="border: 1px solid black;">{{KetQuaPhanUng3}}</td>
            </tr>
        </table> <br>
        <table style="width:100%;">
            <tr>
                <td colspan="3"
                    style="border: 1px dotted black;border-bottom: none;text-align: center;font-size: 13px;"><b>CAM
                        KẾT</b></td>
            <tr>
            <tr>
                <td style="border: 1px dotted black;text-align: center;border-bottom: none;font-size: 13px;width: 34%;">
                    <b>CAM KẾT ĐỀ NGHỊ </b></td>
                <td style="border: 1px dotted black;text-align: center;border-bottom: none;font-size: 13px;width: 32%;">
                    <b>ĐIỀU DƯỠNG THỬ <br>PHẢN ỨNG </b></td>
                <td style="border: 1px dotted black;text-align: center;border-bottom: none;font-size: 13px;width: 34%;">
                    <b>BÁC SỸ ĐỌC PHẢN ỨNG</b></td>
            </tr>
            <tr>
                <td style="border: 1px dotted black;border-bottom: none;border-top: none;font-size: 13px;width: 34%;">
                    Tôi (gia đình tôi) đề nghị và <b>đồng ý/không đồng ý</b> thử phản ứng trước khi tiêm, sau khi được
                    nghe BS giải thích.</td>
                <td style="border: 1px dotted black;border-bottom: none;border-top: none;font-size: 13px;width: 32%;">
                    {{DieuDuongPhanUng}} <br>{{ChucDanhDieuDuongPhanUng}} </td>
                <td style="border: 1px dotted black;border-bottom: none;border-top: none;font-size: 13px;width: 34%;">
                    {{HoTenBSDocPhanUng}} <br> {{ChucDanhBSDocPhanUng}} </td>
            </tr>
            <tr>
                <td
                    style="border: 1px dotted black;border-top: none;border-bottom: none;text-align: center;font-size: 13px;font-style: italic;">
                    {{NgayGioThangnamCamKet}}</td>
                <td
                    style="border: 1px dotted black;border-top: none;border-bottom: none;text-align: center;font-size: 13px;font-style: italic;">
                    {{NgayGioDieuDuongPhanUngThu}}</td>
                <td
                    style="border: 1px dotted black;border-top: none;border-bottom: none;text-align: center;font-size: 13px;font-style: italic;">
                    {{NgayGioBSDocPhanUng}}</td>
            </tr>
            <tr>
                <td
                    style="border: 1px dotted black;border-top: none;border-bottom: none;text-align: center;font-size: 13px;font-style: italic;">
                    (Ký và ghi rõ họ tên)</td>
                <td
                    style="border: 1px dotted black;border-top: none;border-bottom: none;text-align: center;font-size: 13px;font-style: italic;">
                    (Ký và ghi rõ họ tên)</td>
                <td
                    style="border: 1px dotted black;border-top: none;border-bottom: none;text-align: center;font-size: 13px;font-style: italic;">
                    (Ký và ghi rõ họ tên)</td>
            </tr>
            <tr>
                <td
                    style="border: 1px dotted black;border-top: none;text-align: center;font-size: 13px;font-weight: bold;">
                    <br><br><br><br><br>{{NguoiViet}}<br><br></td>
                <td
                    style="border: 1px dotted black;border-top: none;text-align: center;font-size: 13px;font-weight: bold;">
                    <br><br><br><br><br>{{DieuDuongThuPhanUng}}<br><br></td>
                <td
                    style="border: 1px dotted black;border-top: none;text-align: center;font-size: 13px;font-weight: bold;">
                    <br><br><br><br><br>{{BacSiDocPhanUng}}<br><br></td>
            </tr>
        </table>
    </div>
</div>'
where  Name='PhieuPhanUngThuoc'

Update dbo.CauHinh
Set [Value] = '3.6.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'