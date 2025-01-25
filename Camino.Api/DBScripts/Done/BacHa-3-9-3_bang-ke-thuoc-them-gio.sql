
update Template set Body = N'<head>
    <style>
        table.table-thuoc {
            page-break-inside: auto
        }

        table.table-thuoc tr {
            page-break-inside: avoid;
            page-break-after: auto
        }

        table.table-thuoc th {
            page-break-inside: avoid;
            page-break-after: auto
        }
    </style>
    <link href="https://fonts.googleapis.com/css?family=Libre Barcode 39" rel="stylesheet">
</head>
<table id="showHeader" style="display:none;"></table>
<table style="width: 100%;">
    <tbody>
        <tr>
            <td style="display: flex;justify-content: space-between;"><img src="{{LogoUrl}}"
                    style="width:500px; height: 100px;" alt="blog-cong-dong" /> <span
                    style="font-size: 20px;font-weight: bold;">{{SoChungTu}}</span></td>
        </tr>
    </tbody>
</table>
<table style="padding: 5px;width: 100%;">
    <th> <span style="font-size: 32px;">BẢNG KÊ THU TIỀN THUỐC</span><br> </th>
</table>
<div style="padding-left: 20px;"> </br>
    <table style="width:100%">
        <tbody>
            <tr>
                <td style="padding-left: -5px;width:300px ;" colspan="2">Họ Tên: <b>{{NguoiNopTien}}</b> </td>
                <td>&nbsp;&nbsp;Năm sinh: <b>{{NamSinh}}</b> </td>
                <td style="padding-left: -5px;">Giới tính: {{GioiTinh}} </td>
            </tr>
            <tr>
                <td style="padding-left: -5px;width:600px" colspan="4"> Địa chỉ : {{DiaChi}} </td>
            </tr>
        </tbody>
    </table> <br>
    <table class="table-thuoc" style="width:100%;border: 1px solid #020000; border-collapse: collapse;">
        <thead>
            <tr style="border: 1px solid black;  border-collapse: collapse;">
                <th
                    style="border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: center;padding:5px;">
                    STT </th>
                <th
                    style="border: 1px solid #020000; border-collapse: collapse;width: 30%;text-align: center;padding:5px;">
                    Tên thuốc,vật tư </th>
                <th
                    style="border: 1px solid #020000; border-collapse: collapse; width: 5%;text-align: center;padding:5px;">
                    ĐVT</th>
                <th
                    style="border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: center;padding:5px;">
                    SL </th>
                <th
                    style="border: 1px solid #020000; border-collapse: collapse;width: 10%;text-align: center;padding:5px;">
                    Đơn giá</th>
                <th
                    style="border: 1px solid #020000; border-collapse: collapse;width: 10%;text-align: center;padding:5px;">
                    Thành tiền </th>
            </tr>
        </thead>
        <tbody>{{keToaThuoc}} </tbody>
    </table>
    <table style="width:100%;">
        <tr>
            <th style="width: 5%;text-align: center;padding:5px;"> </th>
            <th style="width: 30%;text-align: center;padding:5px;"> </th>
            <th style=" width: 5%;text-align: center;padding:5px;"> </th>
            <th style="width: 5%;text-align: center;padding:5px;"> </th>
            <th style="width: 10%;text-align: right;padding:5px;"> Tổng cộng</th>
            <th style="width: 10%;text-align: right;padding:5px;"> {{tongTienToaThuoc}} </th>
        </tr>
    </table> <span style="font-size: 17px;">{{soTienBangChu}}</span><br>
    <table style="width:100%;">
        <tr>
            <td style="width:6%;"></td>
            <td style="width:10%;"></td>
            <td style="text-align:  left;width:6%;"><i>{{ngayThangHientai}}</i></td>
        </tr>
        <tr>
            <td style="text-align: center;"><b>Người nộp tiền</b> </td>
            <td style="text-align: center;"><b>Người phát thuốc</b> </td>
            <td style="text-align: center;"><b>Người thu tiền</b> </td>
        </tr>
        <tr>
            <td style="text-align: center;"><i>(Ký, ghi rõ họ tên)</i> </td>
            <td style="text-align: center;"><i>(Ký, ghi rõ họ tên)</i> </td>
            <td style="text-align: center;"><i>(Ký, ghi rõ họ tên)</i> </td>
        </tr>
        <tr>
            <td style="text-align: center;"></td>
            <td style="text-align: center;padding-top: 100px;"><b>{{NguoiThuTien}}</b></td>
            <td style="text-align: center;"></td>
        </tr>
        <tr>
            <td style="padding-left: -5px;padding-top: 20px;" colspan="3"><i>* <b>Bệnh nhân có nhu cầu xuất hóa đơn xin
                        vui lòng liên hệ trong ngày với bộ phận lễ tân bệnh viện</b></i> </td>
        </tr>
        <tr>
            <td style="padding-left: -5px;" colspan="3"><b><u>*Chú ý:</u></b> <i>-Yêu cầu quý khách kiểm tra số lượng,
                    chất lượng, HSD</i> <br> <i style="padding-left:60px;">-Thuốc đã ra khỏi quầy Quý khách vui lòng
                    không hoàn trả lại</i> </td>                   
        </tr>
        <tr><td>{{GoiHienTai}}</td></tr>
    </table>
</div>'
 where [Name]='BangKeThuTienThuoc'

Update dbo.CauHinh
Set [Value] = '3.9.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'