update Template
set Body=N'<style>
    table,
    th,
    td {
        /* border-collapse: collapse; */
        font-family: Times New Roman;
        font-size: 12px;
        height: 5px;
    }

    th,
    td {
        padding: 0px;
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
        border-bottom: 0.5px dotted black;
    }

    .container .values {
        display: table-cell;
        width: 100%;
        height: 100%;
        vertical-align: top;
        position: relative;
        box-sizing: border-box;
    }

    span.square {
        border: solid 1px;
        width: 25px;
        height: 20px;
        text-align: center;
        margin: 2px;
        display: inline-block;
        margin-top: 2px;
    }

    .paddingTable {
        padding: 5px;
        padding-top: 0px;
        padding-bottom: 0px;
    }
</style>
<div>
    <div style="width: 100%;">
        <table width="100%" style="margin-bottom:0;">
            <tr>
                <td colspan="4" style="vertical-align: top;font-size: 14px;width: 30%;"> BỆNH VIỆN ĐKQT BẮC HÀ
                    <br><b>Khoa&nbsp;{{KhoaPhongDangIn}}</b> </td>
                <td style="font-size: 17px;font-weight: bold;text-align: center;width: 50%;"> BẢNG KIỂM AN TOÀN NGƯỜI
                    BỆNH<br> TRƯỚC PHẪU THUẬT </td>
                <td style="text-align:center;float:right;"> <img style="height: 40px;"
                        src="data:image/png;base64,{{BarCodeImgBase64}}"><br>
                    <p style="margin:0;padding:0;font-size: 12px;">Mã TN: {{MaTN}}</p>
                </td>
            </tr>
        </table> <br>
    </div>
</div> <br>
<table width="100%">
    <tbody>
        <tr>
            <td width="100%" style="text-align: right;"> Ngày<span
                    style="height: 100%;vertical-align: top;position: relative;box-sizing: border-box;"> {{Ngay}}
                </span> <span
                    style="height: 100%;vertical-align: top;position: relative;box-sizing: border-box;">tháng<span>
                        {{Thang}} </span> năm<span
                        style="height: 100%;vertical-align: top;position: relative;box-sizing: border-box;">
                        {{Nam}}</span> </td>
        </tr>
    </tbody>
</table>
<table style="width:100%">
    <tr>
        <td style="width: 55%;"> {{HoTenNguoiBenh}} </td>
        <td style="width: 10%;"> {{Tuoi}} </td>
        <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Giới: 1. Nam:
            &nbsp;&nbsp;<span class="square" style="text-align: center;"> {{GioiTinhNam}} </span>&nbsp;&nbsp;&nbsp;2.
            Nữ: &nbsp;&nbsp;<span class="square" style="text-align: center;"> {{GioiTinhNu}} </span> </td>
    </tr>
</table>
<table style="width:100%">
    <tr>
        <td style="width: 50%;"> {{KhoaPhong}} </td>
        <td style="width: 50%;"> {{Giuong}} </td>
    </tr>
    <tr>
        <td style="width: 100%;" colspan="2"> {{ChanDoan}} </td>
    </tr>
    <tr>
        <td style="width: 100%;" colspan="2"> {{ThuocDangDung}} </td>
    </tr>
    <tr>
        <td style="width: 100%;" colspan="2"> {{TienSuDiUng}} </td>
    </tr>
</table>
<table style="width:100%;border-collapse: collapse;">
    <tr>
        <th rowspan="2" colspan="2"
            style=" border: 1px solid black;width: 50%;padding: 0px;margin: 0px;text-align: left;padding: 2px;text-align: center;">
            <b>Chuẩn bị người bệnh </b> </th>
        <th style=" border: 1px solid black;width: 4%;padding: 0px;margin: 0px;font-weight: normal;">Có </th>
        <th style=" border: 1px solid black;width: 6%;padding: 0px;margin: 0px;font-weight: normal;">Không </th>
        <th rowspan="2" colspan="2"
            style=" border: 1px solid black;width: 40%;padding: 0px;margin: 0px;text-align: left;padding: 2px;text-align: center;">
            <b>Các thông số: </b> </th>
    </tr>
    <tbody>
        <tr>
            <td colspan="2"
                style=" border: 1px solid black;width: 50%;padding: 0px;margin: 0px;text-align: left;padding: 2px;">
                <div class="paddingTable">Khám và tư vấn của PTV </div>
            </td>
            <th style=" border: 1px solid black;width: 4%;padding: 0px;margin: 0px;font-weight: normal;">
                {{KhamVaTuVanCuaPTV}} </th>
            <th style=" border: 1px solid black;width: 6%;padding: 0px;margin: 0px;font-weight: normal;">
                {{KhamVaTuVanCuaPTVKhong}} </th>
            <th
                style=" border: 1px solid black;width: 15%;padding: 0px;margin: 0px;font-weight: normal;text-align: left;padding: 2px;">
                <div class="paddingTable">Tiếp xúc</div>
            </th>
            <th style=" border: 1px solid black;width: 25%;padding: 0px;margin: 0px;font-weight:normal;"> {{TiepXuc}}
            </th>
        </tr>
        <tr>
            <td rowspan="2"
                style=" border: 1px solid black;width: 15%;padding: 0px;margin: 0px;font-weight: normal;text-align: left;padding: 2px;">
                <div class="paddingTable">Chẩn đoán:</div>
            </td>
            <td
                style=" border: 1px solid black;width: 35%;padding: 0px;margin: 0px;font-weight: normal;text-align: left;padding: 2px;">
                <div class="paddingTable">Bệnh chính</div>
            </td>
            <th style=" border: 1px solid black;width: 4%;padding: 0px;margin: 0px;font-weight: normal;">
                {{BenhChinhStringCo}} </th>
            <th style=" border: 1px solid black;width: 6%;padding: 0px;margin: 0px;font-weight: normal;">
                {{BenhChinhStringKhong}} </th>
            <th
                style=" border: 1px solid black;width: 15%;padding: 0px;margin: 0px;font-weight: normal;text-align: left;padding: 2px;">
                <div class="paddingTable">Chiều cao </div>
            </th>
            <th style=" border: 1px solid black;width: 25%;padding: 0px;margin: 0px;font-weight: normal;"> {{ChieuCao}}
            </th>
        </tr>
        <tr>
            <td
                style=" border: 1px solid black;width: 35%;padding: 0px;margin: 0px;font-weight: normal;text-align: left;padding: 2px;">
                <div class="paddingTable">Bệnh kèm theo </div>
            </td>
            <th style=" border: 1px solid black;width: 5%;padding: 0px;margin: 0px;font-weight: normal;">
                {{BenhKemTheoString}} </th>
            <th style=" border: 1px solid black;width: 5%;padding: 0px;margin: 0px;font-weight: normal;">
                {{BenhKemTheoStringKhong}} </th>
            <th
                style=" border: 1px solid black;width: 15%;padding: 0px;margin: 0px;font-weight: normal;text-align: left;padding: 2px;">
                <div class="paddingTable">Cân nặng</div>
            </th>
            <th style=" border: 1px solid black;width: 25%;padding: 0px;margin: 0px;font-weight: normal;"> {{CanNang}}
            </th>
        </tr>
        <tr>
            <td rowspan="3"
                style=" border: 1px solid black;width: 15%;padding: 0px;margin: 0px;font-weight: normal;text-align: left;padding: 2px;">
                <div class="paddingTable">Tiền sử bệnh:</div>
            </td>
            <td
                style=" border: 1px solid black;width: 35%;padding: 0px;margin: 0px;font-weight: normal;text-align: left;padding: 2px;">
                <div class="paddingTable">Đái tháo đường</div>
            </td>
            <th style=" border: 1px solid black;width: 4%;padding: 0px;margin: 0px;font-weight: normal;">
                {{DaiThaoDuong}} </th>
            <th style=" border: 1px solid black;width: 6%;padding: 0px;margin: 0px;font-weight: normal;">
                {{DaiThaoDuongKhong}} </th>
            <th
                style=" border: 1px solid black;width: 15%;padding: 0px;margin: 0px;font-weight: normal;text-align: left;padding: 2px;">
                <div class="paddingTable">Mạch</div>
            </th>
            <th style=" border: 1px solid black;width: 25%;padding: 0px;margin: 0px;font-weight: normal;"> {{Mach}}
            </th>
        </tr>
        <tr>
            <td
                style=" border: 1px solid black;width: 35%;padding: 0px;margin: 0px;font-weight: normal;text-align: left;padding: 2px;">
                <div class="paddingTable">Tăng huyết áp</div>
            </td>
            <th style=" border: 1px solid black;width: 5%;padding: 0px;margin: 0px;font-weight: normal;">
                {{TangHuyetAp}} </th>
            <th style=" border: 1px solid black;width: 5%;padding: 0px;margin: 0px;font-weight: normal;">
                {{TangHuyetApKhong}} </th>
            <th
                style=" border: 1px solid black;width: 15%;padding: 0px;margin: 0px;font-weight: normal;text-align: left;padding: 2px;">
                <div class="paddingTable">Nhiệt độ</div>
            </th>
            <th style=" border: 1px solid black;width: 25%;padding: 0px;margin: 0px;font-weight: normal;"> {{NhietDo}}
            </th>
        </tr>
        <tr>
            <td
                style=" border: 1px solid black;width: 35%;padding: 0px;margin: 0px;font-weight: normal;text-align: left;padding: 2px;">
                <div class="paddingTable">Khác:</div>
            </td>
            <th style=" border: 1px solid black;width: 5%;padding: 0px;margin: 0px;font-weight: normal;"> {{Khac}} </th>
            <th style=" border: 1px solid black;width: 5%;padding: 0px;margin: 0px;font-weight: normal;"> {{KhacKhong}}
            </th>
            <th
                style=" border: 1px solid black;width: 15%;padding: 0px;margin: 0px;font-weight: normal;text-align: left;padding: 2px;">
                <div class="paddingTable">Huyết áp</div>
            </th>
            <th style=" border: 1px solid black;width: 25%;padding: 0px;margin: 0px;font-weight: normal;"> {{HuyetAp}}
            </th>
        </tr>
        <tr>
            <td colspan="2"
                style=" border: 1px solid black;width: 50%;padding: 0px;margin: 0px;text-align: left;padding: 2px;">
                <div class="paddingTable">Thẻ định danh</div>
            </td>
            <td style=" border: 1px solid black;width: 4%;padding: 0px;margin: 0px;text-align: center;">
                {{TheDinhDanhCo}} </td>
            <td style=" border: 1px solid black;width: 6%;padding: 0px;margin: 0px;text-align: center;">
                {{TheDinhDanhKhong}} </td>
            <th
                style=" border: 1px solid black;width: 15%;padding: 0px;margin: 0px;font-weight: normal;text-align: left;padding: 2px;">
                <div class="paddingTable">Nhịp thở</div>
            </th>
            <th style=" border: 1px solid black;width: 25%;padding: 0px;margin: 0px;font-weight:normal;"> {{NhipTho}}
            </th>
        </tr>
        <tr>
            <td colspan="2"
                style=" border: 1px solid black;width: 50%;padding: 0px;margin: 0px;text-align: left;padding: 2px;">
                <div class="paddingTable">Dặn người bệnh nhịn ăn uống trước 6 giờ</div>
            </td>
            <td style=" border: 1px solid black;width: 4%;padding: 0px;margin: 0px;text-align: center;">
                {{DanNguoiBenhCo}} </td>
            <td style=" border: 1px solid black;width: 6%;padding: 0px;margin: 0px;text-align: center;">
                {{DanNguoiBenhKhong}} </td>
            <th
                style=" border: 1px solid black;width: 15%;padding: 0px;margin: 0px;font-weight: normal;text-align: left;padding: 2px;">
            </th>
            <th style=" border: 1px solid black;width: 25%;padding: 0px;margin: 0px;font-weight:normal;"> </th>
        </tr>
        <tr>
            <td colspan="2"
                style=" border: 1px solid black;width: 50%;padding: 0px;margin: 0px;text-align: left;padding: 2px;">
                <div class="paddingTable">Vệ sinh (tắm gội)</div>
            </td>
            <td style=" border: 1px solid black;width: 4%;padding: 0px;margin: 0px;text-align: center;">
                {{VeSinhTamGoi}} </td>
            <td style=" border: 1px solid black;width: 6%;padding: 0px;margin: 0px;text-align: center;">
                {{VeSinhTamGoiKhong}} </td>
            <th
                style=" border: 1px solid black;width: 15%;padding: 0px;margin: 0px;font-weight: normal;text-align: left;padding: 2px;">
            </th>
            <th style=" border: 1px solid black;width: 25%;padding: 0px;margin: 0px;font-weight:normal;"> </th>
        </tr>
        <tr>
            <td colspan="2"
                style=" border: 1px solid black;width: 50%;padding: 0px;margin: 0px;text-align: left;padding: 2px;">
                <div class="paddingTable">Vật liệu cấy ghép(Prothese)</div>
            </td>
            <td style=" border: 1px solid black;width: 4%;padding: 0px;margin: 0px;text-align: center;">
                {{VatLieuCayGhep}} </td>
            <td style=" border: 1px solid black;width: 6%;padding: 0px;margin: 0px;text-align: center;">
                {{VatLieuCayGhepKhong}} </td>
            <th
                style=" border: 1px solid black;width: 15%;padding: 0px;margin: 0px;font-weight: normal;text-align: left;padding: 2px;">
                <div class="paddingTable">Nhóm máu</div>
            </th>
            <th style=" border: 1px solid black;width: 25%;padding: 0px;margin: 0px;font-weight:normal;"> {{NhomMau}}
            </th>
        </tr>
        <tr>
            <td colspan="2"
                style=" border: 1px solid black;width: 50%;padding: 0px;margin: 0px;text-align: left;padding: 2px;">
                <div class="paddingTable">Cắt móng tay, móng chân</div>
            </td>
            <td style=" border: 1px solid black;width: 4%;padding: 0px;margin: 0px;text-align: center;">
                {{CatMongTayMongChan}} </td>
            <td style=" border: 1px solid black;width: 6%;padding: 0px;margin: 0px;text-align: center;">
                {{CatMongTayMongChanKhong}} </td>
            <th
                style=" border: 1px solid black;width: 15%;padding: 0px;margin: 0px;font-weight: normal;text-align: left;padding: 2px;">
                <div class="paddingTable">Khác</div>
            </th>
            <th style=" border: 1px solid black;width: 25%;padding: 0px;margin: 0px;font-weight:normal;">
                {{ThongSoKhac}} </th>
        </tr>
        <tr>
            <td colspan="2"
                style=" border: 1px solid black;width: 50%;padding: 0px;margin: 0px;text-align: left;padding: 2px;">
                <div class="paddingTable">Đồ trang sức
            </td>
            <td style=" border: 1px solid black;width: 4%;padding: 0px;margin: 0px;text-align: center;"> {{DoTrangSuc}}
            </td>
            <td style=" border: 1px solid black;width: 6%;padding: 0px;margin: 0px;text-align: center;">
                {{DoTrangSucKhong}} </td>
            <th colspan="2"
                style=" border: 1px solid black;width: 15%;padding: 0px;margin: 0px;font-weight: normal;text-align: left;padding: 2px;">
                <div class="paddingTable"><b>Nguy cơ suy hôm hấp, mất máu:</b></div>
            </th>
        </tr>
        <tr>
            <td colspan="2"
                style=" border: 1px solid black;width: 50%;padding: 0px;margin: 0px;text-align: left;padding: 2px;">
                <div class="paddingTable">Tháo răng giả</div>
            </td>
            <td style=" border: 1px solid black;width: 4%;padding: 0px;margin: 0px;text-align: center;"> {{ThaoRangGia}}
            </td>
            <td style=" border: 1px solid black;width: 6%;padding: 0px;margin: 0px;text-align: center;">
                {{ThaoRangGiaKhong}} </td>
            <th colspan="2" rowspan="4"
                style=" border: 1px solid black;width: 40%;padding: 0px;margin: 0px;font-weight: normal;text-align: left;padding: 2px;">
                {{NguyCoSuyHoHapMatMau}} </th>
        </tr>
        <tr>
            <td colspan="2"
                style=" border: 1px solid black;width: 50%;padding: 0px;margin: 0px;text-align: left;padding: 2px;">
                <div class="paddingTable">Quần áo sạch(quần, áo, váy )</div>
            </td>
            <td style=" border: 1px solid black;width: 4%;padding: 0px;margin: 0px;text-align: center;">
                {{QuanAoSachMoiThay}} </td>
            <td style=" border: 1px solid black;width: 6%;padding: 0px;margin: 0px;text-align: center;">
                {{QuanAoSachMoiThayKhong}} </td>
        </tr>
        <tr>
            <td colspan="2"
                style=" border: 1px solid black;width: 50%;padding: 0px;margin: 0px;text-align: left;padding: 2px;">
                <div class="paddingTable"> Vết thương hở</div>
            </td>
            <td style=" border: 1px solid black;width: 4%;padding: 0px;margin: 0px;text-align: center;"> {{VetThuongHo}}
            </td>
            <td style=" border: 1px solid black;width: 6%;padding: 0px;margin: 0px;text-align: center;">
                {{VetThuongHoKhong}} </td>
        </tr>
        <tr>
            <td colspan="2"
                style=" border: 1px solid black;width: 50%;padding: 0px;margin: 0px;text-align: left;padding: 2px;">
                <div class="paddingTable">Vệ sinh da vùng mổ</div>
            </td>
            <td style=" border: 1px solid black;width: 4%;padding: 0px;margin: 0px;text-align: center;">
                {{VeSinhDaVungMo}} </td>
            <td style=" border: 1px solid black;width: 6%;padding: 0px;margin: 0px;text-align: center;">
                {{VeSinhDaVungMoKhong}} </td>
        </tr>
        <tr style="text-align: left">
            <td colspan="2"
                style=" border: 1px solid black;width: 50%;padding: 0px;margin: 0px;text-align: left;padding: 2px;">
                <div class="paddingTable">Băng vô trùng da/đánh dấu vị trí phẫu thuật</div>
            </td>
            <td style=" border: 1px solid black;width: 4%;padding: 0px;margin: 0px;text-align: center;">
                {{BangVoTrungDanhDauViTriPhauThuat}} </td>
            <td style=" border: 1px solid black;width: 6%;padding: 0px;margin: 0px;text-align: center;">
                {{BangVoTrungDanhDauViTriPhauThuatKhong}} </td>
            </th>
            <th colspan="2"
                style=" border: 1px solid black;width: 15%;padding: 0px;margin: 0px;font-weight: normal;text-align: left;padding: 2px;">
                <div class="paddingTable"><b>Lâm sàng, Cận LS cần lưu ý</b></div>
            </th>
        </tr>
        <tr style="text-align: left">
            <td colspan="2"
                style=" border: 1px solid black;width: 50%;padding: 0px;margin: 0px;text-align: left;padding: 2px;">
                <div class="paddingTable">Khám gây mê</div>
            </td>
            <td style=" border: 1px solid black;width: 4%;padding: 0px;margin: 0px;text-align: center;"> {{KhamGayMe}}
            </td>
            <td style=" border: 1px solid black;width: 6%;padding: 0px;margin: 0px;text-align: center;">
                {{KhamGayMeKhong}} </td>
            <td colspan="2" rowspan="5"
                style=" border: 1px solid black;width: 40%;padding: 0px;margin: 0px;font-weight: normal;text-align: left;padding: 2px;">
                {{LamSangCLSCanLuuY}} </td>
        </tr>
        <tr>
            <td colspan="2"
                style=" border: 1px solid black;width: 50%;padding: 0px;margin: 0px;text-align: left;padding: 2px;">
                <div class="paddingTable"> Hồ sơ bệnh án đủ: Phiếu cam đoan PT/TT, GMHS</div>
            </td>
            <td style=" border: 1px solid black;width: 4%;padding: 0px;margin: 0px;text-align: center;"> {{PTTTGMHS}}
            </td>
            <td style=" border: 1px solid black;width: 6%;padding: 0px;margin: 0px;text-align: center;">
                {{PTTTGMHSKhong}} </td>
        </tr>
        <tr>
            <td colspan="2"
                style=" border: 1px solid black;width: 50%;padding: 0px;margin: 0px;text-align: left;padding: 2px;">
                <div class="paddingTable">Phiếu xét nghiệm:CTM, nhóm máu, đông máu, HBsAg, HIV, sinh hoá nước tiểu, sinh
                    hoá máu</div>
            </td>
            <td style=" border: 1px solid black;width: 4%;padding: 0px;margin: 0px;text-align: center;">
                {{PhieuXetNghiemNhomMauDongMau}} </td>
            <td style=" border: 1px solid black;width: 6%;padding: 0px;margin: 0px;text-align: center;">
                {{PhieuXetNghiemNhomMauDongMauKhong}} </td>
        </tr>
        <tr>
            <td colspan="2"
                style=" border: 1px solid black;width: 50%;padding: 0px;margin: 0px;text-align: left;padding: 2px;">
                <div class="paddingTable">Phim chụp phổi, số lượng</div>
            </td>
            <td style=" border: 1px solid black;width: 4%;padding: 0px;margin: 0px;text-align: center;">
                {{PhimChupPhoiSoLuong}} </td>
            <td style=" border: 1px solid black;width: 6%;padding: 0px;margin: 0px;text-align: center;">
                {{PhimChupPhoiSoLuongKhong}} </td>
        </tr>
        <tr>
            <td colspan="2"
                style=" border: 1px solid black;width: 50%;padding: 0px;margin: 0px;text-align: left;padding: 2px;">
                <div class="paddingTable">Các loại phim ảnh khác, số lượng</div>
            </td>
            <td style=" border: 1px solid black;width: 4%;padding: 0px;margin: 0px;text-align: center;">
                {{CacLoaiPhimAnhKhacSoLuong}} </td>
            <td style=" border: 1px solid black;width: 6%;padding: 0px;margin: 0px;text-align: center;">
                {{CacLoaiPhimAnhKhacSoLuongKhong}} </td>
        </tr>
        <tr style="text-align: left">
            <td colspan="2"
                style=" border: 1px solid black;width: 50%;padding: 0px;margin: 0px;text-align: left;padding: 2px;">
                <div class="paddingTable">Điện tim, siêu âm</div>
            </td>
            <td style=" border: 1px solid black;width: 4%;padding: 0px;margin: 0px;text-align: center;"> {{DienTim}}
            </td>
            <td style=" border: 1px solid black;width: 6%;padding: 0px;margin: 0px;text-align: center;">
                {{DienTimKhong}} </td>
            <td colspan="2"
                style=" border: 1px solid black;width: 15%;padding: 0px;margin: 0px;font-weight: normal;text-align: left;padding: 2px;">
                <div class="paddingTable"><b>Những lưu ý khác</b></div>
            </td>
        </tr>
        <tr>
            <td colspan="2"
                style=" border: 1px solid black;width: 50%;padding: 0px;margin: 0px;text-align: left;padding: 2px;">
                <div class="paddingTable">Xác nhận thành viên gây mê, phẫu thuật</div>
            </td>
            <td style=" border: 1px solid black;width: 4%;padding: 0px;margin: 0px;text-align: center;">
                {{XacNhanThanhVienGayMePhauThuat}} </td>
            <td style=" border: 1px solid black;width: 6%;padding: 0px;margin: 0px;text-align: center;">
                {{XacNhanThanhVienGayMePhauThuatKhong}} </td>
            <td colspan="2" rowspan="4"
                style=" border: 1px solid black;width: 40%;padding: 0px;margin: 0px;font-weight: normal;text-align: left;padding: 2px;">
                {{NhungLuuYKhac}} </td>
        </tr>
        <tr>
            <td colspan="2"
                style=" border: 1px solid black;width: 50%;padding: 0px;margin: 0px;text-align: left;padding: 2px;">
                <div class="paddingTable">Ký cam kết sử dụng KT cao, VT Theo yêu cầu</div>
            </td>
            <td style=" border: 1px solid black;width: 4%;padding: 0px;margin: 0px;text-align: center;">
                {{KyCamKetSuDungKTCao}} </td>
            <td style=" border: 1px solid black;width: 6%;padding: 0px;margin: 0px;text-align: center;">
                {{KyCamKetSuDungKTCaoKhong}} </td>
        </tr>
        <tr>
            <td colspan="2"
                style=" border: 1px solid black;width: 50%;padding: 0px;margin: 0px;text-align: left;padding: 2px;">
                <div class="paddingTable">Kháng sinh dự phòng</div>
            </td>
            <td style=" border: 1px solid black;width: 4%;padding: 0px;margin: 0px;text-align: center;">
                {{KhangSinhDuPhong}} </td>
            <td style=" border: 1px solid black;width: 6%;padding: 0px;margin: 0px;text-align: center;">
                {{KhangSinhDuPhongKhong}} </td>
        </tr>
        <tr>
            <td colspan="2"
                style=" border: 1px solid black;width: 50%;padding: 0px;margin: 0px;text-align: left;padding: 2px;">
                <div class="paddingTable">Duyệt phẫu thuật</div>
            </td>
            <td style=" border: 1px solid black;width: 4%;padding: 0px;margin: 0px;text-align: center;">
                {{DuyetPhauThuat}} </td>
            <td style=" border: 1px solid black;width: 6%;padding: 0px;margin: 0px;text-align: center;">
                {{DuyetPhauThuatKhong}} </td>
        </tr>
    </tbody>
</table>
<table style="width: 100%;">
    <tr>
        <td> {{GioPhauThuat}} </td>
    </tr>
    <tr>
        <td> {{GioGayMe}} </td>
    </tr>
    <tr>
        <td> Ý kiến của người nhận người bệnh tại phòng GMHS:</td>
    </tr>
    <tr>
        <td> {{YKienNguoiNguoiNhanNguoiBenh}} </td>
    </tr>
</table> <br>
<table style="width: 100%;">
    <tr>
        <td style="width: 33%;text-align:center"><b>CHUẨN BỊ TRƯỚC PT</b></td>
        <td style="width: 33%;text-align:center"><b>NGƯỜI GIAO</b></td>
        <td style="width: 34%;text-align:center"><b>NGƯỜI NHẬN</b></td>
    </tr>
    <tr>
        <td style="width: 33%;text-align:center;font-style:italic">(Ký và ghi rõ họ tên)</td>
        <td style="width: 33%;text-align:center;font-style:italic">(Ký và ghi rõ họ tên)</td>
        <td style="width: 34%;text-align:center;font-style:italic">(Ký và ghi rõ họ tên)</td>
    </tr>
</table>
<table style="width: 100%;padding-top:80px">
    <tr>
        <td style="width: 33%;text-align:center"><b>{{DDChuanBiNBTruocPT}}</b></td>
        <td style="width: 33%;text-align:center"><b>{{DDChuanBiNBDenPhongPT}}</b></td>
        <td style="width: 34%;text-align:center"><b>{{DDNhanBNTaiPhongPTGMHS}}</b></td>
    </tr>
</table>'
where Name='PhieuBangKiemAnToanNguoiBenhPhauThuat'
UPDATE CauHinh
Set [Value] = '4.0.7' where [Name] = 'CauHinhHeThong.DatabaseVesion'