



update Template
set Body=N'<style>
    table,
    th,
    td {
        /* border-collapse: collapse; */
        font-family: Times New Roman;
        font-size: 13px;
        height: 23px;
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
        border-bottom: 1px dotted black;
    }

    .container .values {
        display: table-cell;
        width: 100%;
        height: 100%;
        vertical-align: top;
        position: relative;
        box-sizing: border-box;
        /* border-bottom: 1px dotted black; */
    }

    span.square {
        vertical-align: bottom;
        border: solid 1px;
        width: 25px;
        height: 20px;
        text-align: center;
        margin: 2px;
        display: inline-block
    }

    .format {
        padding: 0px;
    }
</style>
<table width="100%" style="margin-bottom:0;">
    <tr>
        <td colspan="4" style="vertical-align: top;"> BỆNH VIỆN ĐKQT BẮC HÀ <br><b>{{KhoaPhongDangIn}}</b> </td>
        <td style="text-align:center;float:right;"> <img style="height: 40px;"
                src="data:image/png;base64,{{BarCodeImgBase64}}"><br>
            <p style="margin:0;padding:0;">Mã TN: {{MaTN}}</p>
        </td>
    </tr>
</table>
<div>
    <div style="width: 100%;">
        <table width="100%">
            <tbody>
                <tr>
                    <td style="text-align: center;font-size:14px;"> <b>HỒ SƠ CHĂM SÓC ĐIỀU DƯỠNG - HỘ SINH</b></td>
                </tr>
            </tbody>
        </table>
    </div>
</div> <br> <br>
<table style="width:100%">
    <tr>
        <td colspan="3"> {{HoTen}} </td>
        <td> {{NgayThangNam}} </td>
        <td> {{GioiTinh}} </td>
    </tr>
    <tr>
        <td colspan="5"> {{DiaChi}} </td>
    </tr>
    <tr>
        <td colspan="3"> {{NgheNghiep}} </td>
        <td colspan="2"> {{DienThoai}} </td>
    </tr>
    <tr>
        <td colspan="5"> {{HoVaTenNguoiNhaKhiCanBaoTin}} </td>
    </tr>
    <tr>
        <td colspan="5" style="padding-left: 10px;"> {{DienThoaiLienLac}} </td>
    </tr>
    <tr>
        <td colspan="5"> {{NgayVaoVien}} </td>
    </tr>
    <tr>
        <td colspan="5">{{ChanDoanKhiVaoVien}} </td>
    </tr>
    <tr>
        <td colspan="5">{{LyDoVaoVien}} </td>
    </tr>
    <tr>
        <td colspan="5"> {{KhaiThacBenhSu}} </td>
    </tr>
    <tr>
        <td colspan="5" style="padding-left: 10px;"> {{TienSu}} </td>
    </tr>
    <tr>
        <td colspan="5" style="padding-left: 10px;">
            <div class="container">
                <div class="label">+ Tiền sử dị ứng ( Thuốc, côn trùng, thời tiết): &nbsp; &nbsp; Có <span
                        class="square">{{Co}}</span> &nbsp; &nbsp; Không <span class="square">{{Khong}}</span> </div>
            </div>
        </td>
    </tr>
    <tr>
        <td colspan="5" style="padding-left: 10px;"> {{NeuCoKeTen}} </td>
    </tr>
    <tr>
        <td colspan="5" style="padding-left: 10px;">
            <div class="container">
                <div class="label">+ Có hút thuốc <span class="square">{{CoHutThuoc}}</span> &nbsp; &nbsp; &nbsp; &nbsp;
                    &nbsp; &nbsp; Có nghiện rượu bia <span class="square">{{CoNghienRuouBia}}</span> </div>
            </div>
        </td>
    </tr>
    <tr>
        <td colspan="5" style="padding-left: 10px;"> {{NguoiBenhCoKhuyetTat}} </td>
    </tr>
    <tr>
        <td colspan="5">
            <div class="container">
                <div class="label">9. Tình trạng hiện tại của người bệnh:</div>
            </div>
        </td>
    </tr>
    <tr>
        <td colspan="5" style="padding-left: 10px;">
            <div class="container">
                <div class="label">+ Tỉnh táo tiếp xúc tốt <span class="square">{{TinhTaoTiepXuc}}</span> &nbsp; &nbsp;
                    &nbsp; Mê <span class="square">{{Me}}</span> &nbsp; &nbsp; &nbsp; Lơ Mơ &nbsp; <span
                        class="square">{{LoMo}}</span> </div>
            </div>
        </td>
    </tr>
    <tr>
        <td colspan="5" style="padding-left: 10px;"> {{TinhTrangHienTaiCuaNguoiBenh}} </td>
    </tr>
    <tr>
        <td colspan="5" style="padding-top: 10px;">
            <div class="container">
                <div class="label">10. Kế hoạch chăm sóc và theo dõi <b>(chú ý những diễn biến đặc biệt của người bệnh):</b>
                </div>
            </div>
        </td>
    </tr>
    <tr>
        <td colspan="5" style="padding-left: 10px;"> {{KeHoachChamSocVaTheoDoi}} </td>
    </tr>
</table>
<div style="break-after:page"></div>
<table style="width:100%">
    <tr>
        <td colspan="5">
            <div class="container">
                <div class="label">11. Đánh giá tình trạng NB khi ra viện:</div>
            </div>
        </td>
    </tr>
    <tr>
        <td colspan="5" style="padding-left: 10px;">
            <div class="container">
                <div class="label">&#10070; NB khỏi:&nbsp; &nbsp; BS cho về nhà<span
                        class="square">{{BSChoVeNha}}</span> &nbsp; &nbsp; Chuyển viện<span
                        class="square">{{ChuyenVien}}</span> &nbsp; &nbsp; Nặng xin về<span
                        class="square">{{NangXinVe}}</span> </div>
            </div>
        </td>
    </tr>
    <tr>
        <td colspan="5" style="padding-left: 10px;"> {{DanhGiaTinhTrangNBKhiRaVien}} </td>
    </tr>
    <tr>
        <td colspan="5">
            <div class="container">
                <div class="label">12. Hướng dẫn NB những điều cần thiết: (hướng dẫn vệ sinh, ăn uống, luyện tập...)
                </div>
            </div>
        </td>
    </tr>
    <tr>
        <td colspan="5" style="padding-left: 10px;"> {{HuongDanNBNhungDieuCanThiet}} </td>
    </tr>
</table> <br>
<table style="width:100%">
    <tr>
        <td colspan="5">
            <div class="container">
                <div class="label">13. Bàn giao cho NB giấy tờ khi ra viện:</div>
            </div>
        </td>
    </tr>
</table> <br>
<table style="width:100%">
    <tr>
        <td colspan="5" style="padding-left: 10px;">
            <div class="container">
                <div class="label">+ Giấy viện &nbsp; &nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span
                        class="square">{{GiayRaVien}}</span> </div>
            </div>
        </td>
    <tr>
        <td colspan="5" style="padding-left: 10px;">
            <div class="container">
                <div class="label">+ Đơn thuốc &nbsp; &nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <span
                        class="square">{{DonThuoc}}</span> </div>
            </div>
        </td>
    </tr>
    <tr>
        <td colspan="5" style="padding-left: 10px;">
            <div class="container">
                <div class="label">+ Biên lai thanh toán viện phí &nbsp; &nbsp; &nbsp; <span
                        class="square">{{BienLaiThanhToanVienPhi}}</span> </div>
            </div>
        </td>
    </tr>
    <tr>
        <td colspan="5" style="padding-left: 10px;">
            <div class="container">
                <div class="label">+ Giấy tờ theo chuyên khoa: </div>
            </div>
        </td>
    </tr>
    <tr>
        <td colspan="5" style="padding-left: 20px;">
            <div class="container">
                <div class="label">
                    <div> - Giấy chứng nhận phẫu thuật &nbsp; &nbsp;<span
                            class="square">{{GiayChungNhanPhauThuat}}</span></div>
                    <div> - Giấy chứng sinh &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span
                            class="square">{{GiayChungSinh}}</span></div>
                </div>
            </div>
        </td>
    </tr>
    </tr>
</table>
<table style="width:100%">
    <tr>
        <td colspan="5">
            <div class="container">
                <div class="label">14. Hẹn đến khám lại:</div>
            </div>
        </td>
    </tr>
    <tr>
        <td colspan="5" style="padding-left: 10px;"> {{HenDenKhamLai}} </td>
    </tr>
</table> <br> <br>
<table style="width:100%">
    <tr>
        <th colspan="4" style="width:30%; font-size: 13px;"></th>
        <th colspan="4" style="font-size: 13px;font-weight: normal;"><i>Bắc Hà, ngày {{NgayHienTai}} tháng {{ThangHienTai}} năm
                {{NamHienTai}}</i> </th>
    </tr>
    <tr>
        <td colspan="4" style="text-align: center;font-size: 13px;;text-transform: uppercase;"><b>điều dưỡng trưởng
                khoa</b></td>
        <td colspan="4" style="text-align: center;font-size: 13px;;text-transform: uppercase"><b>Điều dưỡng chăm sóc
                người bệnh</b> </td>
    </tr>
    <tr>
        <td colspan="4" style="text-align: center;font-size: 13px;;font-style: italic;">(Ký và ghi rõ họ tên)</td>
        <td colspan="4" style="text-align: center;font-size: 13px;;font-style: italic">(Ký và ghi rõ họ tên)</td>
    </tr>
    <tr>
        <td colspan="4" style="text-align: center;font-size: 13px;"></td>
        <td colspan="4" style="text-align: center;font-size: 13px;"></td>
    </tr>
    <tr>
        <td colspan="4" style="text-align: center;font-size: 13px;"></td>
        <td colspan="4" style="text-align: center;font-size: 13px;"></td>
    </tr>
    <tr>
        <td colspan="4" style="text-align: center;font-size: 13px;"></td>
        <td colspan="4" style="text-align: center;font-size: 13px;"></td>
    </tr>
    <tr>
        <td colspan="4" style="text-align: center;font-size: 13px;font-weight: bold;">{{DieuDuongTruongKhoa}}</td>
        <td colspan="4" style="text-align: center;font-size: 13px;font-weight: bold;">{{DieuDuongChamSocNguoiBenh}}</td>
    </tr>
</table>'

where Name=N'HoSoChamSocDieuDuong'

Update dbo.CauHinh
Set [Value] = '3.4.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'