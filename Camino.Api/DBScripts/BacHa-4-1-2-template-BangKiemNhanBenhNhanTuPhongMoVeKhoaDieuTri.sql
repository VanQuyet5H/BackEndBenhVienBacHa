update template
set Body=N'<style>
    table,
    th,
    td {
        border-collapse: collapse;
        font-family: Times New Roman;
        font-size: 13px;
        height: 20px;
    }

    th,
    td {
        padding: 0px;
    }

    span.square {
        vertical-align: bottom;
        border: solid 1px;
        width: 18px;
        height: 18px;
        font-weight: normal;
        margin: 2px;
        display: inline-block
    }
</style>
<div>
    <div style="width: 100%;">
        <table width="100%" style="margin-bottom:0;">
            <tr>
                <td colspan="4" style="vertical-align: top;font-size: 14px;width: 30%;"> BỆNH VIỆN ĐKQT BẮC HÀ
                    <br><b>{{Khoa}}</b> </td>
                <td style="font-size: 17px;font-weight: bold;text-align: center;width: 50%;"> BẢNG KIỂM BÀN GIAO VÀ NHẬN
                    NGƯỜI BỆNH SAU PHẪU THUẬT</td>
                <td style="text-align:center;float:right;"> <img style="height: 40px;"
                        src="data:image/png;base64,{{BarCodeImgBase64}}"><br>
                    <p style="margin:0;padding:0;font-size: 12px;">Mã TN: {{MaTN}}</p>
                </td>
            </tr>
        </table>
    </div>
</div> <br>
<table style="width:100%; padding-top: 35px;">
    <tr>
        <td colspan="2" style="border: 1px solid black;width: 50%;text-align: center;"><b>TOÀN TRẠNG</b></th>
        <td colspan="3" style="border: 1px solid black;width: 50%;text-align: center;"><b>HỒ SƠ BỆNH ÁN</b></th>
    </tr>
    <tbody>
        <tr>
            <td style="border: 1px solid black;width: 23%;">&nbsp;&nbsp;Kiểm tra trước khi lên PM</b></td>
            <td style="border: 1px solid black;width: 23%;text-align: center;"> Có <span
                    class="square">{{KiemTraTruocKhiLenPMCo}}</span> &nbsp; &nbsp; Không <span
                    class="square">{{KiemTraTruocKhiLenPMKhong}}</span> </td>
            <td colspan="2" style="border: 1px solid black;width: 23%;text-align: center;">BA Ngoại,Sản khoa hoàn
                thiện</b></td>
            <td style="border: 1px solid black;width: 23%;text-align: center;"> Có <span
                    class="square">{{BANgoaiKhoaHoanThienCo}}</span> &nbsp;Không <span
                    class="square">{{BANgoaiKhoaHoanThienKhong}}</span> </td>
        </tr>
        <tr>
            <td style="border: 1px solid black;width: 23%;">&nbsp;&nbsp;Kiểm tra trước khi rạch da</b></td>
            <td style="border: 1px solid black;width: 23%;text-align: center;"> Có <span
                    class="square">{{KiemTraTruocKhiRachDaCo}}</span> &nbsp; &nbsp; Không <span
                    class="square">{{KiemTraTruocKhiRachDaKhong}}</span> </td>
            <td rowspan="2"
                style="border: 1px solid black;border-top: none;width: 9%;text-align: center;font-weight: bold;"> Phẫu
                <br>thuật viên &nbsp;&nbsp;</td>
            <td colspan="2" style="border: 1px solid black;border-top: none;width: 20%;">&nbsp;&nbsp; Họ tên PTV:
                {{HoTenPTV}} </td>
        </tr>
        <tr>
            <td style="border: 1px solid black;width: 23%;">&nbsp;&nbsp;Tỉnh táo</b></td>
            <td style="border: 1px solid black;width: 23%;text-align: center;"> Có <span
                    class="square">{{TinhTaoCo}}</span> &nbsp; &nbsp; Không <span class="square">{{TinhTaoKhong}}</span>
            </td>
            <td style="border: 1px solid black;border-top: none;width: 20%;">&nbsp;&nbsp; Nơi công tác: {{NoiCongTac}}
            </td>
            <td style="border: 1px solid black;border-top: none;width: 20%;text-align: center;"> BV ĐKQT BẮC HÀ &nbsp;
                <span class="square">{{BVDKQTBacHa}}</span> <br> Hợp tác <span style="padding-left: 68px;"><span
                        class="square">{{HopTac}}</span></span> </td>
        </tr>
        <tr>
            <td style="border: 1px solid black;border-top: none;width: 23%;">&nbsp;&nbsp; Kích thích vật vã </td>
            <td style="border: 1px solid black;border-top: none;width: 23%;text-align: center;"> Có <span
                    class="square">{{KichThichVatVaCo}}</span> &nbsp; &nbsp; Không <span
                    class="square">{{KichThichVatVaKhong}}</span> </td>
            <td colspan="2" style="border: 1px solid black;border-top: none;width: 20%;text-align: center;"
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;> Chế độ ăn uống </td>
            <td style="border: 1px solid black;border-top: none;width: 20%;">&nbsp;&nbsp;{{CheDoAnUong}} </td>
        </tr>
        <tr>
            <td style="border: 1px solid black;border-top: none;width: 23%;">&nbsp;&nbsp; DH Sinh tồn ổn định </td>
            <td style="border: 1px solid black;border-top: none;width: 23%;text-align: center;"> Có <span
                    class="square">{{DHSinhTonOnDinhCo}}</span> &nbsp; &nbsp; Không <span
                    class="square">{{DHSinhTonOnDinhKhong}}</span> </td>
            <td rowspan="5"
                style="border: 1px solid black;border-top: none;width: 7%;text-align: center;font-weight: bold;">
                Cận<br>lâm <br> sàng </td>
            <td style="border: 1px solid black;border-top: none;width: 20%;">&nbsp;&nbsp; Giải phẫu bệnh </td>
            <td style="border: 1px solid black;border-top: none;width: 20%;text-align: center;"> Có <span
                    class="square">{{GiaiPhauBenhCo}}</span> &nbsp; &nbsp; Không <span
                    class="square">{{GiaiPhauBenhKhong}}</span> </td>
        </tr>
        <tr>
            <td style="border: 1px solid black;border-top: none;width: 23%;">&nbsp;&nbsp; Nghi ngờ chảy máu </td>
            <td style="border: 1px solid black;border-top: none;width: 23%;text-align: center;"> Có <span
                    class="square">{{NghiNgoChayMauCo}}</span> &nbsp; &nbsp; Không <span
                    class="square">{{NghiNgoChayMauKhong}}</span> </td>
            <td style="border: 1px solid black;border-top: none;width: 20%;"> &nbsp;&nbsp;Số mẫu bệnh phẩm: </td>
            <td style="border: 1px solid black;border-top: none;width: 20%;">&nbsp;&nbsp;{{SoMauBenh}} </td>
        </tr>
        <tr>
            <td style="border: 1px solid black;border-top: none;width: 23%;">&nbsp;&nbsp; Suy hô hấp </td>
            <td style="border: 1px solid black;border-top: none;width: 23%;text-align: center;"> Có <span
                    class="square">{{SuyHoHapCo}}</span> &nbsp; &nbsp; Không <span
                    class="square">{{SuyHoHapKhong}}</span> </td>
            <td style="border: 1px solid black;border-top: none;width: 20%;">&nbsp;&nbsp; Kết quả film (XQ, MSCT, MRI..)
            </td>
            <td style="border: 1px solid black;border-top: none;width: 20%;">&nbsp;&nbsp;{{KetQuaFilm}} </td>
        </tr>
        <tr>
            <td style="border: 1px solid black;border-top: none;width: 23%;">&nbsp;&nbsp; Nôn, nấc </td>
            <td style="border: 1px solid black;border-top: none;width: 23%;text-align: center;"> Có <span
                    class="square">{{NonNacCo}}</span> &nbsp; &nbsp; Không <span class="square">{{NonNacKhong}}</span>
            </td>
            <td colspan="2" style="border: 1px solid black;border-top: none;width: 20%;"> &nbsp;&nbsp;Xét nghiệm cần
                làm: {{XeNghiemCanLam}} </td>
        </tr>
        <tr>
            <td style="border: 1px solid black;border-top: none;width: 23%;">&nbsp;&nbsp; Cầu bàng quang </td>
            <td style="border: 1px solid black;border-top: none;width: 23%;text-align: center;"> Có <span
                    class="square">{{CauBangQuangCo}}</span> &nbsp; &nbsp; Không <span
                    class="square">{{CauBangQuangKhong}}</span> </td>
            <td style="border: 1px solid black;border-top: none;width: 20%;">&nbsp;&nbsp; Đã ký chọn phòng </td>
            <td style="border: 1px solid black;border-top: none;width: 20%;text-align: center;"> Thường <span
                    class="square">{{Thuong}}</span> &nbsp; &nbsp; VIP <span class="square">{{Vip}}</span> </td>
        </tr>
        <tr>
            <td style="border: 1px solid black;border-top: none;width: 23%;">&nbsp;&nbsp; Vân tím trên da </td>
            <td style="border: 1px solid black;border-top: none;width: 23%;text-align: center;"> Có <span
                    class="square">{{VanTimTrenDaCo}}</span> &nbsp; &nbsp; Không <span
                    class="square">{{VanTimTrenDaKhong}}</span> </td>
            <td rowspan="6"
                style="border: 1px solid black;border-top: none;width: 5%;text-align: center;font-weight: bold;">
                Chăm<br>sóc </td>
            <td rowspan="3" style="border: 1px solid black;border-top: none;width: 23%;"> &nbsp;&nbsp;Chế độ chăm sóc
            </td>
            <td rowspan="3" style="border: 1px solid black;border-top: none;width: 23%;text-align: center;"> <span
                    style="margin-left: 8px;">Cấp I</span> <span style="padding-left: 17px;"><span
                        class="square">{{CapMot}}</span> <br><span style="margin-left: 8px;">Cấp II</span> &nbsp;<span
                        style="padding-left: 10px;"><span class="square">{{CapHai}}</span> <br><span
                            style="margin-left: 8px;">Cấp III</span> <span style="padding-left: 9px;"><span
                                class="square">{{CapBa}}</span> </td>
        </tr>
        <tr>
            <td style="border: 1px solid black;border-top: none;width: 23%;">&nbsp;&nbsp; Đau nhiều </td>
            <td style="border: 1px solid black;border-top: none;width: 23%;text-align: center;"> Có <span
                    class="square">{{DauNhieuCo}}</span> &nbsp; &nbsp; Không <span
                    class="square">{{DauNhieuKhong}}</span> </td>
        </tr>
        <tr>
            <td style="border: 1px solid black;border-top: none;width: 23%;"> <b>&nbsp;&nbsp; TRUYỀN DỊCH</b> </td>
            <td style="border: 1px solid black;border-top: none;width: 23%;text-align: center;"> Có <span
                    class="square">{{TruyenDichCo}}</span> &nbsp; &nbsp; Không <span
                    class="square">{{TruyenDichKhong}}</span> </td>
        </tr>
        <tr>
            <td style="border: 1px solid black;border-top: none;width: 23%;">&nbsp;&nbsp; Vị trí </td>
            <td style="border: 1px solid black;border-top: none;width: 23%;">&nbsp;&nbsp; {{ViTri}} </td>
            <td rowspan="3" colspan="2" style="border: 1px solid black;border-top: none;width: 20%;">&nbsp;&nbsp; Chỉ
                định theo dõi :&nbsp;{{ChiDinhTheoDoi}} </td>
        </tr>
        <tr>
            <td style="border: 1px solid black;border-top: none;width: 23%;">&nbsp;&nbsp; Lưu thông </td>
            <td style="border: 1px solid black;border-top: none;width: 23%;text-align: center;"> Có <span
                    class="square">{{LuuThongCo}}</span> &nbsp; &nbsp; Không <span
                    class="square">{{LuuThongKhong}}</span> </td>
        </tr>
        <tr>
            <td style="border: 1px solid black;border-top: none;width: 23%;"> <b>&nbsp;&nbsp;ỐNG THÔNG DẠ DÀY</b> </td>
            <td style="border: 1px solid black;border-top: none;width: 23%;text-align: center;"> Có <span
                    class="square">{{OngThongDaDayCo}}</span> &nbsp; &nbsp; Không <span
                    class="square">{{OngThongDaDayKhong}}</span> </td>
        </tr>
        <tr>
            <td style="border: 1px solid black;border-top: none;width: 23%;">&nbsp;&nbsp; Số lượng <br>&nbsp;&nbsp; Màu
                sắc </td>
            <td style="border: 1px solid black;border-top: none;width: 23%;">&nbsp;&nbsp;{{SoLuongMauSacDaDay}}
                <br>&nbsp;&nbsp; {{MauSacDaDay}} </td>
            <td rowspan="20"
                style="border: 1px solid black;border-top: none;width: 5%;text-align: center;font-weight: bold;">Thuốc
                <br>theo y <br>lệnh và <br>giấy tờ <br>bàn giao </td>
            <td rowspan="3" colspan="2" style="border: 1px solid black;border-top: none;width: 40%;vertical-align:top;">
                &nbsp;&nbsp;&nbsp;Thuốc đang dùng: <span style="margin-left: 40px;text-align: center;">Có</span> <span
                    class="square" style="text-align: center;">{{ThuocDangDungCo}}</span> &nbsp; &nbsp;&nbsp;
                &nbsp;&nbsp; &nbsp; Không <span class="square" style="text-align: center;">{{ThuocDangDungKhong}}</span>
        </tr>
        <tr>
            <td style="border: 1px solid black;border-top: none;width: 23%;"> <b>&nbsp;&nbsp;ỐNG THÔNG TIỂU</b> </td>
            <td style="border: 1px solid black;border-top: none;width: 23%;text-align: center;"> Có <span
                    class="square">{{OngThongTieuCo}}</span> &nbsp; &nbsp; Không <span
                    class="square">{{OngThongTieuKhong}}</span> </td>
            </td>
        </tr>
        <tr>
            <td style="border: 1px solid black;border-top: none;width: 23%;">&nbsp;&nbsp; Số lượng <br>&nbsp;&nbsp; Màu
                sắc </td>
            <td style="border: 1px solid black;border-top: none;width: 23%;">
                &nbsp;&nbsp;{{SoLuongMauSacOngThongTieu}}<br>&nbsp;&nbsp;{{MauSacOngThongTieu}} </td>
        </tr>
        <tr>
            <td style="border: 1px solid black;border-top: none;width: 23%;"> <b>&nbsp;&nbsp;DẪN LƯU</b> </td>
            <td style="border: 1px solid black;border-top: none;width: 23%;text-align: center;"> Có <span
                    class="square">{{DanLuuCo}}</span> &nbsp; &nbsp; Không <span class="square">{{DanLuuKhong}}</span>
            </td>
            <td rowspan="3" colspan="2" style="border: 1px solid black;border-top: none;width: 40%;vertical-align:top;">
                &nbsp;&nbsp; Thuốc bàn giao: <span style="margin-left: 50px;">Có</span><span class="square"
                    style="text-align: center;">{{ThuocBanGiaoCo}}</span> &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;Không
                <span class="square" style="text-align: center;">{{ThuocBanGiaoKhong}}</span> </td>
        </tr>
        <tr>
            <td style="border: 1px solid black;border-top: none;width: 23%;"> &nbsp;&nbsp;Vị trí </td>
            <td style="border: 1px solid black;border-top: none;width: 23%;"> &nbsp;&nbsp;{{ViTriDanLuu}}</td>
        </tr>
        <tr>
            <td style="border: 1px solid black;border-top: none;width: 23%;"> &nbsp;&nbsp;Số lượng, màu sắc dịch </td>
            <td style="border: 1px solid black;border-top: none;width: 23%;"> &nbsp;&nbsp;{{SoLuongMauSacDich}}
                <br>&nbsp;&nbsp;{{MauSacDich}} </td>
        </tr>
        <tr>
            <td style="border: 1px solid black;border-top: none;width: 23%;">&nbsp;&nbsp; Áp lực </td>
            <td style="border: 1px solid black;border-top: none;width: 23%;text-align: center;"> Có <span
                    class="square">{{ApLucCo}}</span> &nbsp; &nbsp; Không <span class="square">{{ApLucKhong}}</span>
            </td>
            <td colspan="2" style="border: 1px solid black;border-top: none;border-bottom: none;width: 23%;">
                <div style="width:100%"><b>&nbsp;Giấy tờ bàn giao:</b> <span
                        style="margin-left: 71px;text-align: center;">Có</span> <span
                        style="margin-left: 50px;">Không</span></div>
            </td>
        </tr>
        <tr>
            <td rowspan="3" style="border: 1px solid black;border-top: none;width: 23%;">&nbsp;&nbsp; <b>VẾT MỔ</b>
            </td>
        </tr>
        <tr>
            <td style="border: 1px solid black;border-top: none;width: 23%;">&nbsp;&nbsp;Băng khô: {{BangKho}} </td>
            <td rowspan="4" colspan="2" style="border: 1px solid black;border-top: none;width: 23%;">
                <div style="width:100%"><span style="width:50%;">&nbsp;- Bảng theo dõi GMHS:</span> <span class="square"
                        style="margin-left: 42px;text-align: center;">{{BangTheoDoiGMHSCo}}</span> <span class="square"
                        style="margin-left: 42px;text-align: center;">{{BangTheoDoiGMHSKhong}}</span> </div>
                <div style="width:100%"><span style="width:50%;">&nbsp;- Bảng theo dõi hồi tỉnh:</span><span
                        class="square"
                        style="margin-left: 43px;text-align: center;">{{BangTheoDoiHoiTinhCo}}</span>&nbsp;&nbsp; &nbsp;
                    &nbsp;&nbsp; &nbsp;&nbsp; &nbsp; <span class="square"
                        style="margin-left: 3px;text-align: center;">{{BangTheoDoiHoiTinhKhong}}</span> </div>
                <div style="width:100%"><span style="width:50%;">&nbsp;- Phiếu đếm gạc: </span><span class="square"
                        style="margin-left: 80px;text-align: center;">{{PhieuDemGacCo}}</span> <span class="square"
                        style="margin-left: 42px;text-align: center;">{{PhieuDemGacKhong}}</span> </div>
                <div style="width:100%"><span style="width:50%;">&nbsp;- Phiếu theo dõi tê NMC:</span><span
                        class="square" style="margin-left: 39px;text-align: center;">{{PhieuTheoDoiTeNMCCo}}</span>
                    <span class="square" style="margin-left: 42px;text-align: center;">{{PhieuTheoDoiTeNMCKhong}}</span>
                </div>
                <div style="width:100%"><span style="width:50%">&nbsp;- Phiếu VT ngoài:</span> <span class="square"
                        style="margin-left: 75px;text-align: center;">{{PhieuVTNgoaiCo}}</span> <span class="square"
                        style="margin-left: 42px;text-align: center;">{{PhieuVTNgoaiKhong}}</span> </div>
                <div style="width:100%"><span style="width:50%;">&nbsp;- Phiếu VTTH GM&PT:</span> <span class="square"
                        style="margin-left: 41px;text-align: center;">{{PhieuVTTHGMPTCo}}</span>&nbsp;&nbsp; &nbsp;
                    &nbsp;&nbsp; &nbsp;&nbsp; &nbsp; <span class="square"
                        style="margin-left: 3px;text-align: center;">{{PhieuVTTHGMPTKhong}}</span> </div>
                <div style="width:100%"><span style="width:50%;">&nbsp;- Số lượng Film:</span> <span class="square"
                        style="margin-left: 82px;text-align: center;">{{SoLuongFilmCo}}</span> <span class="square"
                        style="margin-left: 42px;text-align: center;">{{SoLuongFilmKhong}}</span> </div>
                <div style="width:100%"><span style="width:50%;">&nbsp;- Quần áo/váy:{{QuanAo}}</span> </div>
            </td>
        </tr>
        <tr>
            <td style="border: 1px solid black;border-top: none;width: 23%;"> &nbsp;&nbsp;Thấm dịch: {{ThamDich}} </td>
        </tr>
        <tr>
            <td style="border: 1px solid black;border-top: none;width: 50%;text-align: center;" colspan="2">
                &nbsp;&nbsp;<b>TƯ TRANG NB </b> &nbsp; &nbsp; &nbsp; &nbsp;Có <span
                    class="square">{{TuTrangBNCo}}</span> &nbsp; &nbsp; Không <span
                    class="square">{{TuTrangBNKhong}}</span> </td>
            </td>
        </tr>
    </tbody>
</table> <br>
<table style="width:100%">
    <tr>
        <th style="width:50%;border: none;"><b></b> </th>
        <td style="width:50%;border: none;text-align: center;"><i>Ngày {{NgayHienTai}} tháng {{ThangHienTai}} năm
                {{NamHienTai}}</i></td>
    </tr>
    <tr>
        <th style="width:50%;border: none;font-size:14px;text-transform: uppercase;"><b>Người giao</b> </th>
        <td style="width:50%;border: none;text-align: center;font-size:14px;text-transform: uppercase;"><b>Người
                nhận</b></td>
    </tr>
    <tr>
        <td style="text-align: center;font-style: italic;">(Ký và ghi rõ họ tên)</td>
        <td style="text-align: center;font-style: italic;">(Ký và ghi rõ họ tên)</td>
    </tr>
    <tr>
        <td style="text-align: center;font-size: 18px"></td>
        <td style="text-align: center;font-size: 18px"> </td>
    </tr>
    <tr>
        <td style="text-align: center;font-size: 18px"></td>
        <td style="text-align: center;font-size: 18px"> </td>
    </tr>
    <tr>
        <td style="text-align: center;font-size: 18px"></td>
        <td style="text-align: center;font-size: 18px"> </td>
    </tr>
    <tr>
        <td style="text-align: center;font-size: 18px"></td>
        <td style="text-align: center;font-size: 18px"> </td>
    </tr>
    <tr>
        <td style="text-align: center;font-size: 18px"><b>{{NguoiGiao}}</b></td>
        <td style="text-align: center;font-size: 18px"><b>{{NguoiNhan}}</b> </td>
    </tr>
</table>'
where Name=N'BangKiemNhanBenhNhanTuPhongMoVeKhoaDieuTri'
UPDATE CauHinh
Set [Value] = '4.1.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'