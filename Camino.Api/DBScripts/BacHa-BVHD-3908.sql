INSERT INTO template (Name, Title,
Body,TemplateType,Language,Description,Version,IsDisabled,CreatedById,LastUserId,LastTime,CreatedOn)
VALUES (N'BangKiemGacDungCuSauPhauThuat', N'Bảng kiểm gạc, dụng cụ phẫu thuật', N'<style>
    * {
        box-sizing: border-box;
    }

    .container {
        width: 100%;
        display: table;
    }

    .container .label {
        width: max-content; white-space: nowrap;
    }

    .container .value {
        display: table-cell;
        width: 100%;
        height: 100%;
        vertical-align: top;
        position: relative;
        box-sizing: border-box;
       
    }

    table,
    th,
    td {
        border-collapse: collapse;
        font-family: Times New Roman;
        font-size: 14px;
    }

    th,
    td {
        padding: 2px;
    }
    .borderRow{
    border: 1px solid black;
    text-align: center;
}
.borderRowItem{
    border: 1px solid black;
}
.xuongDong{
    word-break: break-work;
}
</style>
<table style="width: 100%;">
    <tbody>
        <tr>
            <td style="display: flex;justify-content: space-between;"><img src="{{LogoUrl}}"
                    style="width:300px; height: 100px;" alt="benh-vien-da-khoa-quoc-te-bac-ha" />
            <td style="padding-left: 50px;">
                <div style="text-align:center;float:right;vertical-align: top;"> <img style="height: 40px;"
                        src="data:image/png;base64,{{BarCodeImgBase64}}"><br>
                    <p style="margin:0;padding:0">Mã TN: {{MaTN}}</p>
                </div>
            </td>
        </tr>
    </tbody>
</table>
<br>
<table style="padding: 5px;width: 100%;">
    <th> <span style="font-size: 16px;">BẢNG KIỂM GẠC, DỤNG CỤ PHẪU THUẬT</span><br> </th>
</table>
<br>
<table style="width:100%">
    <tr>
        <td style="width: 70%;">
            <div class="container">
                <div class="label">Họ và tên: </div>
                <div class="value"><b>{{HoVaTen}}</b></div>
            </div>
        </td>
        <td style="width: 30%;">
            <div class="container">
                <div class="label">Năm sinh: </div>
                <div class="value">{{NamSinh}}</div>
            </div>
        </td>
    </tr>
    <tr>
        <td style="width: 100%;" colspan="2">
            <div class="container">
                <div class="value">Chẩn đoán phẩu thuật:{{ChanDoanPT}}</div>
            </div>
        </td>
    </tr>
    <tr>
        <td style="width: 70%;">
            <div class="container">
                <div class="label"></div>
                <div class="value"> Phẫu thuật: {{PhauThuat}}</div>
            </div>
        </td>
        <td style="width: 30%;">
            <div class="container">
                <div class="label">Giờ: </div>
                <div class="value">{{Gio}}</div> 
            </div>
        </td>
    </tr>
</table>
<br>
<table style="width:100%;border-collapse: collapse;">
    <thead>
        <tr>
            <th colspan="3" style="width: 21%;" class="borderRow">Gạc trước phẫu thuật</th>
            <th colspan="3" style="width: 21%;" class="borderRow">Gạc sau phẫu thuật</th>
            <th style="width: 29%;" class="borderRow" colspan="2">Dụng cụ trước phẫu thuật</th>
            <th style="width: 29%;" class="borderRow" colspan="2">Dụng cụ sau phẫu thuật</th>
        </tr>
        <tr>
            <th style="width: 7%;word-break: break-all;" class="borderRow">To</th>
            <th style="width: 7%;word-break: break-all;" class="borderRow">Nhỏ</th>
            <th style="width: 7%;word-break: break-all;" class="borderRow">MeChe</th>
            <th style="width: 7%;word-break: break-all;" class="borderRow">To</th>
            <th style="width: 7%;word-break: break-all;" class="borderRow">Nhỏ</th>
            <th style="width: 7%;word-break: break-all;" class="borderRow">MeChe</th>
            <th style="width: 19%;word-break: break-all;" class="borderRow">Loại</th>
            <th style="width: 10%;word-break: break-all;" class="borderRow">Số lượng</th>
            <th style="width: 19%;word-break: break-all;" class="borderRow">Loại</th>
            <th style="width: 10%;word-break: break-all;" class="borderRow">Số lượng</th>
        </tr>
    </thead>
    <tbody>
        {{BangKiemGacDungCuSauPhauThuat}}
    </tbody>
</table>
<br>
<table style="width:100%;">
    <tr>
        <th>Người đếm thứ nhất</th>
        <th>Người đếm thứ hai</th>
        <th>Phẫu thuật viên</th>
    </tr>
    <tr>
        <td style="text-align: center;font-style:italic;">(Ký và ghi rõ họ tên)</td>
        <td style="text-align: center;font-style:italic;">(Ký và ghi rõ họ tên)</td>
        <td style="text-align: center;font-style:italic;">(Ký và ghi rõ họ tên)</td>
    </tr>
    <tr>
        <th><br><br><br><br></th>
        <th><br><br><br><br></th>
        <th><br><br><br><br></th>
    </tr>
    <tr>
        <th>{{NguoiThucNhat}}</th>
        <th>{{NguoiThuHai}}</th>
        <th>{{PhauThuatVien}}</th>
    </tr>
</table>
',1,1,N'Bảng kiểm gạc, dụng cụ phẫu thuật ',1,0,1,1,GETDATE(),GETDATE())
UPDATE CauHinh
Set [Value] = '3.4.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'