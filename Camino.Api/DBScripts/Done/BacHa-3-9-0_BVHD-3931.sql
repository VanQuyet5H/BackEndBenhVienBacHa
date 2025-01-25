

INSERT INTO template (Name, Title,
Body,TemplateType,Language,Description,Version,IsDisabled,CreatedById,LastUserId,LastTime,CreatedOn)
VALUES (N'PhieuInDoanThuNhaThuoc', N'Phiếu in doanh thu nhà thuốc', N'<style>
    table,
    th,
    td {
        border-collapse: collapse;
        font-family: Times New Roman;
        font-size: 15px;
    }

    th,
    td {
        padding: 3px;
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
</style> <br>
<div>
    <div style="width: 100%;">
        <table width="100%">
            <tbody>
                <tr>
                    <td style="font-size: 30px; text-align:center"> <b>DOANH THU NHÀ THUỐC</b><br> <span
                            style="font-size: 20px; text-align:center"><i>{{ngaythu}}</i><span> </td>
                    <td style="vertical-align:bottom;"> </td>
                </tr>
            </tbody>
        </table>
    </div>
</div> <br>
<table id="customers">
        <tr>
            <th rowspan="2"> STT </th>
            <th rowspan="2"> Số chứng từ </th>
            <th rowspan="2"> Ngày </th>
            <th rowspan="2"> Mã Y tế </th>
            <th rowspan="2">Tên người bệnh </th>
            <th rowspan="2">NS</th>
            <th rowspan="2">GT</th>
            <th rowspan="2"> Thành tiền </th>
            <th rowspan="2"> Tiền mặt </th>
            <th rowspan="2"> Chuyển khoản </th>
            <th rowspan="2"> Pos</th>
            <th rowspan="2">Công nợ</th>
            <th rowspan="2">Người bán</th>
            <th rowspan="2">Ghi chú</th>
            <th rowspan="2">Chi tiết công nợ</th>
            <th rowspan="2">Số hóa đơn</th>
            <th rowspan="2">Lý do hủy bán thuốc</th>
        </tr>
    <tbody> {{DoanhThuThuoc}} </tbody>
</table> <br> <br>
<table style="width:100%">
    <tr>
        <td width="43%" style="font-size: 20px;"><b>Tổng tiền: {{tongTien}}</b> <br /> <b>Tiền mặt: {{tienMat}}</b> <br> <b>Chuyển khoản:
                {{chuyenKhoan}}</b> <br /> <b>POS: {{pos}}</b> <br><b>Công nợ: {{congNo}}</b> <br></td>
        <td width="17%"></td>
        <td style="text-align:center;font-size: 20px"><b>{{ngayThangHientai}}</b> <br><b>Người lập</b> <br><i>(Ký, ghi rõ họ tên)</i></td>
    </tr>
    <tr>
        <td width="43%"> </td>
        <td width="17%"> </td>
        <td style="text-align:center"> <br><br><br> <b style="font-size: 20px;">{{NguoiTao}}</b> </td>
    </tr>
</table>',1,1,N'Phiếu in doanh thu nhà thuốc',1,0,1,1,GETDATE(),GETDATE())

Update dbo.CauHinh
Set [Value] = '3.9.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'