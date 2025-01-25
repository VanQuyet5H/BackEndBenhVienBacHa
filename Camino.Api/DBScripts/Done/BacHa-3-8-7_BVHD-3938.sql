

INSERT INTO template (Name, Title,
Body,TemplateType,Language,Description,Version,IsDisabled,CreatedById,LastUserId,LastTime,CreatedOn)
VALUES (N'BangKeChiPhiNgoaiGoiKCB', N'Bảng Kê Chi Phí Ngoài Gói Khám', N'<style>
    * {
        box-sizing: border-box;
    }

    span.square {
        vertical-align: bottom;
        border: solid 1px;
        width: 30px;
        height: 30px;
        text-align: center;
        margin: 2px;
        display: inline-block
    }

    input[type=checkbox].css-checkbox {
        position: absolute;
        overflow: hidden;
        clip: rect(0 0 0 0);
        height: 1px;
        width: 1px;
        margin: -1px;
        padding: 0;
        border: 0;
    }

    input[type=checkbox].css-checkbox+label.css-label {
        padding-left: 20px;
        height: 15px;
        display: inline-block;
        line-height: 15px;
        background-repeat: no-repeat;
        background-position: 0 0;
        font-size: 15px;
        vertical-align: middle;
        cursor: pointer;
    }

    input[type=checkbox].css-checkbox:checked+label.css-label {
        background-position: 0 -15px;
    }

    .css-label {
        background-image: url(http://csscheckbox.com/checkboxes/lite-x-red.png);
    }
</style>
<div>
    <div>
        <div style="float: left;width: 50%;padding: 10px;">
            <table>
                <tbody>
                    <tr> <b>Cơ sở khám, chữa bệnh: BỆNH VIỆN ĐKQT BẮC HÀ</b></br> <b>{{TenKhoa}}</b></br>
                        <b>{{MaKhoa}}</b> </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div style="float: right;padding-left:100px; overflow: hidden;">
            <table>
                <tbody>
                    <tr>
                        <td style="padding-left: -5px;"> <b>Mẫu số: 01/KBCB </b></br> <b>Mã số người bệnh: {{MaBN}}
                            </b></br> <b>Số khám bệnh: {{MaTN}}</b> </br> <b> {{SoBenhAn}}</b> </br> </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
    <table style="padding: 5px;width: 100%;">
        <th> <span style="font-size: 30px;">BẢNG KÊ CHI PHÍ {{TitleBangKe}}  <span class="">(Ngoài gói)</span></span><br> </th>
    </table>
    <div> <b>I.Hành chính</b></br>
        <table style="width: 100%; overflow: hidden;">
            <tbody>
                <tr>
                    <td style="padding-left: -5px;width: 100%;">(1) Họ tên người bệnh: <b>{{HoTen}} </b> <span
                            style="padding-left:60px"> Ngày,tháng,năm sinh: <b>{{NamSinh}} </b> </span> <span
                            style="padding-left:40px">Giới tính: <b>{{GioiTinh}}</b></span></td>
                </tr>
                <tr>
                    <td style="padding-left: -5px;width: 100%;" colspan="1">(2) Địa chỉ hiện tại: <b>{{DiaChi}}</b>
                        <span style="padding-left:40px">(3) Mã khu vực(K1/K2/K3): <b>{{MKV}}</b></span></td>
                </tr>
                <tr>
                    <td style="padding-left: -5px;  width:30%" colspan="1"> (4) Mã thẻ BHYT:<b> {{MaBHYT}} </b> <span
                            style="padding-left:40px">Giá trị từ:<b>{{BHYTTuNgay}}</b> đến <b>{{BHYTDenNgay}}</b></span>
                    </td>
                </tr>
                <tr>
                    <td style="padding-left: -5px; width: 30%;" colspan="1">(5) Nơi ĐK KCB ban đầu:
                        <b>{{NoiDKKCBBanDau}}</b> <span style="padding-left:40px">(6) Mã: <b>{{MaKCBBanDau}}</b></span>
                    </td>
                </tr>
                <tr>
                    <td style="padding-left: -5px; width:30%">(7) Đến Khám: <b>{{NgayDenKham}}</b></td>
                    </td>
                </tr>
                <tr>
                    <td style="padding-left: -5px; width:1000px" colspan="3">(8) Điều trị ngoại trú/nội trú từ:
                        {{DieuTriKNT}}</td>
                </tr>
                <tr>
                    <td style="padding-left: -5px;width: 100%;">(9) Kết thúc khám/điều trị: {{KetThucKhamNgoaiTru}} Tổng
                        số ngày điều trị: <b>{{SoNgayDTri}} </b>(10) Tình trạng ra viện : <b>{{TinhTrangRaVien}} </td>
                </tr>
            </tbody>
        </table>
        <table style="width:100%;">
            <tr>
                <td style="padding-left: -5px;width: 25%;">(11) Cấp cứu: <b>{{CoCapCuu}}</b>(12) Đúng tuyến:
                    <b>{{CoDungTuyen}}</b> Nơi chuyển đến: <b>{{NoiChuyenDen}}</b>Nơi chuyển đi: <b>{{NoiChuyenDi}}</b>
                </td>
            </tr>
            <tr>
                <td style="padding-left: -5px;width: 40%;">(13) Thông tuyến: <b>{{CoThongTuyen}}</b>(14) Trái tuyến:
                    <b>{{CoTraiTuyen}}</b></td>
            </tr>
            <tr>
                <td style="padding-left: -5px;width: 100%;" colspan="1">(15) Chẩn đoán xác định: <b>
                        {{ChuanDoanXacDinh}}</b> <span style="padding-left:20px">(16) Mã bệnh: <b>{{MaICD10}}</b></span>
                </td>
            </tr>
            <tr>
                <td style="padding-left: -5px;width: 100%;" colspan="1">(17) Bệnh kèm theo: <b>{{BenhKemTheo}}</b> <span
                        style="padding-left:40px">(18) Mã bệnh kèm theo: <b>{{ICDKemTheo10}}</b></span></td>
            </tr>
            <tr>
                <td style="padding-left: -5px;width: 100%;" colspan="1">(19) Thời điểm đủ 05 năm liên tục từ ngày: <b>
                        {{ThoiGian5Nam}}</b> <span style="padding-left:40px">(20) Miễn cùng chi trả trong năm từ
                        ngày:<b>{{NgayMiemCungTC}}</b></span></td>
            </tr>
        </table>
    </div>
    <div> <b>II. Phần Chi phí khám, chữa bệnh </b> {{DateItemChiPhis}} </div>
    <div style="padding-top: 5px;">
        <table style="width:100%;">
            <tbody>
                <tr>
                    <td><b>Tổng chi phí lần khám bệnh/cả đợt điều trị:</b> {{TongChiPhi}} đồng </br>(Viết bằng chữ:
                        {{SoTienBangChu}})
                </tr>
            </tbody>
        </table>
        <table style="width:100%;">
            <tbody>
                <tr>
                    <td><b>Trong đó </b>, số tiền do: </br> - Quỹ BHYT thanh toán: {{TTQuyTT}}</br> - Người bệnh trả
                        trong đó: {{NguoiBenhPhaiTra}}</b> </br> + Cùng chi trả trong phạm vi BHYT: {{BHYTChiTra}} </br>
                        + Các khoản phải trả khác: {{CacKhoanTraKhac}} </br> - Nguồn khác: {{SoTienKhac}}</br> </td>
                </tr>
            </tbody>
        </table>
    </div>
    <div>
        <table> 
            <tr style="width:100%">
                <td style="width:31%;text-align:center"><b>NGƯỜI LẬP BẢNG KÊ</b><br /><i>(Ký, ghi rõ họ tên)</i></td>
                <td style="width:31%;text-align:center"><b>KẾ TOÁN VIỆN PHÍ</b><br /><i>(Ký, ghi rõ họ tên)</i></td>
                <td style="width:28%;text-align:center"><i>{{NgayHienTai}}</i><br /><b>XÁC NHẬN CỦA NGƯỜI
                        BỆNH</b><br /><i>(Ký, ghi rõ họ tên)</td>
            </tr>
        </table>
    </div>
</div>',1,1,N'Bảng Kê Chi Phí Ngoài Gói Khám',1,0,1,1,GETDATE(),GETDATE())

Update dbo.CauHinh
Set [Value] = '3.8.7' where [Name] = 'CauHinhHeThong.DatabaseVesion'