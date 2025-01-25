update Template
set  Body=N'<style>
    table,
    th,
    td {
        border-spacing: 0;
        font-family: Times New Roman;
    }

    th,
    td {
        padding: 1.5px;
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
    }

    .container .value {
        display: table-cell;
        width: 100%;
        height: 100%;
        position: relative;
        box-sizing: border-box;
        border-bottom: 1px dotted black;
    }

    .containerGD {
        width: 50%;
        display: table;
    }

    .containerGD .label {
        width: max-content;
    }

    .containerGD .value {
        display: table-cell;
        width: 100%;
        height: 100%;
        vertical-align: top;
        position: relative;
        box-sizing: border-box;
        border-bottom: 1px dotted black;
    }

    #customers {
        width: 100%;
        border-collapse: collapse;
    }

    #customers td,
    #customers th {
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
        display: inline-block
    }

    #img1 {
        width: 70%;
        height: 140px;
        display: block;
    }

    #img {
        align: right;
        height: 130px;
        width: 100%;
        display: block;
    }
</style>
<table style="width: 100%;">
    <tbody>
        <tr>
            <td style="text-transform: uppercase; text-align:center" width="50%"><b>BỆNH VIỆN ĐKQT BẮC HÀ</b> </td>
            <td style="text-align:center"> Mã BA: {{MaBA}} </td>
        </tr>
        <tr>
            <td style="text-align:center"><b>Khoa Ngoại - Gây Mê Hồi Sức</b></td>
            <td style="text-align:center"><b>PHIẾU THEO DÕI <br> GIẢM ĐAU SAU MỔ</b></td>
        </tr>
    </tbody>
</table>
<table id="customers">
    <tr>
        <td style="font-size:18px"> <b> Hành chính </b> </td>
    </tr>
    <tr>
        <td>
            <div class="container">
                <div class="label">Tên bệnh nhân: </div>
                <div class="value" style="width:85%"><b>{{TenBenhNhan}}</b></div>
                <div class="label">Tuổi: </div>
                <div class="value"><b>{{Tuoi}}</b></div>
                <div class="label"> &nbsp; Giới tính: Nam <span class="square"> {{Nam}} </span> &nbsp;&nbsp; Nữ <span
                        class="square"> {{Nu}} </span>
        </td>
    </tr>
    <tr>
        <td style="border-top:hidden;">
            <div class="container">
                <div class="label">Cân nặng: </div>
                <div class="value" style="width:30%"><b>{{CanNang}}</b></div>
                <div class="label">kg&nbsp;&nbsp;&nbsp;&nbsp;Chiều cao: </div>
                <div class="value" style="width:30%"><b>{{ChieuCao}}</b></div>
                <div class="label">cm&nbsp;&nbsp;&nbsp;&nbsp;ĐT liên lạc: </div>
                <div class="value"><b>{{DienThoaiLienLac}}</b></div>
            </div>
        </td>
    </tr>
    <tr>
        <td style="border-top:hidden;">
            <div class="container">
                <div class="label">Khoa: </div>
                <div class="value" style="width:30%"><b>{{Khoa}}</b></div>
                <div class="label">Phòng: </div>
                <div class="value" style="width:30%"><b>{{Phong}}</b></div>
                <div class="label">Giường: </div>
                <div class="value"><b>{{Giuong}}</b></div>
            </div>
        </td>
    </tr>
    <tr>
        <td style="border-top:hidden;">
            <div class="container">
                <div class="label">Chẩn đoán: </div>
                <div class="value breakword"><b>{{ChanDoan}}</b></div>
            </div>
        </td>
    </tr>
    <tr>
        <td style="border-top:hidden;">
            <div class="container">
                <div class="label">Phẫu thuật: </div>
                <div class="value breakword"><b>{{PhauThuat}}</b></div>
            </div>
        </td>
    </tr>
    <tr>
        <td style="font-size:18px"> <b> Phần dành cho BS thực hiện thủ thuật giảm đau sau mổ </b> </td>
    </tr>
</table>
<table id="customers">
    <tr>
        <td colspan="3" style="border-top:hidden;">
            <div class="container">
                <div class="label">Bác sĩ làm giảm đau: </div>
                <div class="value" style="width:70%"><b>{{BacSiLamGiamDau}}</b></div>
                <div class="label">Điện thoại: </div>
                <div class="value"><b>{{DienThoai}}</b></div>
            </div>
        </td>
    </tr>
    <tr>
        <td colspan="3" style="border-top:hidden;"> Phương pháp giảm đau: </td>
    </tr>
    <tr>
        <td style="border-top:hidden;"> 1. Tê vùng: <span class="square"> {{MorphinTs}} </span> Morphin TS </td>
        <td style="border-top:hidden; border-left:hidden;"> <span class="square"> {{NMC}} </span> NMC (một liều - BTĐ -
            PCEA) </td>
        <td style="border-top:hidden;border-left:hidden;" width="20%"> <span class="square"> {{DRCanhTay}} </span> ĐR
            cánh tay </td>
    </tr>
    <tr>
        <td style="border-top:hidden;"> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;
            &nbsp;&nbsp;&nbsp;&nbsp; <span class="square"> {{TKBenDui}} </span> TK bẹn, đùi </td>
        <td style="border-top:hidden; border-left:hidden;"> <span class="square"> {{CanhCotSong}} </span> Cạnh cột sống
            <span class="square"> {{TAP}} </span> TAP </td>
        <td style="border-top:hidden;border-left:hidden;"> <span class="square"> {{KhacTeVung}} </span>
            Khác:{{KhacVungTeText}} </td>
    </tr>
    <tr>
        <td style="border-top:hidden;" colspan="2"> 2. Toàn thân: <span class="square"> {{MorphinPCA}} </span> Morphin:
            (IM/SC - dò liều IV - PCA) </td>
        <td style="border-top:hidden; border-left:hidden;"> <span class="square"> {{Nefopam}} </span> Nefopam </td>
    </tr>
    <tr>
        <td style="border-top:hidden;"> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;
            &nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp; <span class="square"> {{Ketamin}} </span> Ketamin &nbsp; <span
                class="square"> {{Paracetamol}} </span> Paracetamol </td>
        <td style="border-top:hidden; border-left:hidden;"> <span class="square"> {{NSAID}} </span> NSAID
            &nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp; <span class="square"> {{Gapentin}} </span> Gapentin </td>
        <td style="border-top:hidden;border-left:hidden;"> <span class="square"> {{KhacToanThan}} </span>
            Khác:{{KhacToanThanText}} </td>
    </tr>
    <tr>
        <td colspan="2" style="border-top:hidden;border-bottom:hidden;">
            <div class="container">
                <div class="label">Vị trí đặt Catheter: C/D/L </div>
                <div class="value" style="width:90%"><b>{{CLD}}</b></div>
                <div class="label">Độ sâu: </div>
                <div class="value"><b>{{DoSau}}</b></div>
                <div class="label">cm&nbsp;&nbsp;&nbsp;Đường: </div>
            </div>
        </td>
        <td style="border-top:hidden; border-left:hidden;border-bottom:hidden;"> <span class="square"> {{DuongGiua}}
            </span> Giữa <span class="square"> {{DuongBen}} </span> Bên </td>
    </tr>
</table>
<table id="customers">
    <tr>
        <td colspan="2" style="border-top:hidden;">
            <div class="container">
                <div class="label">Thuốc giảm đau:</div>
                <div class="value" style="width:50%"><b>{{ThuocGiamDau}}</b></div>
                <div class="label">Thời gian bắt đầu GĐSM: </div>
                <div class="value"><b>{{ThoiGianBatDauGDSM}}</b></div>
            </div>
        </td>
    </tr>
    <tr>
        <td style="border-top:hidden;">
            <div class="container">
                <div class="label">Thuốc 1:</div>
                <div class="value" style="width:70%"><b>{{Thuoc1}}</b></div>
                <div class="label">Thể tích: </div>
                <div class="value"><b>{{TheTich}}</b></div>
                <div class="label">ml</div>
            </div>
        </td>
        <td style="border-top:hidden;border-left:hidden;">
            <div class="container">
                <div class="label">Nồng độ:</div>
                <div class="value" style="width:80%"><b>{{NongDo1}}</b></div>
                <div class="label">%,mg,mcg/ 1ml</div>
            </div>
        </td>
    </tr>
    <tr>
        <td style="border-top:hidden;">
            <div class="container">
                <div class="label">Thuốc 2:</div>
                <div class="value"><b>{{Thuoc2}}</b></div>
            </div>
        </td>
        <td style="border-top:hidden;border-left:hidden;">
            <div class="container">
                <div class="label">Nồng độ:</div>
                <div class="value" style="width:80%"><b>{{NongDo2}}</b></div>
                <div class="label">%,mg,mcg/ 1ml</div>
            </div>
        </td>
    </tr>
    <tr>
        <td style="border-top:hidden;">
            <div class="container">
                <div class="label">Thuốc 3:</div>
                <div class="value"><b>{{Thuoc3}}</b></div>
            </div>
        </td>
        <td style="border-top:hidden;border-left:hidden;">
            <div class="container">
                <div class="label">Nồng độ:</div>
                <div class="value" style="width:80%"><b>{{NongDo3}}</b></div>
                <div class="label">%,mg,mcg/ 1ml</div>
            </div>
        </td>
    </tr>
    <tr>
        <td colspan="2" style="border-top:hidden;">
            <div class="container">
                <div class="label">Tốc độ thuốc tối đa (ml/giờ):</div>
                <div class="value" style="width:50%"><b>{{TocDoThuocToiDa}}</b></div>
                <div class="label">Dự kiến thời gian kết thúc: </div>
                <div class="value"><b>{{DuKienThoiGianKetThuc}}</b></div>
            </div>
        </td>
    </tr>
    <tr>
        <td colspan="2" style="font-size:18px;border-bottom:hidden;"> <b> Phần dành cho điều dưỡng chăm sóc giảm đau sau
                mổ </b> </td>
    </tr>
</table>
<table id="customers">
    <tr>
        <th width="10%"> <i>Ngày</i> </th>
        <th width="7%"> <i>Giờ</i> </th>
        <th> <i>Tốc độ(ml/giờ)</i> </th>
        <th> <i>Liều Bolus(ml)</i> </th>
        <th width="10%"> <i>Thời gian khóa(phút)</i> </th>
        <th> <i>Liều tối đa/4 giờ</i> </th>
        <th width="15%"> <i>Người chỉ định</i> </th>
    </tr>
    <tbody> {{HTMLPhanGianhChoNguoiDieuDuongSauMos}} </tbody>
</table> Chuẩn bị thuốc, thay thuốc <table id="customers">
    <tr>
        <th width="10%"> <i>Ngày Giờ</i> </th>
        <th width="30%"> <i>Loại thuốc</i> </th>
        <th width="16%"> <i>Thể tích chuẩn bị(ml)</i> </th>
        <th width="10%"> <i>Kiểm tra(ml)</i> </th>
        <th> <i>Hủy bỏ(ml)</i> </th>
        <th width="20%"> <i>Người thực hiện</i> </th>
    </tr>
    <tbody> {{HTMLChuanBiThuocVaThayThuocs}} </tbody>
</table>
<table width="100%">
    <tr>
        <td colspan="2"> Bupivacaine: <b>BPV</b>; Levobupovacaine: <b>LBPV</b>; Adrenaline: <b>ADRE</b>; Fentanyl:
            <b>FEN</b>; Nofopam: <b>NEFO</b>; Ketamin: <b>KETA</b> </td>
    </tr>
    <tr>
        <td colspan="2">
            <div class="container">
                <div class="label">Kết thúc và rút catheter: {{KetThucVaRutCetheter}}&nbsp; &nbsp; &nbsp; &nbsp;
                    Ngày/giờ: </div>
                <div class="value" style="width:40%"><b>{{NgayGioKetThuc}}</b></div>
                <div class="label">Người rút</div>
                <div class="value"><b>{{NguoiRut}}</b></div>
            </div>
        </td>
    </tr>
    <tr>
        <td colspan="2"> <b>CHÚ Ý KHÔNG DÙNG THÊM THUỐC AN THẦN HOẶC GIẢM ĐAU NHÓM MORPHIN NẾU KHÔNG CÓ CHỈ ĐỊNH CỦA BÁC
                SỸ GÂY MÊ HỒI SỨC</b> </td>
    </tr>
    <tr>
        <td> G337 VĐ - 16 </td>
    </tr>
</table>
<div style="break-after:page"></div>
<table width="100%">
    <tr>
        <th style="font-size:18px;"> BẢNG ĐÁNH GIÁ </th>
    </tr>
</table>
<table id="customers">
    <tr>
        <th rowspan="3"> Điểm đau </th>
        <td style="border-right:hidden;border-bottom:hidden" width="30%"> 0: Không đau </td>
        <td style="border-right:hidden;border-bottom:hidden" width="30%"> 2: Đau nhẹ </td>
        <td width="30%" style="border-bottom:hidden"> 4: Đau vừa </td>
    </tr>
    <tr style="border-bottom:hidden">
        <td style="border-right:hidden"> 6: Đau nặng </td>
        <td style="border-right:hidden"> 8: Đau rất nặng </td>
        <td> 10: Đau không thể chịu được </td>
    </tr>
    <tr>
        <!-- <td colspan="3" align="middle"> <img id="img1" src="C:\Users\User\Desktop\custom.png"> </td> -->
        <td colspan="3">
            <table style="width: 100%;">
                <tr style="border: 1px solid black;">
                    <td style="border: none;text-align: center;"><img style="height:60px;padding: 0px;"
                            src="{{ImageTiket}}"></td>
                    <td style="border: none;text-align: center;"><img style="height:60px;padding: 0px;"
                            src="{{ImageTiket2}}"></td>
                    <td style="border: none;text-align: center;"><img style="height:60px;padding: 0px;"
                            src="{{ImageTiket3}}"></td>
                    <td style="border: none;text-align: center;"><img style="height:60px;padding: 0px;"
                            src="{{ImageTiket4}}"></td>
                    <td style="border: none;text-align: center;"><img style="height:60px;padding: 0px;"
                            src="{{ImageTiket5}}"></td>
                    <td style="border: none;text-align: center;"><img style="height:60px;padding: 0px;"
                            src="{{ImageTiket6}}"></td>
                </tr>
                <tr style="border: 1px solid black;">
                    <td style="border: none;text-align: center;">0 <span style="float: right;">1</span><br>Không<br>đau
                    </td>
                    <td style="border: none;text-align: center;">2 <span style="float: right;">3</span><br>Đau<br>nhẹ
                    </td>
                    <td style="border: none;text-align: center;">4 <span style="float: right;">5</span><br>Đau<br>vừa
                    </td>
                    <td style="border: none;text-align: center;">6 <span style="float: right;">7</span><br>Đau<br>nặng
                    </td>
                    <td style="border: none;text-align: center;">8 <span style="float: right;">9</span><br>Đau<br>rât
                        nặng</td>
                    <td style="border: none;text-align: center;">10 <span style="float: right;"></span><br>Không
                        thể<br>chịu được</td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <th rowspan="2"> Điểm an thần </th>
        <td colspan="2" style="border-right:hidden; border-bottom:hidden"> 0: Tỉnh táo </td>
        <td style="border-bottom:hidden;"> 2: Buồn ngủ thường xuyên </td>
    </tr>
    <tr>
        <td colspan="2" style="border-right:hidden;"> 1: Thỉnh thoảng buồn ngủ, dễ đánh thức </td>
        <td> 3: Ngủ gà, khó đánh thức </td>
    </tr>
    <tr>
        <th rowspan="4"> Điểm vận động </th>
        <td style="border-bottom:hidden;"> 0: Vận động bình thường </td>
        <td rowspan="4" colspan="2">
            <table style="width: 100%;">
                <tr style="border: 1px solid black;">
                    <td style="border: none;"><img style="height:50;width: 200px;padding: 0px;" src="{{ImageTiket7}}">
                    </td>
                    <td style="border: none;text-align: center;">Không thể cử động <br>gối và bàn chân</td>
                </tr>
                <tr style="border: 1px solid black;">
                    <td style="border: none;"><img style="height:50;width: 200px;padding: 0px;" src="{{ImageTiket8}}">
                    </td>
                    <td style="border: none;text-align: center;">Chỉ cử động<br> bàn chân</td>
                </tr>
                <tr style="border: 1px solid black;">
                    <td style="border: none;"><img style="height:50;width: 200px;padding: 0px;" src="{{ImageTiket9}}">
                    </td>
                    <td style="border: none;text-align: center;">Chỉ cử động <br> nhẹ gối</td>
                </tr>
                <tr style="border: 1px solid black;">
                    <td style="border: none;"><img style="height:50;width: 200px;padding: 0px;" src="{{ImageTiket10}}">
                    </td>
                    <td style="border: none;text-align: center;">Cử động gối và bàn <br> chân bình thường</td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td style="border-bottom:hidden;"> 1: Chỉ cử động nhẹ gối </td>
    </tr>
    <tr>
        <td style="border-bottom:hidden;"> 2: Chỉ cử động bàn chân </td>
    </tr>
    <tr>
        <td> 3: Không vận động </td>
    </tr>
</table>
<table width="100%">
    <tr>
        <td> <b>Các vấn đề chú ý</b> </td>
    </tr>
</table>
<table id="customers">
    <tr>
        <th colspan="2"> Triệu chứng </th>
        <th> Xử trí </th>
    </tr>
    <tr>
        <td width="10%" style="vertical-align: baseline; text-align:center"> 1 </td>
        <td width="45%" style="vertical-align: baseline;"> Đau không đủ </td>
        <td>
            <div class="containerGD">
                <div class="label">Tiêm Bolus</div>
                <div class="value" style="width:10%"><b>{{bolus}}</b></div>
                <div class="label"> ml và nhắc lại đến khi về ngưỡng đau cho phép.</div>
            </div>
            <div class="containerGD">
                <div class="label">Sau đó tăng tốc độ truyền lên thêm 2ml/giờ. Báo BS trực GĐ</div>
            </div>
        </td>
    </tr>
    <tr>
        <td width="10%" style="vertical-align: baseline; text-align:center"> 2 </td>
        <td width="45%" style="vertical-align: baseline;"> Tụt huyết áp </td>
        <td>
            <div class="containerGD">
                <div class="label">Thường do thiếu KLTH. Nếu HA tối đa xuống < </div>
                        <div class="value" style="width:10%"><b>{{HA}}</b></div>
                        <div class="label">mmHG -></div>
                </div>
                <div class="containerGD">
                    <div class="label"><b>NGỪNG TRYỀN</b> giảm đau, cho thở <b>OXY</b>, nằm đầu bằng, kê</div>
                </div>
                <div class="containerGD">
                    <div class="label"><b>CAO CHÂN</b> truyền dịch</div>
                    <div class="value"><b>{{TDich}}</b></div>
                    <div class="label">ml. Báo BS trực GĐ</div>
                </div>
        </td>
    </tr>
    <tr>
        <td width="10%" style="vertical-align: baseline; text-align:center"> 3 </td>
        <td width="45%" style="vertical-align: baseline;"> Suy hô hấp </td>
        <td> <span style="text-decoration:underline">Cho thở <b>OXY, NGỪNG TRUYỀN</b> giảm đau</span><br> Nếu <b>NGỪNG
                THỞ</b> hoặc <b>TÍM TÁI</b> -> Bóp bóng hỗ trợ, mời BSGĐ, cho <b>NALOXONE 400mcg</b> tĩnh mạch<br> Nếu
            BN thở chậm &le; 8 lần/phút hoăc điểm an thần 3 -> <b>NGỪNG TRUYỀN</b> giảm đau, báo BSGĐ, tiêm <b>NALOXONE
                100mcg</b> TM </td>
    </tr>
    <tr>
        <td width="10%" style="text-align:center"> 4 </td>
        <td width="45%"> An thần quá mức </td>
        <td> Nếu điểm an thần &ge; 2 -> <b>NGỪNG TRUYỀN</b> giảm đau, báo bác sỹ </td>
    </tr>
    <tr>
        <th colspan="3"> <br> THEO DÕI </th>
    </tr>
    <tr>
        <td rowspan="5"> <b>Ngày 1</b> </td>
        <td colspan="2" style="border-bottom:hidden;"> Nếu BN nằm tại Hồi tỉnh: 1 giờ/lần trong 2 giờ đầu. Sau đó 2
            giờ/lần trong 6 giờ tiếp, sau đó theo thời gian quy định đi tua giảm đau </td>
    </tr>
    <tr>
        <td colspan="2" style="border-bottom:hidden;"> &#10004; Mạch và huyết áp </td>
    </tr>
    <tr>
        <td colspan="2" style="border-bottom:hidden;"> &#10004; Điểm đau, điểm an thần, nhịp thở </td>
    </tr>
    <tr>
        <td colspan="2" style="border-bottom:hidden;"> &#10004; Điểm vận động và cảm giác </td>
    </tr>
    <tr>
        <td colspan="2"> &#10004; Mực phong bế tối đa: 3 lần/ngày hoặc/và mỗi lần Bolus </td>
    </tr>
    <tr>
        <td style="text-align:center"> Sau khi Bolus </td>
        <td colspan="2"> Mạch và huyết áp: 5 phút/lần trong 20 phút<br> Điểm đau, điểm an thần, nhịp thở: 20 phút sau
            bolus </td>
    </tr>
    <tr>
        <td style="text-align:center"> Vị trí Catheter </td>
        <td colspan="2"> 3 lần/ngày trên Hồi tỉnh và mỗi lần đi tua bệnh phòng </td>
    </tr>
</table>
<table width="100%">
    <tr>
        <td> <br> <b>CHÚ Ý KHÔNG DÙNG THÊM THUỐC AN THẦN HOẶC GIẢM ĐAU NHÓM MORPHIN NẾU KHÔNG CÓ CHỈ ĐỊNH CỦA BÁC SỸ GÂY
                MÊ HỒI SỨC</b> </td>
    </tr>
</table>
<div style="break-after:page"></div> <br>
<table width="100%">
    <tr>
        <th style="font-size:18px;" width="70%"> BẢNG THEO DÕI BỆNH NHÂN GIẢM ĐAU SAU MỔ </th>
        <td width="15%"> <i>HT: Hồi tỉnh</i> </td>
        <td width="15%"> <i>BP: Bệnh phòng</i> </td>
    </tr>
</table>
<table id="customers">
    <tr style="text-align:center">
        <td colspan="2"> Địa điểm </td>
        <td width="5%"> HT </td>
        <td width="5%"> HT </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> BP </td>
        <td width="5%"> BP </td>
        <td width="5%"> BP </td>
        <td width="5%"> BP </td>
        <td width="5%"> BP </td>
    </tr>
    <tr style="text-align:center">
        <td colspan="2"> Ngày giảm đau </td>
        <td width="5%"> 1 </td>
        <td width="5%"> 1 </td>
        <td width="5%"> 1 </td>
        <td width="5%"> 1 </td>
        <td width="5%"> 1 </td>
        <td width="5%"> 1 </td>
        <td width="5%"> 1 </td>
        <td width="5%"> 1 </td>
        <td width="5%"> 2 </td>
        <td width="5%"> 2 </td>
        <td width="5%"> 2 </td>
        <td width="5%"> 2 </td>
        <td width="5%"> 3 </td>
        <td width="5%"> 3 </td>
        <td width="5%"> 3 </td>
        <td width="5%"> 3 </td>
        <td width="5%"> &nbsp; </td>
    </tr>
    <tr style="text-align:center">
        <td colspan="2"> Ngày/Giờ </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
    </tr>
    <tr style="text-align:center">
        <td rowspan="2" width="8%"> Thuốc </td>
        <td> Tốc độ (ml/giờ) </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
    </tr>
    <tr style="text-align:center">
        <td> Bolus (ml) </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
    </tr>
    <tr style="text-align:center">
        <td colspan="2"> Mạch (lần/phút) </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
    </tr>
    <tr style="text-align:center">
        <td colspan="2"> Huyết áp (mmHg) </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
    </tr>
    <tr style="text-align:center">
        <td colspan="2"> Nhịp thở (lần/phút) </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
    </tr>
    <tr style="text-align:center">
        <td colspan="2"> SpO<sub>2</sub> (%) </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
    </tr>
    <tr style="text-align:center">
        <td rowspan="2" width="8%"> Điểm đau </td>
        <td> Khi nghỉ </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
    </tr>
    <tr style="text-align:center">
        <td> Khi VĐ </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
    </tr>
    <tr style="text-align:center">
        <td colspan="2"> Điểm vận động </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
    </tr>
    <tr style="text-align:center">
        <td colspan="2"> Mức phong bế tối đa </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
    </tr>
    <tr style="text-align:center">
        <td colspan="2"> Ghi chú <br> &nbsp; <br> &nbsp; <br> &nbsp; <br> &nbsp; <br> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
    </tr>
    <tr style="text-align:center">
        <td colspan="2"> ĐD theo dõi (Ký và ghi rõ họ tên) <br> &nbsp; <br> &nbsp; <br> &nbsp; <br> &nbsp; <br> &nbsp;
        </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
    </tr>
    <tr style="text-align:center">
        <td colspan="2"> BS đi tua ghi chú và ký tên <br> &nbsp; <br> &nbsp; <br> &nbsp; <br> &nbsp; <br> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
        <td width="5%"> &nbsp; </td>
    </tr>
</table>
<table width="100%">
    <tr>
        <td> <br> <b>CHÚ Ý KHÔNG DÙNG THÊM THUỐC AN THẦN HOẶC GIẢM ĐAU NHÓM MORPHIN NẾU KHÔNG CÓ CHỈ ĐỊNH CỦA BÁC SỸ GÂY
                MÊ HỒI SỨC</b> </td>
    </tr>
</table>'
where Name='PhieuTheoDoiGiamDauSauMos'
UPDATE CauHinh
Set [Value] = '3.4.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'