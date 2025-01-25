update Template 
set Body=N'<style>
    table,
    th,
    td {
        border-collapse: collapse;
        font-size: 14px;
        font-family: Times New Roman;
    }

    td {
        text-align: center;
        font-weight: bold;
        padding: 5px;
    }

    th,
    .column {
        float: left;
        width: 48%;
    }

    .columnCenTer {
        float: left;
        width: 4%;
    }

    .container {
        width: 80%;
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
    }

    .pagebreak {
        page-break-before: always;
    }

    .page1 {
        position: absolute;
        left: 902px;
        top: 526px;
    }

    .page1re {
        position: absolute;
        left: 760px;
        top: 524px;
    }

    .page4 {
        position: absolute;
        left: 442px;
        top: 526px;
    }

    .page4re {
        position: absolute;
        left: 370px;
        top: 526px;
    }
</style>
<div class="column" style="padding-top: 5px;">
    <div>
        <div style="margin-left: 0.5cm;margin-right: 0.3cm;">
            <table width="100%">
                <tr>
                    <td colspan="4" style="border: 1px solid black;height: 25px;text-align: center;font-weight: bold;">
                        NHỮNG LẦN VÀO VIỆN SAU </td>
                </tr>
                <tr>
                    <td style="border: 1px solid black;height: 55px;vertical-align: top;">Vào ngày</td>
                    <td style="border: 1px solid black;height: 55px;vertical-align: top;">Ra ngày</td>
                    <td style="border: 1px solid black;height: 55px;vertical-align: top;">Bệnh viện</td>
                    <td style="border: 1px solid black;height: 55px;vertical-align: top;">Ghi chú</td>
                </tr>
                <tr>
                    <td style="border: 1px solid black;height: 85px;width: 26%;"></td>
                    <td style="border: 1px solid black;height: 85px;width: 22%;"></td>
                    <td style="border: 1px solid black;height: 85px;width: 30%"></td>
                    <td style="border: 1px solid black;height: 85px;width: 22%;"></td>
                </tr>
                <tr>
                    <td style="border: 1px solid black;height: 90px;width: 26%;"></td>
                    <td style="border: 1px solid black;height: 90px;width: 22%;"></td>
                    <td style="border: 1px solid black;height: 90px;width: 30%;"></td>
                    <td style="border: 1px solid black;height: 90px;width: 22%;"></td>
                </tr>
                <tr>
                    <td style="border: 1px solid black;height: 90px;width: 26%;"></td>
                    <td style="border: 1px solid black;height: 90px;width: 22%;"></td>
                    <td style="border: 1px solid black;height: 90px;width: 30%;"></td>
                    <td style="border: 1px solid black;height: 90px;width: 22%;"></td>
                </tr>
                <tr>
                    <td style="border: 1px solid black;height: 90px;width: 26%;"></td>
                    <td style="border: 1px solid black;height: 90px;width: 22%;"></td>
                    <td style="border: 1px solid black;height: 90px;width: 30%;"></td>
                    <td style="border: 1px solid black;height: 90px;width: 22%;"></td>
                </tr>
                <tr>
                    <td style="border: 1px solid black;height: 90px;width: 26%;"></td>
                    <td style="border: 1px solid black;height: 90px;width: 22%;"></td>
                    <td style="border: 1px solid black;height: 90px;width: 30%;"></td>
                    <td style="border: 1px solid black;height: 90px;width: 22%;;text-align: right;">
                        <div class="page4"><b>4</b></div>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</div>
<div class="columnCenTer" style="padding-top: 5px;"></div>
<div class="column" style="padding-top: 5px;">
    <div>
        <div style="margin-left: 0.5cm;margin-right: 0.3cm;">
            <div style="width:100%;text-align: center;border: 1px solid black;height: 50px;border-bottom: none;"> <span
                    style="padding-top: 5px;"><b>CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM</b></span> <br> <b><span
                        style="text-decoration: underline;"> Độc lập - Tự do -
                        Hạnh phúc</b></span> </div>
            <div
                style="width:100%;text-align: right;border: 1px solid black;height: 42px;border-bottom: none;border-top: none;vertical-align: middle;">
                Số lưu trữ:{{SoLuuTru}}</div>
            <div
                style="width:100%;text-align: center;border: 1px solid black;height: 436px;border-top: none;vertical-align: top;">
                <div><b>GIẤY CHỨNG NHẬN PHẪU THUẬT</b></div>
                <div style="padding-top: 5px;"><b>Bệnh viện ĐKQT Bắc Hà</b></div>
                <div style="padding-top: 15px;word-break: break-word;font-weight: normal;text-align: left;">
                    &nbsp;&nbsp;Chứng nhận ông/bà: <span
                        style="text-transform: uppercase;font-weight: bold;">{{ChungNhan}}</span></div>
                <div style="padding-top: 15px;word-break: break-word;font-weight: normal;text-align: left;">
                    &nbsp;&nbsp;- Ngày/tháng/năm sinh: {{NgayThangNamSinh}}</div>
                <div style="padding-top: 10px;word-break: break-word;font-weight: normal;text-align: left;">
                    &nbsp;&nbsp;- Địa chỉ: {{DiaChi}}</div>
                <div style="padding-top: 10px;word-break: break-word;font-weight: normal;text-align: left;">
                    &nbsp;&nbsp;- Vào viện ngày: {{VaoVienNgay}}</div>
                <div style="padding-top: 10px;word-break: break-word;font-weight: normal;text-align: left;">
                    &nbsp;&nbsp;- Ra viện ngày: {{RaVienNgay}}</div>
                <div
                    style="padding-top: 10px;word-break: break-word;font-weight: normal;text-align: left;height: 150px;">
                    &nbsp;&nbsp;- Chẩn đoán bệnh: {{ChanDoanBenhNhan}}</div>
                <div style="padding-top: 5px;word-break: break-word;font-weight: normal;text-align: left;">&nbsp;&nbsp;-
                    Nhóm máu: {{NhomMau}}</div>
                <div style="padding-top: 5px;word-break: break-word;font-weight: normal;text-align: left;">&nbsp;&nbsp;-
                    Yếu tố Rh: {{YeuToRh}}</div>
                <div style="padding-top: 5px;word-break: break-word;font-weight: normal;text-align: right;height: 10px">
                    <div class="page1"><b>1</b></div>
                </div>
            </div>
        </div>
    </div>
</div>'
where Name='PhieuChungNhanPhauThuat1'
update Template 
set Body=N'<style>
    table,
    th,
    td {
        border-collapse: collapse;
        font-size: 14px;
        font-family: Times New Roman;
    }

    td {
        font-weight: bold;
        padding: 5px;
    }

    th,
    .column {
        float: left;
        width: 49.5%;
    }

    .columnCenTer {
        float: left;
        width: 0.5%;
    }

    .container {
        width: 80%;
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
    }

    .pagebreak {
        page-break-before: always;
    }
	.page2{
	 position: absolute;
                    left: 442px;
                    top: 1075px;
	}
	
	.page2re{
	 position: absolute;
                    left: 370px;
                    top: 1087px;
	}
	.page3{
	position: absolute;
                    left: 900px;
                    top: 1072px;
	}
	.page3re{
	position: absolute;
                    left: 760px;
                    top: 1088px;
	}
</style>
<div class="column" style="padding-top: 15px;">
    <div>
        <div style="margin-left: 0.5cm;margin-right: 0.3cm;">
            <div style="width:100%;text-align: center;border: 1px solid black;height: 33px;">
                <div style="padding-top: 8px;"><b>CÁCH THỨC PHẪU THUẬT</b></div>
            </div>
            <div style="width:100%;text-align: center;border: 1px solid black;height: 495px;border-top: none;">
                <div style="padding-top: 15px;font-weight: normal;text-align: left;">&nbsp;Phẫu thuật ngày: {{PhauThuatNgay}}
                </div>
                <div style="padding-top: 5px;font-weight: normal;text-align: left;word-break: break-word;height: 40px;">
                    - Phương thức vô cảm: {{PhuongThucVoCam}}</div>
                <div style="padding-top: 5px;word-break: break-word;font-weight: normal;text-align: left;">- Phẫu thuật
                    viên: BS. {{PhauThuatVien}}</div>
                <div
                    style="padding-top: 5px;word-break: break-word;font-weight: normal;text-align: left;height: 108px;">
                    - Phương pháp phẫu thuật: {{PhuongThucPhauThuat}}</div>
                <div style="padding-top: 5px;word-break: break-word;font-weight: normal;text-align: left;height: 40px;">
                    - Tình trạng lúc ra viện: {{TinhTrangLucRaVien}}</div>
                <div style="padding-top: 5px;word-break: break-word;font-weight: normal;text-align: left;height: 50px;">
                    - Kết quả GPB: {{KetQuaGPB}}</div>
                <div style="padding-top: 5px;word-break: break-word;font-weight: i;text-align: right;"><i> Ngày {{Ngay}}
                        tháng {{Thang}} năm {{Nam}}</i></div>
                <table width="100%">
                    <tr>
                        <td style="word-break: break-word;text-align: center;">TRƯỞNG KHOA </td>
                        <td style="word-break: break-word;;text-align: center;">GIÁM ĐỐC CM </td>
                    </tr>
                    <tr>
                        <td style="word-break: break-word;text-align: center;font-weight: normal;vertical-align: top;"><i>(Ký và ghi rõ họ tên) </i></td>
                        <td style="word-break: break-word;;text-align: center;font-weight: normal;vertical-align: top"><i>(Ký và ghi rõ họ tên) </i></td>
                    </tr>
                </table>
                <div style="height: 80px;"></div>
                <table width="100%">
                    <tr>
                        <td style="word-break: break-word;text-align: center;font-weight:normal;">
                            <b>{{TruongKhoa}}</b> <br> </td>
                        <td style="word-break: break-word;;text-align: center;font-weight:normal;">
                            <b>{{GiamDocCM}}</b> <div class="page2"><b>2</b></div></span></td>
                    </tr>
                </table>
				
            </div>
        </div>
    </div>
</div>
<div class="columnCenTer" style="padding-top: 15px;"></div>
<div class="column" style="padding-top: 15px;">
    <div>
        <div style="margin-left: 0.5cm;margin-right: 0.3cm;">
            <table width="100%">
                <tr>
                    <td colspan="4" style="border: 1px solid black;height: 33px;text-align: center;font-weight: bold;">
                        KHÁM LẠI </td>
                </tr>
                <tr>
                    <td style="border: 1px solid black;height: 33px;text-align: center;">Ngày</td>
                    <td colspan="2" style="border: 1px solid black;height: 33px;text-align: center;">Kết quả</td>
                    <td style="border: 1px solid black;height: 33px;text-align: center;">BS khám</td>
                </tr>
                <tr>
                    <td style="border: 1px solid black;height: 33px;"></td>
                    <td colspan="2" style="border: 1px solid black;height: 33px;"></td>
                    <td style="border: 1px solid black;height: 33px;"></td>
                </tr>
                <tr>
                    <td style="border: 1px solid black;height: 33px;"></td>
                    <td colspan="2" style="border: 1px solid black;height: 33px;"></td>
                    <td style="border: 1px solid black;height: 33px;"></td>
                </tr>
                <tr>
                    <td style="border: 1px solid black;height: 33px;"></td>
                    <td colspan="2" style="border: 1px solid black;height: 33px;"></td>
                    <td style="border: 1px solid black;height: 33px;"></td>
                </tr>
                <tr>
                    <td style="border: 1px solid black;height: 33px;"></td>
                    <td colspan="2" style="border: 1px solid black;height: 33px;"></td>
                    <td style="border: 1px solid black;height: 33px;"></td>
                </tr>
                <tr>
                    <td style="border: 1px solid black;height: 33px;"></td>
                    <td colspan="2" style="border: 1px solid black;height: 33px;"></td>
                    <td style="border: 1px solid black;height: 33px;"></td>
                </tr>
                <tr>
                    <td style="border: 1px solid black;height: 33px;"></td>
                    <td colspan="2" style="border: 1px solid black;height: 33px;"></td>
                    <td style="border: 1px solid black;height: 33px;"></td>
                </tr>
                <tr>
                    <td style="border: 1px solid black;height: 33px;"></td>
                    <td colspan="2" style="border: 1px solid black;height: 33px;"></td>
                    <td style="border: 1px solid black;height: 33px;"></td>
                </tr>
                <tr>
                    <td style="border: 1px solid black;height: 33px;"></td>
                    <td colspan="2" style="border: 1px solid black;height: 33px;"></td>
                    <td style="border: 1px solid black;height: 33px;"></td>
                </tr>
                <tr>
                    <td style="border: 1px solid black;height: 33px;"></td>
                    <td colspan="2" style="border: 1px solid black;height: 33px;"></td>
                    <td style="border: 1px solid black;height: 33px;"></td>
                </tr>
                <tr>
                    <td style="border: 1px solid black;height: 33px;"></td>
                    <td colspan="2" style="border: 1px solid black;height: 33px;"></td>
                    <td style="border: 1px solid black;height: 33px;"></td>
                </tr>
                <tr>
                    <td style="border: 1px solid black;height: 33px;"></td>
                    <td colspan="2" style="border: 1px solid black;height: 33px;"></td>
                    <td style="border: 1px solid black;height: 33px;"></td>
                </tr>
                <tr>
                    <td style="border: 1px solid black;height: 33px;"></td>
                    <td colspan="2" style="border: 1px solid black;height: 33px;"></td>
                    <td style="border: 1px solid black;height: 33px;"></td>
                </tr>
                <tr>
                    <td style="border: 1px solid black;height: 33px;"></td>
                    <td colspan="2" style="border: 1px solid black;height: 33px;"></td>
                    <td style="border: 1px solid black;height: 33px;"></td>
                </tr>
                <tr>
                    <td style="border: 1px solid black;height: 33px;"></td>
                    <td colspan="2" style="border: 1px solid black;height: 33px;"></td>
                    <td style="border: 1px solid black;height: 33px;"> <div class="page3"><b>3</b></div></td>
                </tr>
            </table>
        </div>
    </div>
</div>'
where Name='PhieuChungNhanPhauThuat2'
Update dbo.CauHinh
Set [Value] = '3.7.7' where [Name] = 'CauHinhHeThong.DatabaseVesion'
