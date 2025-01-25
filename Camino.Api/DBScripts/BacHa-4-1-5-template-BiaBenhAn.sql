update template
set Body=N'{{Header}} <style>
    table,
    td,
    th {
        border-collapse: collapse;
        font-family: Times New Roman
    }

    td,
    th {
        padding: 2px
    }

    .container {
        width: 100%;
        display: table
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
        border-bottom: 1px dotted #000;
        padding-left: 2px
    }

    #customers {
        width: 100%;
        border: 1px solid #000
    }

    #customers td,
    #customers th {
        border: 1px solid #000
    }

    #customers th {
        color: #000
    }

    span.square {
        border: solid 1px;
        width: 25px;
        height: 20px;
        text-align: center;
        margin: 2px;
        display: inline-block
    }

    .hidden {
        display: none;
    }
</style>
<table style="width:100%">
    <tbody>
        <tr>
            <td>
                <table style="width:100%">
                    <tr>
                        <td colspan="2" style="font-weight:700"> <img src="{{LogoUrl}}" style="height: 70px"
                                alt="logo-bv-bac-ha" /> </td>
                        <td width="25%">
                            <div style="height: 70px; border-style: solid; border-color: #e2e2e5;">
                                <div
                                    style="text-decoration: underline; font-size: 12px; color:#b1b1b3;text-align: center;">
                                    TIỀN SỬ DỊ ỨNG THUỐC</div>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2"></td>
                        <td>
                            <div style="height: 70px; border-style: solid; border-color: #e2e2e5; text-align: center;">
                                <div style="font-size: 12px; color:#b1b1b3;"> <label
                                        style="text-decoration: underline; ">BHYT</label> <br> <br> <b class="tagBHYT"
                                        style="color:red; width:40%; height: 20px; border-style: solid; border-color: red; font-size: 20px;padding: 5px 15px 5px 15px;">BHYT</b>
                                </div>
                            </div>
                        </td>
                    </tr>
                </table>
                <table style="width:100%">
                    <tr>
                        <td width="30%">
                            <div class="label" style="text-align: center;"> <b>Số lưu trữ</b> (Storage) </div>
                            <div class="container" style="width: 100% ;margin:0 auto;font-weight:normal">
                                <div class="value">{{SoLuuTru}}</div>
                            </div>
                        </td>
                        <td width="40%"></td>
                        <td width="30%">
                            <div class="label" style="text-align: center;"> <b>Mã vào viện</b> (Hospitalisation) </div>
                            <div class="container" style="width: 100% ;margin:0 auto;font-weight:normal">
                                <div class="value">{{MaVaoVien}}</div>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="3"> <b style="font-size:50px; color: #005dab">BỆNH ÁN</b> <br> <b
                                style="font-size:20px">MEDICAL RECORD</b> <br> <b>Năm</b> (YEAR) <b>{{NamNBRaVien}}</b> </td>
                    </tr>
                </table>
            </td>
        </tr>
    </tbody>
</table> <br>
<table style="width:100%">
    <tbody>
        <tr>
            <td>
                <div class="container" style="width:100%;">
                    <div class="label"> <b>Họ và tên</b> (Fullname): </div>
                    <div class="value">{{HoVaTen}}</div>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <table width="100%">
                    <tr>
                        <td width="80%">
                            <div class="container" style="width:100%;">
                                <div class="label"> <b>Năm sinh</b> (DOB): </div>
                                <div class="value">{{BenhNhanTuoi}}</div>
                            </div>
                        </td>
                        <td width="10%">
                            <div class="container" style="width:100%;">
                                <div class="label"> <b>Nam</b> (Male): <span class="square">{{LaNam}}</span> </div>
                            </div>
                        </td>
                        <td width="10%">
                            <div class="container" style="width:100%;">
                                <div class="label"> <b>Nữ</b> (Female): <span class="square">{{LaNu}}</span> </div>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <div class="container" style="width:100%;">
                    <div class="label"> <b>Địa chỉ</b> (Address): </div>
                    <div class="value">{{DiaChi}}</div>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="container" style="width:100%;">
                    <div class="label"> <b>Nghề nghiệp</b> (Occupations): </div>
                    <div class="value">{{NgheNghiep}}</div>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="container" style="width:80%; float: left">
                    <div class="label"> <b>Chẩn đoán</b> (Diagnosis): </div>
                    <div class="value">{{ChanDoan}}</div>
                </div>
                <div class="container" style="width:20%;">
                    <div class="label"> <b>ICD 10:</b> </div>
                    <div class="value">{{MaChanDoan}}</div>
                </div>
            </td>
        </tr>
    </tbody>
</table> <br>
<table width="100%">
    <tbody>
        <tr style="margin-top:10px">
            <td width="30%" style="vertical-align: top;">
                <div style="text-align: center;"> <b>NHẬP VIỆN</b> <br> (Primary Admission) </div>
                <div class="container" style="width:100%;">
                    <div class="label"> <b>Khoa</b> (Dept): </div>
                    <div class="value">{{KhoaNhapVien}}</div>
                </div>
                <div class="container" style="width:100%;">
                    <div class="label"> <b>Ngày</b> (Date): </div>
                    <div class="value" style="width:30%;">{{NgayNhapVien}}</div>
                    <div class="value" style="width:30%;">/{{ThangNhapVien}}</div>
                    <div class="value">/20{{NamNhapVien}}</div>
                </div>
            </td>
            <td width="2%"></td>
            <td style="vertical-align: top;">
                <div style="text-align: center;"> <b>CHUYỂN VIỆN - KHOA</b> <br> (2nd Admission) </div>
                {{ThongTinKhoaChuyen}}
            </td>
            <td width="2%"></td>
            <td width="30%" style="vertical-align: top;">
                <div style="text-align: center;"> <b>XUẤT VIỆN</b> <br> (Discharge) </div>
                <div class="container" style="width:100%;">
                    <div class="label"> <b>Khoa</b> (Dept): </div>
                    <div class="value">{{KhoaXuatVien}}</div>
                </div>
                <div class="container" style="width:100%;">
                    <div class="label"> <b>Ngày</b> (Date): </div>
                    <div class="value" style="width:30%;">{{NgayXuatVien}}</div>
                    <div class="value" style="width:30%;">/{{ThangXuatVien}}</div>
                    <div class="value">/20{{NamXuatVien}}</div>
                </div>
            </td>
        </tr>
    </tbody>
</table>'
where Name=N'BiaBenhAnDienTu'
UPDATE CauHinh
Set [Value] = '4.1.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'