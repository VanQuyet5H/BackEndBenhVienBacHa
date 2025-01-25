



INSERT INTO template (Name, Title,
Body,TemplateType,Language,Description,Version,IsDisabled,CreatedById,LastUserId,LastTime,CreatedOn)
VALUES (N'TemPlateFooterPhieuDangKyKham', N'Footer Phiếu đăng ký khám', N'<!DOCTYPE html>
<html>

<head>
    <meta charset="UTF-8">
    <script>
        function subst() {
            var vars = {};
            var x = window.location.search.substring(1).split("&");
            for (var i in x) { var z = x[i].split("=", 2); vars[z[0]] = unescape(z[1]); }
            var x = ["frompage", "topage", "page", "webpage", "section", "subsection", "subsubsection"];
            for (var i in x) {
                var y = document.getElementsByClassName(x[i]);
                for (var j = 0; j < y.length; ++j) y[j].textContent = vars[x[i]];
            }
        }
    </script>
</head>

<body style="border:0; margin: 0;" onload="subst()">
    <table style="width: 100%">
        <tr>
            <td>{{NgayHienTai}}</td>
            <td style="text-align: center;">{{TexTFooTer}}</td>
            <td style="text-align:right">
                Trang <span class="page"></span>/<span class="topage"></span>
            </td>
        </tr>
    </table>
</body>

</html>',1,1,N'Footer Phiếu đăng ký khám',1,0,1,1,GETDATE(),GETDATE())





INSERT INTO template (Name, Title,
Body,TemplateType,Language,Description,Version,IsDisabled,CreatedById,LastUserId,LastTime,CreatedOn)
VALUES (N'TemplateFooterInKetQuaKham', N'Footer Phiếu in kết quả khám', N'<!DOCTYPE html>
<html>

<head>
    <meta charset="UTF-8">
    <script>
        function subst() {
            var vars = {};
            var x = window.location.search.substring(1).split("&");
            for (var i in x) { var z = x[i].split("=", 2); vars[z[0]] = unescape(z[1]); }
            var x = ["frompage", "topage", "page", "webpage", "section", "subsection", "subsubsection"];
            for (var i in x) {
                var y = document.getElementsByClassName(x[i]);
                for (var j = 0; j < y.length; ++j) y[j].textContent = vars[x[i]];
            }
        }
    </script>
</head>

<body style="border:0; margin: 0;" onload="subst()">
    <table style="width: 100%">
        <tr>
            <td><b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;*Ghi chú: Phân loại khám sức khỏe: Loại 1: Rất khỏe; Loại 2: Khỏe; Loại 3: Trung bình; Loại 4: Yếu; Loại 5: Rất yếu.</b></td>
            <td style="text-align:right">
                Trang <span class="page"></span>/<span class="topage"></span>
            </td>
        </tr>
    </table>
</body>

</html>',1,1,N'Footer Phiếu in kết quả khám',1,0,1,1,GETDATE(),GETDATE())

update template 
set Body=N'<style>
    * {
        box-sizing: border-box
    }

    tbody th,
    tbody td {
        padding: 5px;
    }
</style>
<table width="100%" style="border-collapse:collapse ;">
    <thead>
        <tr>
            <th colspan="2" style="width: 50%; height: 80px;text-align: left;"> <img src="{{LogoUrl}}"
                    style="height: 80px;" alt="BỆNH VIỆN ĐKQT BẮC HÀ" /></th>
            <th colspan="2" style="width: 30%;"></th>
            <th style="width: 20%; text-align: center; vertical-align: top;">
                <div style="width: 100%; display: flex; flex-direction: column; justify-content: center;">
                    <div style="text-align:center;float:right;"> <img style="height: 40px;"
                            src="data:image/png;base64,{{BarCodeImgBase64}}"><br>
                        <p style="margin:0;padding:0">{{MaTN}}</p>
                    </div>
                </div>
            </th>
        </tr>
        <tr>
            <th colspan="5"
                style="text-align: center; color: #8a1515; font-size: large; font-weight: bold; padding-top: 0.25rem; padding-bottom: 0.25rem;">
                PHIẾU ĐĂNG KÝ KHÁM SỨC KHOẺ</th>
        </tr>
        <tr>
            <th colspan="5">
                <table id="table-thong-tin">
                    <tr>
                        <th colspan="2">Đơn vị khám: <b>{{DonViKham}}</b></th>
                    </tr>
                    <tr>
                        <th style="width: 70%;">Họ tên: <b>{{HoTen}}</b></th>
                        <th>Giới tính: {{GioiTinh}}</th>
                    </tr>
                    <tr>
                        <th>Năm sinh: <b>{{NamSinh}}</b></th>
                        <th>Mã nhân viên: {{MaNhanVien}}</th>
                    </tr>
                    <tr>
                        <th>Chức vụ: {{ChucVu}}</th>
                        <th>Ghi chú: {{GhiChu}}</th>
                    </tr>
                    <tr>
                        <th colspan="2">Vị trí công tác: {{ViTriCongTac}}</th>
                    </tr>
                </table>
            </th>
        </tr>
        <tr>
            <th style="width: 8%;" class="border-table">STT</th>
            <th style="width: 23%;" class="border-table">Nội dung</th>
            <th style="width: 20%;" class="border-table">Ngày thực hiện</th>
            <th style="width: 29%;" class="border-table">Khoa/Phòng thực hiện</th>
            <th style="width: 20%;" class="border-table">Ghi chú</th>
        </tr>
    </thead>
    <tbody> {{columnTable}}</tbody>
</table>
<div>{{GhiChuDV}}</div>
<table width="100%">
    <tbody>
        <tr>
            <td>
                <table style="width: 100%; text-align: center;">
                    <tr>
                        <td style="width: 50%;"> Nhân viên tiếp đón <br> <span style="font-style: italic;">(Ký, ghi rõ
                                họ tên)</span></td>
                        <td style="width: 50%;"> Ngày {{Ngay}} tháng {{Thang}} năm {{Nam}} <br> Người đi khám <br> <span
                                style="font-style: italic;">(Ký, ghi rõ họ tên)</span></td>
                    </tr>
                    <tr>
                        <td style="width: 50%; padding-top: 5rem;"> {{NhanVienTiepDon}}</td>
                        <td style="width: 50%; padding-top: 5rem;"> {{NguoiDiKham}}</td>
                    </tr>
                </table>
            </td>
        </tr>
    </tbody>
</table>
<style>
    .border-table {
        border: 1px solid black;
        border-collapse: collapse
    }

    #table-thong-tin {
        width: 100%;
        text-align: left
    }

    #table-thong-tin tr th {
        font-weight: normal
    }

    #table-dich-vu {
        width: 100%;
        border: 1px solid black;
        border-collapse: collapse
    }

    #table-dich-vu tr,
    #table-dich-vu td,
    #table-dich-vu th {
        border: 1px solid black
    }

    #table-dich-vu .table-dich-vu-stt {
        text-align: center
    }

    #table-dich-vu .table-dich-vu-title {
        padding-left: 1rem;
        font-weight: bold;
        text-transform: uppercase
    }
</style>'
where Name=N'PhieuDangKyKhamSucKhoeKhamDoan'

Update dbo.CauHinh
Set [Value] = '3.4.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'