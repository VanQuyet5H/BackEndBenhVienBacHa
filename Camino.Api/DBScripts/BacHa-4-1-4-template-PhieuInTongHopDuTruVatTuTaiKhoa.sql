update template
set Body=N'{{Header}} <style>
    table,
    th,
    td {
        border-collapse: collapse;
        font-family: Times New Roman;
    }

    th,
    td {
        padding: 0px;
    }
</style>
<table style="width: 100%;">
    <tbody>
        <tr>
            <td style="display: flex;justify-content: space-between;"><b>BỆNH VIỆN ĐKQT BẮC HÀ</b> <span
                    style="font-size: 20px;font-weight: bold; text-align:center"> BMBH-KVT02</span> </td>
        </tr>
        <tr>
            <td style="text-align: left;">TÊN KHOA/PHÒNG: {{KhoaPhong}}</td>
        </tr>
    </tbody>
</table>
<table style="padding: 5px;width: 100%;">
    <tr>
        <th> <span style="font-size: 20px;">PHIẾU TỔNG HỢP DỰ TRÙ VẬT TƯ</span><br> </th>
    </tr>
    <tr>
        <th> <span style="font-size: 15px;"><i>(Từ ngày {{TuNgay}} đến ngày {{DenNgay}})</i></span><br> </th>
    </tr>
</table>
<div style="padding-left: 20px;"> </br>
    <table style="width:100%;border: 1px solid #020000; border-collapse: collapse;">
        <thead>
            <tr>
                <th rowspan="2"
                    style="border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: center;padding: 5px;">
                    STT</th>
                <th rowspan="2"
                    style="border: 1px solid #020000; border-collapse: collapse;width: 10%;text-align: center;padding: 5px;">
                    MÃ HÀNG</th>
                <th rowspan="2"
                    style="border: 1px solid #020000; border-collapse: collapse;width: 20%;text-align: center;padding: 5px;">
                    TÊN VẬT TƯ</th>
                <th rowspan="2"
                    style="border: 1px solid #020000; border-collapse: collapse; width: 5%;text-align: center;padding: 5px;">
                    ĐƠN VỊ</th>
                <th colspan="1"
                    style="border: 1px solid #020000; border-collapse: collapse;width: 5%;text-align: center;padding: 5px;">
                    SỐ LƯỢNG</th>
                <th rowspan="2"
                    style="border: 1px solid #020000; border-collapse: collapse;width: 20%;text-align: center;padding: 5px; border-bottom:opx;">
                    GHI CHÚ</th>
            </tr>
        </thead>
        <tbody> {{DuTruMuaVatTuChiTiet}} </tbody>
    </table>
    <table style="width: 100%;">
        <tbody>
            <tr>
                <td style="justify-content: space-between; text-align:right"><span><i>Ngày {{Ngay}} tháng {{Thang}} năm
                            {{Nam}}</i></span></td>
            </tr>
        </tbody>
    </table>
    <table style="width:100%;">
        <tr>
            <th><b>NGƯỜI LẬP</b> </th>
            <th><b>TRƯỞNG PHÒNG PHỤ TRÁCH</b> </th>
            <th><b>GIÁM ĐỐC CM</b> </th>
        </tr>
        <tr>
            <td style="text-align: center;"><i>(Ký, ghi rõ họ tên)</i> </td>
            <td style="text-align: center;"><i>(Ký, ghi rõ họ tên)</i> </td>
            <td style="text-align: center;"><i>(Ký, ghi rõ họ tên)</i> </td>
        </tr>
    </table><br><br><br><br><br><br>
    <table>
        <tr>
            <td colspan="2" style="font-size: 15px; text-decoration: underline"><b>Lưu ý:</b></td>
        </tr>
        <tr>
            <td colspan="2" style="font-size: 15px"><i>- Tên khoa/phòng: </i></td>
        </tr>
        <tr>
            <td colspan="2" style="font-size: 15px">&nbsp;&nbsp;&nbsp;<i>+ Là phòng chuyên môn </i></td>
        </tr>
        <tr>
            <td colspan="2" style="font-size: 15px">&nbsp;&nbsp;&nbsp;<i>+ Khoa VTYT</i></td>
        </tr>
        <tr>
            <td colspan="2" style="font-size: 15px"><i>- Trưởng phòng phụ trách:</i></td>
        </tr>
        <tr>
            <td colspan="2" style="font-size: 15px"><i>&nbsp;&nbsp;&nbsp;Là trưởng khoa</i></td>
        </tr>
        <tr>
            <td colspan="2" style="font-size: 15px"><i>- Lưu 3 năm</i></td>
        </tr> 
    </table>
</div>'
where Name=N'PhieuInTongHopDuTruVatTuTaiKhoa'
UPDATE CauHinh
Set [Value] = '4.1.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'