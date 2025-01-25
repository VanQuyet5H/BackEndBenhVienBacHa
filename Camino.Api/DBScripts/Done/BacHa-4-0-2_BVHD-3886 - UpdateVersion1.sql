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
        font-size: 16px;
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
        <td style="text-align: center;font-weight: bold;font-size:15px;vertical-align: top;"> <span
                style="padding-top: 5px;"><b>PHIẾU SƠ KẾT 15 NGÀY ĐIỀU
                    TRỊ</b><br> <span style="font-size: 16px;"> Từ ngày:{{TuNgay}}- đến ngày:{{DenNgay}}</span> </td>
        <td style="text-align:center;float:right;vertical-align: top;font-size: 15px;"> <img
                style="height:45px;" src="data:image/png;base64,{{BarCodeImgBase64}}"><br>
            <p style="margin:0;padding:0;">Mã TN {{MaTN}}</p>
        </td>
    </tr>
</table> <br>
<table width="100%">
    <tr>
        <td style="width: 60%;">
            {{HoTenNgBenh}}
        </td>
        <td style="width: 25%;">
            {{TuoiNgBenh}}
        </td>
        <td style="width: 15%;">
            {{GT}}
        </td>
    </tr>
    <tr>
        <td colspan="3" style="width: 100%;">
            {{DiaChi}}
        </td>
    </tr>
    <tr>
        <td style="width: 60%;">
            {{Khoa}}
        </td>
        <td style="width: 20%;">
            {{Buong}}
        </td>
        <td style="width: 20%;">
            {{Giuong}}
        </td>
    </tr>
    <tr>
        <td colspan="3" style="width: 100%;">
            {{ChanDoan}}
        </td>
    </tr>
    <tr>
        <td colspan="3" style="width: 100%;">
            {{DienBienLS}}
        </td>
    </tr>
    <tr>
        <td colspan="3" style="width: 100%;">
            {{XetNghiemCLS}}
        </td>
    </tr>
    <tr>
        <td colspan="3" style="width: 100%;">
            {{QuaTrinhDieuTri}}
        </td>
    </tr>
    <tr>
        <td colspan="3" style="width: 100%;">
            {{DanhGiaKetQua}}
        </td>
    </tr>
    <tr>
        <td colspan="3" style="width: 100%;">
            {{HuongDieuTriTiep}}
        </td>
    </tr>
</table> <br>
<table width="100%">
    <tr>
        <td style="text-align:center;" width="50%"><i>Ngày {{NgayK}} tháng {{ThangK}} năm {{NamK}}</i></td>
        <td></td>
        <td style="text-align:center;" width="45%"><i>Ngày {{Ngay}} tháng {{Thang}} năm {{Nam}}</i></td>
    </tr>
    <tr>
        <td style="text-align:center;" style="font-size: 18px;text-transform: uppercase;"><b>Trưởng khoa</b></td>
        <td></td>
        <td style="font-size: 18px; text-align:center;text-transform: uppercase;"><b>Bác sĩ điều trị</b></td>
    </tr>
    <tr>
        <td style="font-size: 18px;font-style: italic;text-align:center">(Ký và ghi rõ họ tên)</td>
        <td></td>
        <td style="font-size: 18px; text-align:center;font-style: italic;">(Ký và ghi rõ họ tên)</td>
    </tr>
    <tr>
        <td style="font-size: 18px; text-align:center;"><br><br><br><br><b>{{HoTenTruongKhoa}}</b></td>
        <td></td>
        <td style="font-size: 18px; text-align:center;"><br><br><br><br><b>{{HoTenBacSi}}</b></td>
    </tr>
</table>'

where Name=N'PhieuSoKet15NgayDieuTri'

update Template
set body=N'<style>
    table,
    th,
    td {
        border-spacing: 0;
        font-family: Times New Roman;
    }

    th,
    td {
        padding: 3px;
        font-size: 18px !important;
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
        position: relative;
        box-sizing: border-box;
    }

    .container .value_khong_gach_chan {
        width: max-content;
        display: table-cell;
        width: 100%;
        height: 100%;
        position: relative;
        box-sizing: border-box;
    }

    .containerGD {
        width: 50%;
        height: 100%;
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
    }

    .containerGD .value_khong_gach_chan {
        display: table-cell;
        width: 100%;
        height: 100%;
        position: relative;
        box-sizing: border-box;
    }

    #customers {
        width: 100%;
        border-collapse: collapse;
    }

    #customers td {
        border: 1px solid black;
    }

    #customers th {
        color: black;
    }

    span.square {
        vertical-align: bottom;
        border: solid 1px;
        width: 25px;
        height: 20px;
        text-align: center;
        margin: 2px;
        display: inline-block;
    }

    span.square2 {
        vertical-align: bottom;
        border: solid 1px;
        width: 80px;
        height: 20px;
        text-align: center;
        margin: 2px;
        display: inline-block;
    }
</style>
<table width="100%" style="margin-bottom:0;padding:0;">
<tr>
    <td colspan="4" style="vertical-align: top;"> BỆNH VIỆN ĐKQT BẮC HÀ
        <br><b>Khoa Phụ Sản</b>
    </td>
    <td style="text-align:center;float:right;"> <img style="height: 40px;"
            src="data:image/png;base64,{{BarCodeImgBase64}}"><br>
        <p style="margin:0;padding:0;">Mã TN: {{MaTN}}</p>
    </td>
</tr>
</table>
<table style="width: 100%">
<tbody>
    <tr>
        <td colspan="3" style="text-align: center;font-size: 19px !important;">
            <b>PHIẾU SÀNG LỌC
                DINH DƯỠNG</b>
            <br /> (Dùng cho phụ nữ mang thai)
        </td>
    </tr>
</tbody>
</table> <br />
<table width="100%">
<tr>
    <td>
        <div class="container">
            <div class="label">Họ và tên:</div>
            <div class="value"> <b>&nbsp;{{HoTen}}</b> </div>
        </div>
    </td>
</tr>
<tr>
    <td>
        <div class="container">
            <div class="label">Ngày/tháng/năm sinh:</div>
            <div class="value"> <b>&nbsp;{{NamSinh}}</b>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Giới
                tính:<b> Nữ</b> </div>
            <div class="label"></div>
        </div>
    </td>
</tr>
<tr>
    <td>
        <div class="container">
            <div class="label">Địa chỉ:</div>
            <div class="value breakword">&nbsp; {{DiaChi}} </div>
        </div>
    </td>
</tr>
<tr>
    <td>
        <div class="containerGD">
            <div class="label">Tuổi thai:</div>
            <div class="value">&nbsp;{{TuoiThai}} </div>
            <div class="label"> &nbsp;tuần&nbsp;&nbsp;&nbsp;Theo kinh cuối cùng
                <input type="checkbox" id="TheoKinhCuoiCung" name="TheoKinhCuoiCung"
                    {{TheoKinhCuoiCung}} disabled />&nbsp;&nbsp;&nbsp;
                Siêu âm 3 tháng đầu thai kỳ <input type="checkbox"
                    id="BaThangDauThaiKy" name="BaThangDauThaiKy"
                    {{BaThangDauThaiKy}} disabled />
            </div>
        </div>
    </td>
</tr>
<tr>
    <td>
        <div class="container">
            <div class="label">Số buồng:</div>
            <div class="value" style="width: 30%"> &nbsp;{{SoBuong}} </div>
            <div class="label">Số giường:</div>
            <div class="value" style="width: 30%">&nbsp;{{SoGiuong}}</div>
            <div class="label">Số bệnh án:</div>
            <div class="value breakword"> &nbsp;{{SoBenhAn}} </div>
        </div>
    </td>
</tr>
<tr>
    <td>
        {{ChanDoan}}
    </td>
</tr>
<tr>
    <td>
        <div class="containerGD">
            <div class="label">Cân nặng trước khi mang thai:&nbsp;{{CanNang}} kg
                Chiều
                cao&nbsp;{{ChieuCao}}
                m </div>
        </div>
    </td>
</tr>
<tr>
    <td>
        <div class="containerGD">
            <div class="label">BMI trước khi mang thai:&nbsp;{{BMIDisplay}}
                kg/m<sup>2</sup>
            </div>
        </div>
    </td>
</tr>
<tr>
    <td>
        <div class="containerGD">
            <div class="label">Cân nặng hiện tại:</div>
            <div class="value">&nbsp;{{CanNangHienTai}} kg </div>
        </div>
    </td>
</tr>
</table>
<table id="customers">
<tr>
    <td rowspan="3" width="25%">BMI trước mang thai</td>
    <td style="text-align: right; border-bottom: hidden" width="43%">18,5 - 24,9</td>
    <td style="border-bottom: hidden" width="32%" 0 điểm <input type="checkbox"
            id="BMITruocMangThaiBT185N249" name="BMITruocMangThai"
            {{BMITruocMangThaiBT185N249}} disabled /> </td>
</tr>
<tr style="border-bottom: hidden">
    <td style="text-align: right" align="top">&ge; 25,0</td>
    <td> 1 điểm <input type="checkbox" id="BMITruocMangThaiGE25"
            name="BMITruocMangThai" {{BMITruocMangThaiGE25}} disabled /> </td>
</tr>
<tr>
    <td style="text-align: right">
        < 18,5</td>
    <td> 1 điểm <input type="checkbox" id="BMITruocMangThaiLT185"
            name="BMITruocMangThai" {{BMITruocMangThaiLT185}} disabled /> </td>
</tr>
<tr>
    <td rowspan="2">Tốc độ tăng cân</td>
    <td style="border-bottom: hidden">Tăng cân theo khuyến nghị</td>
    <td style="border-bottom: hidden"> 0 điểm <input type="checkbox"
            id="TocDoTangCanTheoKhuyenNghi" name="TocDoTangCan"
            {{TocDoTangCanTheoKhuyenNghi}} disabled /> </td>
</tr>
<tr>
    <td>Tăng cân trên, hoặc dưới mức khuyến nghị</td>
    <td> 1 điểm <input type="checkbox" id="TocDoTangCanTrenDuoiMucKhuyenNghi"
            name="TocDoTangCan" {{TocDoTangCanTrenDuoiMucKhuyenNghi}} disabled />
    </td>
</tr>
<tr>
    <td rowspan="2">Bệnh kèm theo</td>
    <td style="border-bottom: hidden">Không</td>
    <td style="border-bottom: hidden"> 0 điểm <input type="checkbox"
            id="BenhKemTheoKhong" name="BenhKemTheo" {{BenhKemTheoKhong}}
            disabled /> </td>
</tr>
<tr>
    <td>Tăng huyết áp, đái tháo đường, nghén nặng...</td>
    <td> 1 điểm <input type="checkbox" id="BenhKemTheoTangHuyetAp"
            name="BenhKemTheo" {{BenhKemTheoTangHuyetAp}} disabled /> </td>
</tr>
<tr>
    <td rowspan="2" style="text-align: center"><b>Kết luận</b></td>
    <td style="text-align: right; border-bottom: hidden">
        <2 Điểm</td>
    <td style="border-bottom: hidden"> Bình thường <input type="checkbox"
            id="KetLuanBinhThuong" name="KetLuan" {{KetLuanBinhThuong}} disabled />
    </td>
</tr>
<tr>
    <td style="text-align: right">&ge; 2 điểm</td>
    <td> Có nguy cơ về dinh dưỡng <input type="checkbox"
            id="KetLuanCoNguyCoDinhDuong" name="KetLuan"
            {{KetLuanCoNguyCoDinhDuong}} disabled /> </td>
</tr>
</table> <br />
<table width="100%">
<tr>
    <td width="33%">&nbsp;</td>
    <td width="25%">&nbsp;</td>
    <td style="text-align: center;"> <i><span
                style="font-style: italic;">Ngày
                {{Ngay}} tháng
                {{Thang}} năm {{Nam}}</span></i><br /> <span
            style="text-transform: uppercase;font-weight: bold;"> Người
            thực hiện </span> <br><span><i>(Ký và ghi rõ họ tên)</i></span> <br />
        <br /> <br /> <br /> <br /><b> {{NguoiThucHien}}</b>
    </td>
</tr>
</table>
<div class="pagebreak"></div>
<table style="width: 100%">
<tbody>
    <tr>
        <td colspan="3" style="text-align: center; font-size: 16px"> <b>HƯỚNG DẪN
                SÀNG LỌC DINH
                DƯỠNG CHO PHỤ NỮ
                MANG THAI</b> </td>
    </tr>
</tbody>
</table> <br />
<table style="width: 100%">
<tr>
    <td>
        <div>
            <div class="label"> 1. Đối tượng đánh giá: tất cả phụ nữ mang thai nằm
                viện đều được
                sàng lọc dinh
                dưỡng. </div>
            <div class="label"> 2. Thời gian thực hiện: trong vòng 48 giờ sau nhập
                viện. </div>
            <div class="label">3. Cán bộ thực hiện: Bác sỹ điều trị.</div>
            <div class="label"> 4. Bảng mức tăng cân của bà mẹ và bào thai trong
                thai kỳ: </div>
        </div>
    </td>
</tr>
<tr>
    <td>
        <table id="customers" style="text-align: center">
            <tr>
                <td style="text-align: center; width: 18%">Mức tăng cân</td>
                <td style="text-align: center; width: 18%">3 tháng đầu (quý I)</td>
                <td style="text-align: center; width: 18%">3 tháng giữa (quý II)
                </td>
                <td style="text-align: center; width: 18%">3 tháng cuối (quý III)
                </td>
            </tr>
            <tr>
                <td>Mẹ</td>
                <td>1 kg</td>
                <td>4 - 5 kg</td>
                <td>5 - 6 kg</td>
            </tr>
            <tr>
                <td>Bào thai</td>
                <td>0,1 kg</td>
                <td>1 kg</td>
                <td>2 kg</td>
            </tr>
        </table>
    </td>
</tr>
<tr>
    <td>
        <div>
            <div class="label" style="width: 100%"> - Phụ nữ mang thai không rõ cân
                nặng trước khi
                mang thai: sử
                dụng BMI trong lần khám thai đầu tiên trong 3 tháng đầu thai kỳ để
                khuyến nghị mức
                tăng cân: Theo
                tiêu chuẩn quốc tế (FAO) mức tăng cân <br /> trung bình của phụ nữ
                châu á nên là 10
                – 12kg. </div>
            <div class="label" style="width: 100%"> - Tình trạng dinh dưỡng tốt
                (BMI: 18,5 – 24,9):
                Mức tăng cân nên
                đạt là 20% cân nặng trước khi mang thai. </div>
            <div class="label" style="width: 100%"> - Tình trạng dinh dưỡng gầy (BMI
                <18,5):Mức tăng cân nên đạt là 25% cân nặng trước khi mang thai.
                    <br/> - Tình trạng
                dinh dưỡng thừa
                cân – béo phì (BMI ≥ 25): Mức tăng cân nên đạt là 15% cân nặng trước
                khi mang thai.
            </div>
        </div>
    </td>
</tr>
</table>'
where Name='PhieuSangLocDinhDuongPhuSan'
update template 
set Body=N'<style>
    table,
    th,
    td {
        border-spacing: 0;
        font-family: Times New Roman;
    }

    th,
    td {
        padding: 3px;
        font-size: 18px;
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

    .container .values {
        display: table-cell;
        width: 100%;
        height: 100%;
        vertical-align: top;
        position: relative;
        box-sizing: border-box;
    }

    #customers {
        width: 100%;
        border-collapse: collapse;
    }

    #customers td {
        border: 1px solid black;
        width: 25%;
        word-break: break-work;
    }

    #customers th {
        color: black;
        word-break: break-work;
        text-align: center;
        width: 25%;
    }

    span.square {
        border: solid 1px;
        width: 25px;
        height: 20px;
        text-align: center;
        margin: 2px;
        display: inline-block
    }

    span.square2 {
        border: solid 1px;
        width: 80px;
        height: 20px;
        text-align: center;
        margin: 2px;
        display: inline-block
    }

    .diemTong {
        text-align: center;
    }

    .keHoachCanThiepDinhDuong {
        text-align: center;
    }

    .keHoachCanThiepDinhDuongtd {
        text-align: left;
    }

    #customerNguyCos {
        width: 50%;
        border-collapse: collapse;
    }

    #customerNguyCos td {
        border: 1px solid black;
        width: 25%;
        word-break: break-work;
    }

    #customerNguyCos th {
        color: black;
        word-break: break-work;
        text-align: center;
        width: 25%;
    }
</style>
<table width="100%" style="margin-bottom:0;">
<tr>
    <td colspan="4" style="vertical-align: top;padding:0;"> BỆNH VIỆN ĐKQT BẮC HÀ
        <br><b>{{KhoaDangIn}}</b>
    </td>
    <td style="text-align:center;float:right;vertical-align: top;"> <img style="height: 40px;"
            src="data:image/png;base64,{{BarCodeImgBase64}}"><br>
        <p style="margin:0;padding:0;">Mã TN: {{MaTN}}</p>
    </td>
</tr>
</table>
<table style="width: 100%;margin-top: 25px;">
<tbody>
    <tr>
        <td style="text-align:center; font-size:18px;"><b>PHIẾU ĐÁNH GIÁ VÀ CAN THIỆP DINH
                DƯỠNG</b><br>(Dùng cho
            người bệnh trên 18 tuổi)</td>
    </tr>
</tbody>
</table> <br>
<table width="100%">
<tr>
    <td style="width: 100%;" colspan="3"> {{HoTen}} </td>
</tr>
<tr>
    <td style="width: 100%;" colspan="3"> {{NgayThangNamSinh}}</td>
</tr>
<tr>
    <td colspan="3" style="width: 100%;"> {{ChanDoan}} </td>
</tr>
<tr>
    <td colspan="3"> Cân nặng :{{CanNang}} </td>
</tr>
</table>
<table width="100%">
<tr>
    <td colspan="2"> <b>I.SÀNG LỌC VÀ ĐÁNH GIÁ DINH DƯỠNG</b></td>
</tr>
</table>
<table id="customers">
<tr>
    <td class="diemTong"> <b>Điểm</b></td>
    <td class="diemTong"> <b>0</b></td>
    <td class="diemTong"> <b>1</b></td>
    <td class="diemTong"> <b>2</b></td>
</tr> {{SangLocDinhDuong}} <tr>
    <td colspan="4" class="diemTong">Điểm tổng: &nbsp;{{DiemTong}}</td>
</tr>
</table>
<table width="100%">
<tr>
    <td colspan="2"> <b>II.PHÂN LOẠI NGUY CƠ DINH DƯỠNG</b></td>
</tr>
</table>
<table id="customerNguyCos"> {{NguyCoDinhDuong}} </table>
<table width="100%">
<tr>
    <td colspan="2"> <b>III.KẾ HOẠCH CAN THIỆP DINH DƯỠNG</b></td>
</tr>
</table>
<table id="customers">
<tr>
    <td class="keHoachCanThiepDinhDuong" style="width: 25%;"> <b>Nội dung</b></td>
    <td colspan="3" class="keHoachCanThiepDinhDuong" style="width: 75%;"> <b>Chỉ định</b></td>
</tr>
<tr>
    <td class="keHoachCanThiepDinhDuongtd" style="width: 25%;"><i> Chế độ ăn uống</i></td>
    <td colspan="3" class="keHoachCanThiepDinhDuongtd" style="width: 75%;"><i> Mã số: {{CheDoAnUongDisplay}}
    </i></td>
</tr>
<tr>
    <td class="keHoachCanThiepDinhDuongtd" style="width: 25%;"> Đường nuôi dưỡng</td>
    <td class="keHoachCanThiepDinhDuongtd" style="width: 25%;"> Đường miệng <input type="checkbox"
            id="dm" name="dm" {{DuongMieng}} disabled style="float: right;"></td>
    <td class="keHoachCanThiepDinhDuongtd" style="width: 25%;"> Ống thông <input type="checkbox" id="ot"
            name="ot" {{OngThong}} disabled style="float: right;"></td>
    <td class="keHoachCanThiepDinhDuongtd" style="width: 25%;"> Tĩnh mạch <input type="checkbox" id="ot"
            name="ot" {{TinhMach}} disabled style="float: right;"></td>
</tr>
<tr>
    <td class="keHoachCanThiepDinhDuongtd" style="width: 25%;"> Hội chẩn dinh dưỡng</td>
    <td class="keHoachCanThiepDinhDuongtd" style="width: 25%;"> Không <input type="checkbox" id="khong"
            name="khong" {{Khong}} disabled style="float: right;"></td>
    <td class="keHoachCanThiepDinhDuongtd" style="width: 25%;"> Có <input type="checkbox" id="co"
            name="co" {{Co}} disabled style="float: right;"></td>
    <td class="keHoachCanThiepDinhDuongtd" style="width: 25%;"> </td>
</tr>
<tr>
    <td class="keHoachCanThiepDinhDuongtd" style="width: 25%;"> Tái đánh giá</td>
    <td class="keHoachCanThiepDinhDuongtd" style="width: 25%;"> Sau 3 ngày <input type="checkbox"
            id="khong" name="khong" {{TaiDanhGiaBaNgay}} disabled style="float: right;"></td>
    <td class="keHoachCanThiepDinhDuongtd" style="width: 25%;"> Sau 7 ngày <input type="checkbox"
            id="co" name="co" {{TaiDanhGiaBayNgay}} disabled style="float: right;"></td>
    <td class="keHoachCanThiepDinhDuongtd" style="width: 25%;"> </td>
</tr>
</table>
<table width="100%">
<tr>
    <td width="25%"></td>
    <td style="text-align:center;font-size: 18px" width="25%"><i>Ngày {{Ngay}} tháng {{Thang}} năm
            {{Nam}}</i></td>
</tr>
<tr>
    <td style="font-size: 18px" width="25%"></td>
    <td style="font-size: 18px; text-align:center;" width="25%"><b>BÁC SỸ ĐIỀU TRỊ</b><br> <i>(Ký và ghi
            rõ họ
            tên)</i></td>
</tr>
<td style="font-size: 18px" width="25%"></td>
<td style="font-size: 18px; text-align:center;" width="25%"><br><br><br><br>{{BacSyDieuTri}}</td>
</tr>
</table>'
where Name='PhieuSangLocDinhDuong'


Update dbo.CauHinh
Set [Value] = '4.0.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'