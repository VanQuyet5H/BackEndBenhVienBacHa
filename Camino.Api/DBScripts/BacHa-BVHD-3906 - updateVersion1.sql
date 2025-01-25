update  Template 
set Body=N'<style>
    table,
    th,
    td {
        border-collapse: collapse;
        font-family: Times New Roman;
    }

    th,
    td {
        padding: 3px;
        font-size: 13px;
    }

    .breakword {
        word-break: break-all;
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
<table width="100%" style="margin-bottom:0;">
    <tr>
        <td style="vertical-align: top;font-size: 15px;"> BỆNH VIỆN ĐKQT BẮC HÀ <br> <b>Khoa:..........</b></td>
        <td style="text-align:center;float:right;vertical-align: top;font-size: 15px;"> <img style="height:45px;"
                src="data:image/png;base64,{{BarCodeImgBase64}}"><br>
            <p style="margin:0;padding:0;">Mã TN:{{MaTN}} <br> Mã NB:{{MaBN}}</p>
        </td>
    </tr>
</table> <br>
<div style="width: 100%;text-align: center;font-size: 14px;"> <b>PHIẾU THEO DÕI THỰC HIỆN XÉT NGHIỆM ĐƯỜNG MÁU <br> MAO
        MẠCH TẠI GIƯỜNG VÀ TIÊM INSULIN THEO GIỜ<b> </div>
<table width="100%">
    <tr>
        <td style="width: 60%;">
            <div class="container">
                <div class="label">Họ tên:&nbsp;</div>
                <div class="value">{{HoTen}}</div>
            </div>
        </td>
        <td style="width: 25%;">
            <div class="container">
                <div class="label">Ngày/tháng/năm sinh:&nbsp;</div>
                <div class="value"> {{NamSinh}}</div>
            </div>
        </td>
        <td style="width: 15%;">
            <div class="container">
                <div class="label">Giới tính:&nbsp;</div>
                <div class="value"> {{GT}}</div>
            </div>
        </td>
    </tr>
    <tr>
        <td style="width: 60%;">
            <div class="container">
                <div class="label">Giường:&nbsp;</div>
                <div class="value"> {{Giuong}}</div>
            </div>
        </td>
        <td style="width: 40%;" colspan="2">
            <div class="container">
                <div class="label">Phòng:&nbsp;</div>
                <div class="value"> {{Phong}}</div>
            </div>
        </td>
    </tr>
    <tr>
        <td colspan="3" style="width: 100%;">
            <div class="container">
                <div class="label">Chẩn đoán:&nbsp;</div>
                <div class="value"> {{ChanDoan}} </div>
            </div>
        </td>
    </tr>
</table> <br>
<table width="100%" style="border-collapse: collapse;">
    <thead>
        <tr>
            <th style="vertical-align: top;width: 20%;border: 1px solid black;text-align: center;">Giờ/ngày/tháng/năm
            </th>
            <th style="vertical-align: top;width: 20%;border: 1px solid black;text-align: center;">Kết quả đường <br>máu
                mao mạch<br>(đơn vị tính<br>mmol/l)</th>
            <th style="vertical-align: top;width: 20%;border: 1px solid black;text-align: center;">Thực hiện tiêm
                <br>insulin (liều lượng tính theo<br>đơn vị/ml) </th>
            <th style="vertical-align: top;width: 20%;border: 1px solid black;text-align: center;">ĐD thực hiện<br>(Ký
                và ghi rõ họ<br> tên)</th>
            <th style="vertical-align: top;width: 20%;border: 1px solid black;text-align: center;"> Bác sỹ chỉ
                định<br>(Ghi rõ họ tên)</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;">.....
                giờ.../..../..../202&nbsp;&nbsp;</td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"> </td>
        </tr>
        <tr>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;">.....
                giờ.../..../..../202&nbsp;&nbsp;</td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"> </td>
        </tr>
        <tr>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;">.....
                giờ.../..../..../202&nbsp;&nbsp;</td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"> </td>
        </tr>
        <tr>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;">.....
                giờ.../..../..../202&nbsp;&nbsp;</td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"> </td>
        </tr>
        <tr>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;">.....
                giờ.../..../..../202&nbsp;&nbsp;</td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"> </td>
        </tr>
        <tr>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;">.....
                giờ.../..../..../202&nbsp;&nbsp;</td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"> </td>
        </tr>
        <tr>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;">.....
                giờ.../..../..../202&nbsp;&nbsp;</td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"> </td>
        </tr>
        <tr>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;">.....
                giờ.../..../..../202&nbsp;&nbsp;</td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"> </td>
        </tr>
        <tr>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;">.....
                giờ.../..../..../202&nbsp;&nbsp;</td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"> </td>
        </tr>
        <tr>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;">.....
                giờ.../..../..../202&nbsp;&nbsp;</td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"> </td>
        </tr>
        <tr>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;">.....
                giờ.../..../..../202&nbsp;&nbsp;</td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"> </td>
        </tr>
        <tr>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;">.....
                giờ.../..../..../202&nbsp;&nbsp;</td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"> </td>
        </tr>
        <tr>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;">.....
                giờ.../..../..../202&nbsp;&nbsp;</td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"> </td>
        </tr>
        <tr>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;">.....
                giờ.../..../..../202&nbsp;&nbsp;</td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"> </td>
        </tr>
        <tr>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;">.....
                giờ.../..../..../202&nbsp;&nbsp;</td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"> </td>
        </tr>
        <tr>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;">.....
                giờ.../..../..../202&nbsp;&nbsp;</td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"></td>
            <td style="vertical-align: top;width: 20%;border: 1px solid black;border-top: none;height: 39px;"> </td>
        </tr>
    </tbody>
</table>
<table width="100%">
    <tr>
        <td style="text-align:center;" width="50%"></td>
        <td style="text-align:center;" width="50%"><i>Ngày .......... tháng .......... năm ..........</i></td>
    </tr>
    <tr>
        <td style="text-align:center;" style="font-size: 15px;text-transform: uppercase;"></td>
        <td style="font-size: 15px; text-align:center;text-transform: uppercase;"><b>ĐIỀU DƯỠNG TRƯỞNG</b></td>
    </tr>
    <tr>
        <td style="font-size: 15px;font-style: italic;text-align:center"></td>
        <td style="font-size: 15px; text-align:center;font-style: italic;">(Ký và ghi rõ họ tên)</td>
    </tr>
    <tr>
        <td style="font-size: 15px; text-align:center;"></td>
        <td style="font-size: 15px; text-align:center;"><br><br><br><br><b>{{DieuDuongTruong}}</b></td>
    </tr>
</table>'
where Name='PhieuTheoDoiThucHienXNDuongMauMaoMachTaiGiuongVaTiemInSulinTheoGioDeFault'


update Template
set Body=N'<style>
    table,
    th,
    td {
        border-collapse: collapse;
        font-family: Times New Roman;
    }

    th,
    td {
        padding: 3px;
        font-size: 13px;
    }

    .breakword {
        word-break: break-all;
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
<table width="100%" style="margin-bottom:0;">
    <tr>
        <td style="vertical-align: top;font-size: 15px;"> BỆNH VIỆN ĐKQT BẮC HÀ <br> {{KhoaDangIn}}</td>
        <td style="text-align:center;float:right;vertical-align: top;font-size: 15px;"> <img style="height:45px;"
                src="data:image/png;base64,{{BarCodeImgBase64}}"><br>
            <p style="margin:0;padding:0;">Mã TN:{{MaTN}}<br>Mã NB:{{MaBN}}</p>
        </td>
    </tr>
</table>
<div style="width: 100%;text-align: center;font-size: 14px;"> <b>PHIẾU THEO DÕI THỰC HIỆN XÉT NGHIỆM ĐƯỜNG MÁU <br> MAO
        MẠCH TẠI GIƯỜNG VÀ TIÊM INSULIN THEO GIỜ<b> </div> <br>
<table width="100%">
    <tr>
        <td style="width: 60%;"> {{HoTenNgBenh}} </td>
        <td style="width: 25%;"> {{NamSinh}} </td>
        <td style="width: 15%;"> {{GT}} </td>
    </tr>
    <tr>
        <td style="width: 60%;"> {{Giuong}} </td>
        <td style="width: 40%;" colspan="2"> {{Phong}} </td>
    </tr>
    <tr>
        <td colspan="3" style="width: 100%;"> {{ChanDoan}} </td>
    </tr>
</table> <br> {{BodyTable}} <table width="100%">
    <tr>
        <td style="text-align:center;" width="50%"></td>
        <td style="text-align:center;" width="50%"><i>{{NgayThangNam}}</i></td>
    </tr>
    <tr>
        <td style="text-align:center;" style="font-size: 15px;text-transform: uppercase;"></td>
        <td style="font-size: 15px; text-align:center;text-transform: uppercase;"><b>ĐIỀU DƯỠNG TRƯỞNG</b></td>
    </tr>
    <tr>
        <td style="font-size: 15px;font-style: italic;text-align:center"></td>
        <td style="font-size: 15px; text-align:center;font-style: italic;">(Ký và ghi rõ họ tên)</td>
    </tr>
    <tr>
        <td style="font-size: 15px; text-align:center;"></td>
        <td style="font-size: 15px; text-align:center;"><br><br><br><br><b>{{DieuDuongTruong}}</b></td>
    </tr>
</table>'
where Name='PhieuTheoDoiThucHienXNDuongMauMaoMachTaiGiuongVaTiemInSulinTheoGio'

	
UPDATE CauHinh
Set [Value] = '3.4.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'