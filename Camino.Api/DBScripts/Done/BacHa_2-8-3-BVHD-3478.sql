update Template
set Body = N'<style>
    table,
    th,
    td {
        border-spacing: 0;
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
        display: inline-block
    }

    span.square2 {
        vertical-align: bottom;
        border: solid 1px;
        width: 80px;
        height: 20px;
        text-align: center;
        margin: 2px;
        display: inline-block
    }
</style>
<table style="width: 100%;">
    <tbody>
        <tr>
            <th style="text-transform: uppercase;" width="35%">Bệnh Viện ĐKQT Bắc Hà </th>
            <td width="30%"> &nbsp; </td>
            <td> &nbsp; </td>
        </tr>
        <tr>
            <th style="text-transform: uppercase;">Khoa Phụ Sản</td>
            <th>
                </td>
            <td> </td>
        </tr>
        <tr>
            <td colspan="3" style="text-align:center; font-size:22px"><b>PHIẾU SÀNG LỌC DINH DƯỠNG</b> <br> (Dùng cho
                phụ nữ mang thai)</td>
        </tr>
    </tbody>
</table> <br>
<table width="100%">
    <tr>
        <td>
            <div class="container">
                <div class="label">Họ và tên: </div>
                <div class="value" style="width:80%"><b>{{HoTen}}</b></div>
                <div class="label">Năm sinh: </div>
                <div class="value"><b>{{NamSinh}}</b></div>
                <div class="label"> Giới tính: Nữ </div>
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <div class="container">
                <div class="label">Địa chỉ: </div>
                <div class="value breakword"><b>{{DiaChi}}</b></div>
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <div class="containerGD">
                <div class="label">Tuổi thai: </div>
                <div class="value"><b>{{TuoiThai}}</b></div>
                <div class="label">&nbsp;tuần&nbsp;&nbsp;&nbsp;Theo kinh cuối cùng <input type="checkbox"
                        id="TheoKinhCuoiCung" name="TheoKinhCuoiCung" {{TheoKinhCuoiCung}} disabled>&nbsp;&nbsp;&nbsp;
                    Siêu âm 3 tháng đầu thai kỳ <input type="checkbox" id="BaThangDauThaiKy" name="BaThangDauThaiKy"
                        {{BaThangDauThaiKy}} disabled> </div>
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <div class="container">
                <div class="label">Số buồng: </div>
                <div class="value" style="width:30%"><b>{{SoBuong}}</b></div>
                <div class="label">số giường: </div>
                <div class="value" style="width:30%"><b>{{SoGiuong}}</b></div>
                <div class="label">Số bệnh án: </div>
                <div class="value breakword"><b>{{SoBenhAn}}</b></div>
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <div class="container">
                <div class="label">Chẩn đoán: </div>
                <div class="value breakword"><b>{{ChanDoan}}</b></div>
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <div class="containerGD">
                <div class="label">Cân nặng trước khi mang thai: </div>
                <div class="value"><b>{{CanNang}}</b></div>
                <div class="label">kg &nbsp;&nbsp;&nbsp; Chiều cao</div>
                <div class="value"><b>{{ChieuCao}}</b></div>
                <div class="label">m</div>
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <div class="containerGD">
                <div class="label">BMI trước khi mang thai: </div>
                <div class="value"><b>{{BMI}}</b></div>
                <div class="label">kg/m<sup>2</sup></div>
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <div class="containerGD">
                <div class="label">Cân nặng hiện tại: </div>
                <div class="value"><b>{{CanNangHienTai}}</b></div>
                <div class="label">kg</div>
            </div>
        </td>
    </tr>
</table>
<table id="customers">
    <tr>
        <td rowspan="3" width="25%"> BMI trước mang thai </td>
        <td style="text-align:right; border-bottom:hidden"> 18,5 - 24,9 </td>
        <td style="border-bottom:hidden"> 0 điểm <input type="checkbox" id="BMITruocMangThaiBT185N249"
                name="BMITruocMangThai" {{BMITruocMangThaiBT185N249}} disabled> </td>
    </tr>
    <tr style="border-bottom:hidden">
        <td style="text-align:right" align="top"> &ge; 25,0 </td>
        <td> 1 điểm <input type="checkbox" id="BMITruocMangThaiGE25" name="BMITruocMangThai" {{BMITruocMangThaiGE25}}
                disabled> </td>
    </tr>
    <tr>
        <td style="text-align:right;">
            < 18,5 </td>
        <td> 1 điểm <input type="checkbox" id="BMITruocMangThaiLT185" name="BMITruocMangThai" {{BMITruocMangThaiLT185}}
                disabled> </td>
    </tr>
    <tr>
        <td rowspan="2"> Tốc độ tăng cân </td>
        <td style="border-bottom:hidden"> Tăng cân theo khuyến nghị </td>
        <td style="border-bottom:hidden"> 0 điểm <input type="checkbox" id="TocDoTangCanTheoKhuyenNghi"
                name="TocDoTangCan" {{TocDoTangCanTheoKhuyenNghi}} disabled> </td>
    </tr>
    <tr>
        <td> Tăng cân trên, hoặc dưới mức khuyến nghị </td>
        <td> 1 điểm <input type="checkbox" id="TocDoTangCanTrenDuoiMucKhuyenNghi" name="TocDoTangCan"
                {{TocDoTangCanTrenDuoiMucKhuyenNghi}} disabled> </td>
    </tr>
    <tr>
        <td rowspan="2"> Bệnh kèm theo </td>
        <td style="border-bottom:hidden"> Không </td>
        <td style="border-bottom:hidden"> 0 điểm <input type="checkbox" id="BenhKemTheoKhong" name="BenhKemTheo"
                {{BenhKemTheoKhong}} disabled> </td>
    </tr>
    <tr>
        <td> Tăng huyết áp, đái tháo đường, nghén nặng... </td>
        <td> 1 điểm <input type="checkbox" id="BenhKemTheoTangHuyetAp" name="BenhKemTheo" {{BenhKemTheoTangHuyetAp}}
                disabled> </td>
    </tr>
    <tr>
        <td rowspan="2" style="text-align:center;"> <b>Kết luận</b> </td>
        <td style="text-align:right;border-bottom:hidden">
            <2 Điểm </td>
        <td style="border-bottom:hidden"> Bình thường <input type="checkbox" id="KetLuanBinhThuong" name="KetLuan"
                {{KetLuanBinhThuong}} disabled> </td>
    </tr>
    <tr>
        <td style="text-align:right;"> &ge; 2 điểm </td>
        <td> Có nguy cơ về dinh dưỡng <input type="checkbox" id="KetLuanCoNguyCoDinhDuong" name="KetLuan"
                {{KetLuanCoNguyCoDinhDuong}} disabled> </td>
    </tr>
</table> <br>
<table width="100%">
    <tr>
        <td width="33%"> &nbsp; </td>
        <td width="25%"> &nbsp; </td>
        <td style="text-align:center;"> Ngày {{Ngay}} tháng {{Thang}} năm {{Nam}}<br> Người thực hiện <br> <br> <br>
            <br> <br> {{NguoiThucHien}} </td>
    </tr>
</table>
<div class="pagebreak"></div>

<table style="width: 100%;">
    <tbody>
      <tr>
            <td colspan="3" style="text-align:center; font-size:22px"><b>HƯỚNG DẪN SÀNG LỌC DINH DƯỠNG CHO PHỤ NỮ MANG THAI</b></td>
        </tr>
    </tbody>
</table> <br>


<table width="100%">
    <tr>
        <td>
            <div class="container" >
                <div class="label">1. Đối tượng đánh giá: tất cả phụ nữ mang thai nằm viện đều được sàng lọc dinh dưỡng.</div>
               <div class="label">2. Thời gian thực hiện: trong vòng 48 giờ sau nhập viện.
</div>
                 <div class="label">3. Cán bộ thực hiện: Bác sỹ điều trị.
</div>
                   <div class="label">4. Bảng mức tăng cân của bà mẹ và bào thai trong thai kỳ:
</div>
                   
            </div>
        </td>
    </tr>
    <tr><td>
    <table id="customers" style="text-align: center;width: 58%;">
    <tr>
        <td > Mức tăng cân </td>
        <td> 3 tháng đầu (quý I) </td>
        <td>3 tháng giữa (quý II) </td>
         <td>3 tháng cuối (quý III) </td>
    </tr>
     <tr>
        <td>Mẹ </td>
        <td>1 kg </td>
        <td>4 - 5 kg </td>
         <td>5 - 6 kg </td>
    </tr>
     <tr>
        <td>Bào thai</td>
        <td>0,1 kg </td>
        <td>1 kg</td>
         <td>2 kg</td>
    </tr>
</table></td></tr>
  <tr>
   <td>
            <div class="container" >
                <div class="label"> - Phụ nữ mang thai không rõ cân nặng trước khi mang thai: sử dụng BMI trong lần khám thai đầu tiên trong 3 tháng đầu thai kỳ để khuyến nghị mức tăng cân: Theo tiêu chuẩn quốc tế (FAO) mức tăng cân trung bình của phụ nữ châu á nên là 10 – 12kg.</div>
               <div class="label">- Tình trạng dinh dưỡng tốt (BMI: 18,5 – 24,9): Mức tăng cân nên đạt là 20% cân nặng trước khi mang thai.

</div>
                 <div class="label">- Tình trạng dinh dưỡng gầy (BMI <18,5): Mức tăng cân nên đạt là 25% cân nặng trước khi mang thai.

</div>
                   <div class="label">- Tình trạng dinh dưỡng thừa cân – béo phì (BMI ≥ 25): Mức tăng cân nên đạt là 15% cân nặng trước khi mang thai.

</div>
                   
            </div>
        </td>
  </tr>
</table>'
where [Name]='PhieuSangLocDinhDuongPhuSan'

Update dbo.CauHinh
Set [Value] = '2.8.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'
