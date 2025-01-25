select * from Template where Name='BangKiemAnToanPhauThuat' 

update Template
set Body=N'<style>
    table,
    th,
    td {
        border-collapse: collapse;
        font-family: Times New Roman;
        font-size: 14px;
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
    }

    span.square {
        vertical-align: bottom;
        border: solid 1px;
        width: 25px;
        height: 20px;
        font-weight: normal;
        margin: 2px;
        display: inline-block
    }

    .coKhong {
        border: 1px solid black;
        text-align: center;
        width: 5%;
        height: 40px;
        word-break: break-word;
        font-size: 12px;
    }

    .text {
        border: 1px solid black;
        width: 23%;
        height: 40px;
        word-break: break-word;
        font-size: 12px;
    }

    .suyRa {
        position: absolute;
        left: 348px;
        top: 97px;
        z-index: -1;
    }

    .suyRa2 {
        position: absolute;
        left: 317px;
        top: 97px;
        z-index: -1;
    }

    .suyRa1 {
        position: absolute;
        left: 721px;
        top: 97px;
        z-index: -1;
    }

    .suyRa3 {
        position: absolute;
        left: 660px;
        top: 97px;
        z-index: -1;
    }

    .paddingTopNone {
        padding: 5px;
        padding-top: 0px;
        padding-bottom: 0px;
    }
</style>
<div>
    <div style="width: 100%;">
        <table width="100%" style="margin-bottom:0;">
            <tr>
                <td colspan="4" style="vertical-align: top;font-size: 14px;width: 30%;"> BỆNH VIỆN ĐA KHOA QUỐC TÊ BẮC
                    HÀ <br><b>{{Khoa}}</b> </td>
                <td style="font-size: 14px;font-weight: bold;text-align: center;width: 50%;"> BẢNG KIỂM AN TOÀN PHẪU
                    THUẬT </td>
                <td style="text-align:center;float:right;"> <img style="height: 40px;"
                        src="data:image/png;base64,{{BarCodeImgBase64}}"><br>
                    <p style="margin:0;padding:0;font-size: 14px;">Mã TN: {{MaTN}}</p>
                </td>
            </tr>
        </table>
    </div>
</div>
<table style="width: 100%;font-size: 14px;">
    <tr>
        <td style="width: 50%">
            <div class="container">
                <div class="label">Họ và tên người bệnh:</div>
                <div class="values">{{HoTenNguoiBenh}}</div>
            </div>
        </td>
        <td style="width: 35%;">
            <div class="container">
                <div class="label">Ngày/tháng/năm sinh:</div> {{NgayThangNamSinh}}
            </div>
        </td>
        <td style="width: 15%">
            <div class="container">
                <div class="label">Giới tính:</div> {{GioiTinh}}
            </div>
        </td>
    </tr>
    <tr>
        <td style="width: 50%;">
            <div class="container">
                <div class="label">Chuẩn đoán trước mổ:</div> {{ChuanDoanTruocMo}}
            </div>
        </td>
        <td style="width: 35%">
            <div class="container">
                <div class="container">
                    <div class="label">Ngày mổ:</div> {{NgayThangNam}}
                </div>
            </div>
        </td>
        <td style="width: 15%;">
            <div class="container">
                <div class="container">
                    <div class="label">Phòng mổ:</div> {{PhongMo}}
                </div>
            </div>
        </td>
    </tr>
    <tr>
        <td colstyle="width: 50%;">
            <div class="container">
                <div class="container">
                    <div class="label">Phương pháp phẫu thuật:</div> {{PhuongPhapPhauThuat}}
                </div>
            </div>
        </td>
        <td colspan="2" style="width: 50%;">
            <div class="container">
                <div class="container">
                    <div class="label">Phương pháp vô cảm:</div> {{PhuongThucVoCam}}
                </div>
            </div>
        </td>
    </tr>
</table>
<table style="width: 100%;margin-top: 4px;">
    <tr>
        <td colspan="3" style="border: 1px solid black;text-align: center;width:33%;height: 30px;"> <b>TRƯỚC KHI GÂY MÊ/
                GÂY TÊ</b> <span class="suyRa"><img src="{{Vector}}" style="height: 70px;width: 70px;"
                    alt="blog-cong-dong" /></span> </td>
        <td colspan="3" style="border: 1px solid black;text-align: center;height: 30px;"> <b>TRƯỚC KHI RẠCH DA</b><span
                class="suyRa1"><img src="{{Vector}}" style="height: 70px;width: 70px;" alt="blog-cong-dong" /></span>
        </td>
        <td colspan="3" style="border: 1px solid black;text-align: center;height: 30px;"> <b>TRƯỚC KHI KẾT THÚC MỔ</b>
        </td>
    </tr>
    <tr>
        <td class="text" style="font-style: italic;">
            <div class="paddingTopNone"> Bác sỹ,Điều dưỡng, KTV gây mê</div>
        </td>
        <td class="coKhong"><b>Có</b></td>
        <td class="coKhong"><b>Không</b></td>
        <td class="text" style="font-style: italic;">
            <div class="paddingTopNone"> Phẫu thuật viên, BS gây mê, BS sơ sinh, Điều dưỡng dụng cụ,Điều dưỡng gây mê
            </div>
        </td>
        <td class="coKhong"><b>Có</b></td>
        <td class="coKhong"><b>Không</b></td>
        <td class="text" style="font-style: italic;">
            <div class="paddingTopNone"> Phẫu thuật viên, BS gây mê, BS sơ sinh, Điều dưỡng dụng cụ,Điều dưỡng gây mê
            </div>
        </td>
        <td class="coKhong"><b>Có</b></td>
        <td class="coKhong"><b>Không</b></td>
    </tr>
    <tr>
        <td class="text">
            <div class="paddingTopNone"> <b>1. Xác nhận họ tên, tuổi,giới và mã người bệnh?</b></div>
        </td>
        <td class="coKhong"> {{BsCheckHoTenCo}} </td>
        <td class="coKhong"> {{BsCheckHoTenKhong}} </td>
        <td class="text">
            <div class="paddingTopNone"><b>1. Các thành viên trong kíp giới thiệu tên và nhiệm vụ?</b></div>
        </td>
        <td class="coKhong">{{CheckGioiThieuEkipCo}}</td>
        <td class="coKhong">{{CheckGioiThieuEkipKhong}}</td>
        <td class="text">
            <div class="paddingTopNone"><b>1. Điều dưỡng xác nhận hoàn thành đếm đủ kim, gạc,dụng cụ phẫu thuật?</b>
            </div>
        </td>
        <td class="coKhong">{{DdDemDungCuCCo}}</td>
        <td class="coKhong">{{DdDemDungCuKhong}}</td>
    </tr>
    <tr>
        <td class="text"> <b>
                <div class="paddingTopNone">2. Xác nhận vị trí, phương pháp mổ, cam kết mổ?</div>
            </b> </td>
        <td class="coKhong">{{XnMoCo}}</td>
        <td class="coKhong">{{XnMoKhong}}</td>
        <td class="text"> <b>
                <div class="paddingTopNone">2. Xác nhận lại họ tên, tuổi,giới và mã người bệnh,phương pháp phẫu thuật?
                </div>
            </b>
        <td class="coKhong">{{XNLaiThongTinNguoibenhCo}}</td>
        <td class="coKhong">{{XNLaiThongTinNguoibenhKhong}}</td>
        <td class="text" colspan="3"> <b>
                <div class="paddingTopNone">2. Trước khi chuyển NB về phòng hồi tỉnh:</div>
            </b> </td>
    </tr>
    <tr>
        <td class="text" colspan="3">
            <div class="paddingTopNone"><b>3. Vùng mổ có được đánh dấu không?</b> <br> {{DanhDauVungMoText}}</div>
        </td>
        <td class="text" colspan="3">
            <div class="paddingTopNone"><b>3. Kháng sinh dự phòng có được sử dụng không?</b> <br>
                {{KhangSinhDuPhongbSuDungDcKhongText}}</div>
        </td>
        <td class="text" colspan="3">
            <div class="paddingTopNone"><span style="font-style: italic;font-weight: bold;">Dán nhãn bệnh phẩm:</span>
                <br> {{DanNhanBenhPhamText}} </div>
        </td>
    </tr>
    <tr>
        <td class="text"> <b>
                <div class="paddingTopNone">4. Thuốc và thiết bị gây mê có đầy đủ không?</div>
            </b> <br> </td>
        <td class="coKhong">{{ThuocvaThietBiGayMeCoDayDuKhongCo}}</td>
        <td class="coKhong">{{ThuocvaThietBiGayMeCoDayDuKhongKhong}}</td>
        <td class="text"> <b>
                <div class="paddingTopNone">4. Phẫu thuật viên:<span style="font-style: italic;font-weight: bold;">Xác
                        nhận lại vị trí rạch da, đối chiếu hình ảnh chẩn đoán?</span></div>
            </b> <br> </td>
        <td class="coKhong">{{XacNhanLiVTRachDaCo}}</td>
        <td class="coKhong">{{XacNhanLiVTRachDaKhong}}</td>
        <td class="text"> <span style="font-style: italic;font-weight: bold;">
                <div class="paddingTopNone">Đảm bảo an toàn và vô khuẩn các hệ thống dẫn lưu, sonde tiểu?</div>
            </span> <br> </td>
        <td class="coKhong">{{DamBaoAnToanVoKhuanCo}}</td>
        <td class="coKhong">{{DamBaoAnToanVoKhuanKhong}}</td>
    </tr>
    <tr>
        <td class="text"> <b>
                <div class="paddingTopNone">5. Monitor có hoạt động bình thường không?</div>
            </b> <br> </td>
        <td class="coKhong">{{XMonitorCo}}</td>
        <td class="coKhong">{{XMonitorKhong}}</td>
        <td class="text"> <span style="font-style: italic;font-weight: bold;">
                <div class="paddingTopNone">Xác nhận điều cần chú ý trong phẫu thuật?</div>
            </span> <br> </td>
        <td class="coKhong">{{XacNhanDieuCanChuYCo}}</td>
        <td class="coKhong">{{XacNhanDieuCanChuYKhong}}</td>
        <td class="text"> <b>
                <div class="paddingTopNone">3. Xác nhận những điều cần chú ý về hồi tỉnh và chăm sóc sau mổ?</div>
            </b> <br> </td>
        <td class="coKhong">{{XacNhanNhungDieuCanChuYVeHoiTinhChamSocSauMoCo}}</td>
        <td class="coKhong">{{XacNhanNhungDieuCanChuYVeHoiTinhChamSocSauMoKhong}}</td>
    </tr>
    <tr>
        <td class="text"> <b>
                <div class="paddingTopNone">6. Người bệnh có tiền sử dị ứng không?</div>
            </b> <br> </td>
        <td class="coKhong">{{NguoiiBenhTienSuBenhCo}}</td>
        <td class="coKhong">{{NguoiiBenhTienSuBenhKhong}}</td>
        <td class="text"> <b>
                <div class="paddingTopNone">5. BS gây mê: <span style="font-style: italic;font-weight: bold;">có điều gì
                        cần chú ý trong gây mê không?</span></div>
            </b> <br> </td>
        <td class="coKhong">{{BSGayMeCoDieuGiChuYCo}}</td>
        <td class="coKhong">{{BSGayMeCoDieuGiChuYKhong}}</td>
        <td class="text" colspan="3" rowspan="4" style="vertical-align: top;">
            <div class="paddingTopNone"><b>Chú thích: </b>{{ChuThich}}</div> <br>
        </td>
    </tr>
    <tr>
        <td class="text"> <b>
                <div class="paddingTopNone"> 7. Người bệnh có nguy cơ khó thở/nguy cơ trào ngược không?</div>
            </b> <br> </td>
        <td class="coKhong">{{NguoiiBenhCoNguyCoCo}}</td>
        <td class="coKhong">{{NguoiiBenhCoNguyCoKhong}}</td>
        <td class="text"> <b>
                <div class="paddingTopNone">6. Điều dưỡng dụng cụ:<span
                        style="font-style: italic;font-weight: bold;">Xác nhận có điều gì cần chú ý về dụng cụ, TTB
                        không?</span></div>
            </b> <br> </td>
        <td class="coKhong">{{DieuDuongDungCuCo}}</td>
        <td class="coKhong">{{DieuDuongDungCuKhong}}</td>
    </tr>
    <tr>
        <td class="text" rowspan="2"> <b>
                <div class="paddingTopNone"> 8. Người bệnh có nguy cơ mất máu >500ml (7ml/kg ở trẻ em) không?</div>
            </b> </td>
        <td class="coKhong" rowspan="2">{{NguyCoMatMauCo}}</td>
        <td class="coKhong" rowspan="2">{{NguyCoMatMauKhong}}</td>
        <td class="text"> <b>
                <div class="paddingTopNone"><span style="font-style: italic;font-weight: bold;">Thực hiện đếm kim,
                        gạc,dụng cụ?</span></div>
            </b> <br> </td>
        <td class="coKhong">{{ThucHienDemKimGacDungCuCo}}</td>
        <td class="coKhong">{{ThucHienDemKimGacDungCuKhong}}</td>
    </tr>
    <tr>
        <td class="text" colspan="3"> <span style="font-style: italic;font-weight: bold;">
                <div class="paddingTopNone">Đặt Plaque dao điện</div>
            </span> <br>{{DatPlaqueDienText}} </td>
    </tr>
</table>
<table style="width: 100%;margin-top: 4px;">
    <tr>
        <td style="text-align: center;font-size: 14px;"> <b>ĐIỀU DƯỠNG CHẠY NGOÀI</b> </td>
        <td style="text-align: center;font-size: 14px;"> <b>ĐIỀU DƯỠNG DỤNG CỤ</b> </td>
        <td style="text-align: center;font-size: 14px;"> <b>ĐIỀU DƯỠNG/KTV GÂY MÊ</b> </td>
        <td style="text-align: center;font-size: 14px;"> <b>BÁC SỸ GÂY MÊ</b> </td>
        <td style="text-align: center;"> <b>PHẪU THUẬT VIÊN</b> </td>
    </tr>
    <tr>
        <td style="text-align: center;font-size: 14px;"> <i>(Ký và ghi rõ họ tên)</i> </td>
        <td style="text-align: center;font-size: 14px;"> <i>(Ký và ghi rõ họ tên)</i> </td>
        <td style="text-align: center;font-size: 14px;"> <i>(Ký và ghi rõ họ tên)</i> </td>
        <td style="text-align: center;font-size: 14px;"> <i>(Ký và ghi rõ họ tên)</i> </td>
        <td style="text-align: center;font-size: 14px;"> <i>(Ký và ghi rõ họ tên)</i> </td>
    </tr>
    <tr>
        <td style="font-weight: bold;text-align: center;padding-top: 69px;font-size: 14px;"> {{DdChayNgoai}} </td>
        <td style="font-weight: bold;text-align: center;padding-top: 69px;font-size: 14px;"> {{DdDungCu}} </td>
        <td style="font-weight: bold;text-align: center;padding-top: 69px;font-size: 14px;">{{DdGayMe}} </td>
        <td style="font-weight: bold;text-align: center;padding-top: 69px;font-size: 14px;"> {{BsGayMe}} </td>
        <td style="font-weight: bold;text-align: center;padding-top: 69px;font-size: 14px;"> {{PhauThuatVien}} </td>
    </tr>
</table>
<div style="break-after:page"></div>'
 where Name='BangKiemAnToanPhauThuat' 
UPDATE CauHinh
Set [Value] = '3.4.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'