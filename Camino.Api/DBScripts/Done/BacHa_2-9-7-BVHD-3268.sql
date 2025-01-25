UPDATE TemplateKhamSangLocTiemChung
SET ComponentDynamics = N'{
    "IsTrongBenhVien": true,
    "ComponentDynamics": [
        {
            "Type": 6,
            "Id": "CanNang",
            "Label": "Cân nặng",
            "SubLabel": "kg",
            "Value": null,
            "fxFlex": "10%",
            "fxFlexSm": "10%",
            "required": true,
            "min": 1,
            "max": 9999999
        },
        {
            "Type": 6,
            "Id": "ThanNhiet",
            "Label": "Thân nhiệt",
            "SubLabel": "&deg;C",
            "Value": null,
            "fxFlex": "10%",
            "fxFlexSm": "10%",
            "required": true,
            "min": 1,
            "max": 9999
        },
        {
            "Type": 10,
            "fxFlex": "70%",
            "fxFlexSm": "70%",
            "Id": "Blank",
            "Value": null
        },
        {
            "Type": 4,
            "Id": "Group1",
            "Label": "1. Sốc, phẩn ứng nặng sau lần tiêm chủng trước?",
            "fxFlex": "80%",
            "fxFlexSm": "60%",
            "groupItems": [
                {
                    "Type": 5,
                    "Id": "Group1Khong",
                    "Label": "Không",
                    "Value": true,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                },
                {
                    "Type": 5,
                    "Id": "Group1Co",
                    "Label": "Có",
                    "Value": null,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                }
            ]
        },
        {
            "Type": 4,
            "Id": "Group2",
            "Label": "2. Đang mắc bệnh cấp tính hoặc bệnh mạn tính tiến triển*",
            "fxFlex": "80%",
            "fxFlexSm": "60%",
            "groupItems": [
                {
                    "Type": 5,
                    "Id": "Group2Khong",
                    "Label": "Không",
                    "Value": true,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                },
                {
                    "Type": 5,
                    "Id": "Group2Co",
                    "Label": "Có",
                    "Value": null,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                }
            ]
        },
        {
            "Type": 4,
            "Id": "Group3",
            "Label": "3. Đang hoặc mới kết thúc đợt điều trị corticoid liều cao (tương đương prednison > 2mg/kg/ngày), hoá trị, xạ trị, gammaglobulin**",
            "fxFlex": "80%",
            "fxFlexSm": "60%",
            "groupItems": [
                {
                    "Type": 5,
                    "Id": "Group3Khong",
                    "Label": "Không",
                    "Value": true,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                },
                {
                    "Type": 5,
                    "Id": "Group3Co",
                    "Label": "Có",
                    "Value": null,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                }
            ]
        },
        {
            "Type": 4,
            "Id": "Group4",
            "Label": "4. Sốt/Hạ thân nhiệt (sốt nhiệt độ &ge; 38&deg;C; Hạ thân nhiệt, nhiệt độ &le; 35,5&deg;C",
            "fxFlex": "80%",
            "fxFlexSm": "60%",
            "groupItems": [
                {
                    "Type": 5,
                    "Id": "Group4Khong",
                    "Label": "Không",
                    "Value": true,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                },
                {
                    "Type": 5,
                    "Id": "Group4Co",
                    "Label": "Có",
                    "Value": null,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                }
            ]
        },
        {
            "Type": 4,
            "Id": "Group5",
            "Label": "5. Nghe tim bất thường***",
            "fxFlex": "80%",
            "fxFlexSm": "60%",
            "groupItems": [
                {
                    "Type": 5,
                    "Id": "Group5Khong",
                    "Label": "Không",
                    "Value": true,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                },
                {
                    "Type": 5,
                    "Id": "Group5Co",
                    "Label": "Có",
                    "Value": null,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                }
            ]
        },
        {
            "Type": 4,
            "Id": "Group6",
            "Label": "6. Nhịp thở, nghe phổi bất thường***",
            "fxFlex": "80%",
            "fxFlexSm": "60%",
            "groupItems": [
                {
                    "Type": 5,
                    "Id": "Group6Khong",
                    "Label": "Không",
                    "Value": true,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                },
                {
                    "Type": 5,
                    "Id": "Group6Co",
                    "Label": "Có",
                    "Value": null,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                }
            ]
        },
        {
            "Type": 4,
            "Id": "Group7",
            "Label": "7. Tri giác bất thường (li bì hoặc kích thích)",
            "fxFlex": "80%",
            "fxFlexSm": "60%",
            "groupItems": [
                {
                    "Type": 5,
                    "Id": "Group7Khong",
                    "Label": "Không",
                    "Value": true,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                },
                {
                    "Type": 5,
                    "Id": "Group7Co",
                    "Label": "Có",
                    "Value": null,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                }
            ]
        },
        {
            "Type": 4,
            "Id": "Group8",
            "Label": "8. Các chống chỉ định khác, nếu có ghi rõ:",
            "fxFlex": "80%",
            "fxFlexSm": "60%",
            "groupItems": [
                {
                    "Type": 5,
                    "Id": "Group8Khong",
                    "Label": "Không",
                    "Value": true,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                },
                {
                    "Type": 5,
                    "Id": "Group8Co",
                    "Label": "Có",
                    "Value": null,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                },
                {
                    "Type": 3,
                    "Id": "Group8Text",
                    "Label": "",
                    "Value": null,
                    "fxFlex": "100%",
                    "fxFlexSm": "100%",
                    "maxlength": 1000,
                    "requiredCheckbox": true
                }
            ]
        },
        {
            "Type": 1,
            "Id": "GhiChu1",
            "Label": "*: Chỉ định tiêm vắc xin nếu trẻ có bệnh nhẹ (ho, sổ mũi, tiêu chảy mức độ nhẹ... và không sốt), bú tốt, ăn tốt.",
            "Value": null,
            "fxFlex": "100%",
            "fxFlexSm": "100%",
            "IsLightText": true
        },
        {
            "Type": 1,
            "Id": "GhiChu2",
            "Label": "**: Trừ kháng huyết thanh viêm gan B.",
            "Value": null,
            "fxFlex": "100%",
            "fxFlexSm": "100%",
            "IsLightText": true
        },
        {
            "Type": 1,
            "Id": "GhiChu3",
            "Label": "***: Nếu khám chuyên khoa không cần xử trí cấp cứu thì chỉ định tiêm chủng.",
            "Value": null,
            "fxFlex": "100%",
            "fxFlexSm": "100%",
            "IsLightText": true
        },
        {
            "Type": 4,
            "Id": "GroupKhamSangLocChuyenKhoa",
            "Label": "Khám sàng lọc theo chuyên khoa:",
            "fxFlex": "20%",
            "fxFlexSm": "20%",
            "boldText": true,
            "groupItems": [
                {
                    "Type": 5,
                    "Id": "GroupKhamSangLocChuyenKhoaKhong",
                    "Label": "Không",
                    "Value": true,
                    "fxFlex": "10%",
                    "fxFlexSm": "10%"
                },
                {
                    "Type": 5,
                    "Id": "GroupKhamSangLocChuyenKhoaCo",
                    "Label": "Có",
                    "Value": null,
                    "fxFlex": "10%",
                    "fxFlexSm": "10%"
                },
                {
                    "Type": 3,
                    "Id": "GroupChuyenKhoaText",
                    "Label": "Chuyên khoa",
                    "Value": null,
                    "fxFlex": "20%",
                    "fxFlexSm": "20%",
                    "maxlength": 1000,
                    "requiredCheckbox": true
                },
                {
                    "Type": 3,
                    "Id": "LyDo",
                    "Label": "Lý do",
                    "Value": null,
                    "fxFlex": "100%",
                    "fxFlexSm": "100%",
                    "maxlength": 1000,
                    "requiredCheckbox": true
                },
                {
                    "Type": 3,
                    "Id": "KetQua",
                    "Label": "Kết quả",
                    "Value": null,
                    "fxFlex": "100%",
                    "fxFlexSm": "100%",
                    "maxlength": 1000,
                    "requiredCheckbox": true
                },
                {
                    "Type": 3,
                    "Id": "KetLuan",
                    "Label": "Kết luận",
                    "Value": null,
                    "fxFlex": "100%",
                    "fxFlexSm": "100%",
                    "maxlength": 1000,
                    "requiredCheckbox": true
                }
            ]
        }
    ]
}'
WHERE Ten = N'KhamSangLocDoiTuongTrenMotThangTuoiTrongBenhVien'

UPDATE TemplateKhamSangLocTiemChung
SET ComponentDynamics = N'{
    "IsTrongBenhVien": false,
    "ComponentDynamics": [
        {
            "Type": 6,
            "Id": "CanNang",
            "Label": "Cân nặng",
            "SubLabel": "kg",
            "Value": null,
            "fxFlex": "10%",
            "fxFlexSm": "10%",
            "required": true,
            "min": 1,
            "max": 9999999
        },
        {
            "Type": 6,
            "Id": "ThanNhiet",
            "Label": "Thân nhiệt",
            "SubLabel": "&deg;C",
            "Value": null,
            "fxFlex": "10%",
            "fxFlexSm": "10%",
            "required": true,
            "min": 1,
            "max": 9999
        },
        {
            "Type": 10,
            "fxFlex": "70%",
            "fxFlexSm": "70%",
            "Id": "Blank",
            "Value": null
        },
        {
            "Type": 4,
            "Id": "Group1",
            "Label": "1. Sốc, phẩn ứng nặng sau lần tiêm chủng trước?",
            "fxFlex": "80%",
            "fxFlexSm": "60%",
            "groupItems": [
                {
                    "Type": 5,
                    "Id": "Group1Khong",
                    "Label": "Không",
                    "Value": true,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                },
                {
                    "Type": 5,
                    "Id": "Group1Co",
                    "Label": "Có",
                    "Value": null,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                }
            ]
        },
        {
            "Type": 4,
            "Id": "Group2",
            "Label": "2. Đang mắc bệnh cấp tính hoặc bệnh mạn tính tiến triển*",
            "fxFlex": "80%",
            "fxFlexSm": "60%",
            "groupItems": [
                {
                    "Type": 5,
                    "Id": "Group2Khong",
                    "Label": "Không",
                    "Value": true,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                },
                {
                    "Type": 5,
                    "Id": "Group2Co",
                    "Label": "Có",
                    "Value": null,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                }
            ]
        },
        {
            "Type": 4,
            "Id": "Group3",
            "Label": "3. Đang hoặc mới kết thúc đợt điều trị corticoid liều cao (tương đương prednison > 2mg/kg/ngày), hoá trị, xạ trị, gammaglobulin**",
            "fxFlex": "80%",
            "fxFlexSm": "60%",
            "groupItems": [
                {
                    "Type": 5,
                    "Id": "Group3Khong",
                    "Label": "Không",
                    "Value": true,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                },
                {
                    "Type": 5,
                    "Id": "Group3Co",
                    "Label": "Có",
                    "Value": null,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                }
            ]
        },
        {
            "Type": 4,
            "Id": "Group4",
            "Label": "4. Sốt/Hạ thân nhiệt (sốt nhiệt độ &ge; 37,5&deg;C; Hạ thân nhiệt, nhiệt độ &le; 35,5&deg;C",
            "fxFlex": "80%",
            "fxFlexSm": "60%",
            "groupItems": [
                {
                    "Type": 5,
                    "Id": "Group4Khong",
                    "Label": "Không",
                    "Value": true,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                },
                {
                    "Type": 5,
                    "Id": "Group4Co",
                    "Label": "Có",
                    "Value": null,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                }
            ]
        },
        {
            "Type": 4,
            "Id": "Group5",
            "Label": "5. Nghe tim bất thường* * *",
            "fxFlex": "80%",
            "fxFlexSm": "60%",
            "groupItems": [
                {
                    "Type": 5,
                    "Id": "Group5Khong",
                    "Label": "Không",
                    "Value": true,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                },
                {
                    "Type": 5,
                    "Id": "Group5Co",
                    "Label": "Có",
                    "Value": null,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                }
            ]
        },
        {
            "Type": 4,
            "Id": "Group6",
            "Label": "6. Nhịp thở, nghe phổi bất thường",
            "fxFlex": "80%",
            "fxFlexSm": "60%",
            "groupItems": [
                {
                    "Type": 5,
                    "Id": "Group6Khong",
                    "Label": "Không",
                    "Value": true,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                },
                {
                    "Type": 5,
                    "Id": "Group6Co",
                    "Label": "Có",
                    "Value": null,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                }
            ]
        },
        {
            "Type": 4,
            "Id": "Group7",
            "Label": "7. Tri giác bất thường (li bì hoặc kích thích)",
            "fxFlex": "80%",
            "fxFlexSm": "60%",
            "groupItems": [
                {
                    "Type": 5,
                    "Id": "Group7Khong",
                    "Label": "Không",
                    "Value": true,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                },
                {
                    "Type": 5,
                    "Id": "Group7Co",
                    "Label": "Có",
                    "Value": null,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                }
            ]
        },
        {
            "Type": 4,
            "Id": "Group8",
            "Label": "8. Cân nặng < 2000g",
            "fxFlex": "80%",
            "fxFlexSm": "60%",
            "groupItems": [
                {
                    "Type": 5,
                    "Id": "Group8Khong",
                    "Label": "Không",
                    "Value": true,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                },
                {
                    "Type": 5,
                    "Id": "Group8Co",
                    "Label": "Có",
                    "Value": null,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                }
            ]
        },
        {
            "Type": 4,
            "Id": "Group9",
            "Label": "9. Các chống chỉ định/tạm hoãn khác, nếu có ghi rõ:",
            "fxFlex": "80%",
            "fxFlexSm": "60%",
            "groupItems": [
                {
                    "Type": 5,
                    "Id": "Group9Khong",
                    "Label": "Không",
                    "Value": true,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                },
                {
                    "Type": 5,
                    "Id": "Group9Co",
                    "Label": "Có",
                    "Value": null,
                    "fxFlex": "10%",
                    "fxFlexSm": "20%"
                },
                {
                    "Type": 3,
                    "Id": "Group9Text",
                    "Label": "",
                    "Value": null,
                    "fxFlex": "100%",
                    "fxFlexSm": "100%",
                    "maxlength": 1000,
                    "requiredCheckbox": true
                }
            ]
        },
        {
            "Type": 1,
            "Id": "GhiChu1",
            "Label": "*: Không hoãn tiêm vắc xin đối với trẻ có bệnh nhẹ (ho, sổ mũi, tiêu chảy mức độ nhẹ... và không sốt), bú tốt, ăn tốt.",
            "Value": null,
            "fxFlex": "100%",
            "fxFlexSm": "100%",
            "IsLightText": true
        },
        {
            "Type": 1,
            "Id": "GhiChu2",
            "Label": "**: Trừ kháng huyết thanh viêm gan B. Tiêu chuẩn này chỉ áp dụng với vắc xin sống giảm độc lực.",
            "Value": null,
            "fxFlex": "100%",
            "fxFlexSm": "100%",
            "IsLightText": true
        }
    ]
}'
WHERE Ten = N'KhamSangLocDoiTuongTrenMotThangTuoiNgoaiBenhVien'

UPDATE Template
SET Body = N'    <style>      
        table, th, td {          
            border-collapse: collapse;          
            font-family: Times New Roman;          
            font-size: medium;          
        } 

        th, td {          
            padding: 1px;      
        }
        
        .breakword {     
            word-break: break-all;
        }
        
        .container {          
            width: 100%;          
            display: table;      
        }    
        
        .container-flex {          
            width: 100%;          
            display: flex;      
        }   
        
        .container .label {          
            width: max-content;      
        } 
        
        .container-flex .label {          
            width: max-content;  
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

        .container-flex .value {          
            display: table-cell;          
            width: 100%;          
            height: 100%;          
            vertical-align: top;          
            position: relative;          
            box-sizing: border-box;          
            border-bottom: 1px dotted black;      
        }

        .container-flex {          
            width: 100%;          
            display: flex;      
        }   
        
        #table-thong-tin, #table-ket-luan, #table-kham-sang-loc-chuyen-khoa, #table-nguoi-thuc-hien {
            width: 100%;
        } 

        #table-ket-qua {
            width: 100%;
            border-collapse: collapse;
        }

        #table-ket-qua tr {
            border: 1px solid black;
        }

        #table-ket-qua td {
            border: 1px solid black;
            padding: 0.2rem !important;
        }
        
        .cell-6 {
            width: 60%;
        }

        .cell-4 {
            width: 40%;
        }

        .text-center {
            text-align: center;
        }

        .float-right {
            float: right;
        }

        .font-small {
            font-size: small;
        }

        .clear-border {
            border: 0px !important;
        }

        span.square {
            vertical-align: bottom;
            border: solid 1px;
            width: 20px;
            height: 20px;
            font-weight: normal;
            display: inline-block;
            text-align: center;
        }

        .remove-border-bottom {
            border-bottom: none !important;
        }
    </style>  

    <div>         
        <table style="width: 100%;">          
            <tbody>              
                <tr>                  
                    <td style="width: 33.33%; font-size: large;text-align: center;">           
                        <img src="{{ImageSrc}}" style="width: 100%; height: 100px;">
                    </td>                       
                    <td colspan="2" class="text-center" style="width: 66.66%;font-size: large;">                      
                        <b>CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM</b>
                        <br>
                        <b>Độc lập - Tự do - Hạnh phúc</b>        
                        <hr style="width: 20%;border-bottom: 2px solid black;border-left: none;border-top: none;border-right: none;">                  
                    </td>              
                </tr>
                <tr>
                    <td colspan="2"></td>
                    <td style="width: 33.33%;" class="text-center font-small">
                        {{HoTen}}
                        <br>
                        <img
                            style="margin: 0 auto; height: 200%"
                            src="data:image/png;base64,{{Barcode}}"
                            alt="barcode"
                        />
                        <br>
                        {{MaTiepNhan}}
                    </td>
                </tr>         
                <tr>                  
                    <td colspan="3" style="font-size: 1.25rem;text-align: center;">                      
                        <br>                      
                        <b>BẢNG KIỂM TRƯỚC TIÊM CHỦNG ĐỐI VỚI ĐỐI TƯỢNG &ge; 1 THÁNG TUỔI TẠI CÁC CƠ SỞ TIÊM CHỦNG THUỘC BỆNH VIỆN</b>                  
                    </td>              
                </tr>          
            </tbody>      
        </table>

        <table id="table-thong-tin">
            <tbody>
                <tr>
                    <td colspan="2">
                        <div class="container">
                            <div class="label">Họ và tên trẻ:&nbsp;</div>
                            <div class="value remove-border-bottom">{{HoTen}}</div>
                            <div class="label" style="display: flex;">
                                &nbsp;           
                                Nam        
                                <span class="square">{{GioiTinhNam}}</span>  
                                &nbsp;&nbsp;           
                                Nữ           
                                <span class="square">{{GioiTinhNu}}</span>
                            </div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td style="width: 50%;">
                        <div class="container">
                            <div class="label">Tuổi:&nbsp;</div>
                            <div class="value remove-border-bottom">{{Tuoi}}</div>
                        </div>
                    </td>
                    <td style="width: 50%;">
                        <div class="container" style="width: fit-content;">
                            <div class="label">sinh ngày&nbsp;</div>
                            <div class="value remove-border-bottom" style="width:fit-content">{{NgaySinh}}&nbsp;</div>
                            <div class="label">tháng&nbsp;</div>
                            <div class="value remove-border-bottom" style="width:fit-content">{{ThangSinh}}&nbsp;</div>
                            <div class="label">năm&nbsp;</div>
                            <div class="value remove-border-bottom" style="width:fit-content">{{NamSinh}}</div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div class="container">
                            <div class="label">Địa chỉ:&nbsp;</div>
                            <div class="value breakword remove-border-bottom">{{DiaChi}}</div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div class="container">
                            <div class="label">Họ tên bố/mẹ:&nbsp;</div>
                            <div class="value remove-border-bottom">{{HoTenBoMe}}</div>
                        </div>
                    </td>
                    <td>
                        <div class="container">
                            <div class="label">Điện thoại:&nbsp;</div>
                            <div class="value remove-border-bottom">{{DienThoaiBoMe}}</div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div class="container-flex">
                            <div class="label">Cân nặng:&nbsp;</div>
                            <div class="value text-center remove-border-bottom" style="width:fit-content">{{CanNang}}</div>
                            <div>g</div>
                        </div>
                    </td>
                    <td>
                        <div class="container-flex">
                            <div class="label">Thân nhiệt:&nbsp;</div>
                            <div class="value text-center remove-border-bottom" style="width:fit-content">{{ThanNhiet}}</div>
                            <div>&deg;C</div>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>

        <table id="table-ket-qua">
            <tbody>
                <tr class="clear-border">
                    <td colspan="3" class="clear-border">
                        <b>I. Khám sàng lọc chung:</b>
                    </td>
                </tr>
                <tr>
                    <td style="width: 70%;">
                        1. Sốc, phẩn ứng nặng sau lần tiêm chủng trước
                    </td>
                    <td style="width: 15%; text-align: center;">
                        <span class="square">{{Group1Khong}}</span>
                        <label for="khong">Không</label>
                    </td>
                    <td style="width: 15%; text-align: center;">
                        <span class="square">{{Group1Co}}</span>
                        <label for="co">Có</label>
                    </td>
                </tr>
                <tr>
                    <td style="width: 70%;">
                        2. Đang mắc bệnh cấp tính hoặc bệnh mạn tính tiến triển*
                    </td>
                    <td style="width: 15%; text-align: center;">
                        <span class="square">{{Group2Khong}}</span>
                        <label for="khong">Không</label>
                    </td>
                    <td style="width: 15%; text-align: center;">
                        <span class="square">{{Group2Co}}</span>
                        <label for="co">Có</label>
                    </td>
                </tr>
                <tr>
                    <td style="width: 70%;">
                        3. Đang hoặc mới kết thúc đợt điều trị corticoid liều cao (tương đương prednison > 2mg/kg/ngày), hoá trị, xạ trị, gammaglobulin**
                    </td>
                    <td style="width: 15%; text-align: center;">
                        <span class="square">{{Group3Khong}}</span>
                        <label for="khong">Không</label>
                    </td>
                    <td style="width: 15%; text-align: center;">
                        <span class="square">{{Group3Co}}</span>
                        <label for="co">Có</label>
                    </td>
                </tr>
                <tr>
                    <td style="width: 70%;">
                        4. Sốt/Hạ thân nhiệt (sốt nhiệt độ &ge; 38&deg;C; Hạ thân nhiệt, nhiệt độ &le; 35,5&deg;C)
                    </td>
                    <td style="width: 15%; text-align: center;">
                        <span class="square">{{Group4Khong}}</span>
                        <label for="khong">Không</label>
                    </td>
                    <td style="width: 15%; text-align: center;">
                        <span class="square">{{Group4Co}}</span>
                        <label for="co">Có</label>
                    </td>
                </tr>
                <tr>
                    <td style="width: 70%;">
                        5. Nghe tim bất thường***
                    </td>
                    <td style="width: 15%; text-align: center;">
                        <span class="square">{{Group5Khong}}</span>
                        <label for="khong">Không</label>
                    </td>
                    <td style="width: 15%; text-align: center;">
                        <span class="square">{{Group5Co}}</span>
                        <label for="co">Có</label>
                    </td>
                </tr>
                <tr>
                    <td style="width: 70%;">
                        6. Nhịp thở, nghe phổi bất thường
                    </td>
                    <td style="width: 15%; text-align: center;">
                        <span class="square">{{Group6Khong}}</span>
                        <label for="khong">Không</label>
                    </td>
                    <td style="width: 15%; text-align: center;">
                        <span class="square">{{Group6Co}}</span>
                        <label for="co">Có</label>
                    </td>
                </tr>
                <tr>
                    <td style="width: 70%;">
                        7. Tri giác bất thường (li bì hoặc kích thích)
                    </td>
                    <td style="width: 15%; text-align: center;">
                        <span class="square">{{Group7Khong}}</span>
                        <label for="khong">Không</label>
                    </td>
                    <td style="width: 15%; text-align: center;">
                        <span class="square">{{Group7Co}}</span>
                        <label for="co">Có</label>
                    </td>
                </tr>
                <tr>
                    <td style="width: 70%;">
                        8. Các chống chỉ định khác, nếu có ghi rõ
                        <br>
                        <span style="display: block; border-bottom: 1px dotted black;">{{Group8Text}}</span>
                    </td>
                    <td style="width: 15%; text-align: center;">
                        <span class="square">{{Group8Khong}}</span>
                        <label for="khong">Không</label>
                    </td>
                    <td style="width: 15%; text-align: center;">
                        <span class="square">{{Group8Co}}</span>
                        <label for="co">Có</label>
                    </td>
                </tr>
                <tr class="clear-border">
                    <td class="clear-border" colspan="3">
                        <span style="font-size: small;">
                            <i>*: Chỉ định tiêm vắc xin nếu trẻ có bệnh nhẹ (ho, sổ mũi, tiêu chảy mức độ nhẹ... và không sốt), bú tốt, ăn tốt.</i>
                        </span>
                    </td>
                </tr>
                <tr class="clear-border">
                    <td class="clear-border" colspan="3">
                        <span style="font-size: small;">
                            <i>**: Trừ kháng huyết thanh viêm gan B.</i>
                        </span>
                    </td>
                </tr>
                <tr class="clear-border">
                    <td class="clear-border" colspan="3">
                        <span style="font-size: small;">
                            <i>***: Nếu khám chuyển khoa không cần xử trí cấp cứu thì chỉ định tiêm chủng.</i>
                        </span>
                    </td>
                </tr>
            </tbody>
        </table>

        <table id="table-kham-sang-loc-chuyen-khoa">
            <tbody>
                <tr>
                    <td style="width: 50%;">
                        <div class="container">
                            <div class="label">
                                <b>_ Khám sàng lọc theo chuyên khoa</b>
                                &nbsp;           
                                Không
                                <span class="square">{{GroupKhamSangLocChuyenKhoaKhong}}</span>           
                                &nbsp;&nbsp;           
                                Có           
                                <span class="square">{{GroupKhamSangLocChuyenKhoaCo}}</span>
                            </div>
                        </div>
                    </td>
                    <td>
                        <div class="container">
                            <div class="label">chuyên khoa:&nbsp;</div>       
                            <div class="value">{{GroupChuyenKhoaText}}</div>    
                        </div>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div class="container">
                            <div class="label">+ Lý do:&nbsp;</div>
                            <div class="value" style="width:95%">{{LyDo}}</div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div class="container">
                            <div class="label">+ Kết quả:&nbsp;</div>
                            <div class="value" style="width:95%">{{KetQua}}</div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div class="container">
                            <div class="label">+ Kết luận:&nbsp;</div>
                            <div class="value" style="width:95%">{{KetLuan}}</div>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>

        <table id="table-ket-luan">
            <tr>
                <td>
                    <b>II. Kết luận</b>
                </td>
            </tr>
            <tr>
                <td>
                    <b>- Đủ điều kiện tiêm chủng ngay</b> (Tất cả đều <b>KHÔNG</b> có điểm bất thường)
                    <span class="square">{{KhongCoBatThuong}}</span>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <div class="container-flex">
                        - Loại vắc xin tiêm chủng lần này:&nbsp;
                        <span style="display: block; width: 50%; border-bottom: 1px dotted black; ">{{TenVacxin}}</span>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    - Chống chỉ định tiêm chủng (Khi <b>CÓ</b> điểm bất thường tại mục 1, 8)
                    <span class="square">{{ChongChiDinh}}</span>
                </td>
            </tr>
            <tr>
                <td>
                    - Tạm hoãn tiêm chủng (Khi <b>CÓ</b> bất kỳ một điểm bất thường tại các mục 2,3,4,5,6,7) 
                    <span class="square">{{TamHoan}}</span>
                </td>
            </tr>
        </table>

        <table id="table-nguoi-thuc-hien">
            <tr>
                <td style="width: 50%;"></td>
                <td class="text-center" style="width: 50%;">
                    Hồi {{Hoi}}, {{NgayThangHienTai}}
                    <br>
                    Người thực hiện sàng lọc
                    <br>
                    (ký, ghi rõ họ và tên)
                    <br><br><br><br>
                    <span style="text-transform: uppercase;">{{HoTenBacSi}}</span>
                </td>
            </tr>
        </table>
    </div>'
WHERE [Name] = N'BanKiemTruocTiemChungTrongBenhVienTren1Thang'

UPDATE Template
SET Body = N'<style>      
    table, th, td {          
        border-collapse: collapse;          
        font-family: Times New Roman;          
        font-size: medium;          
    } 

    th, td {          
        padding: 1px;      
    }
    
    .breakword {     
        word-break: break-all;
    }
    
    .container {          
        width: 100%;          
        display: table;      
    }    
    
    .container-flex {          
        width: 100%;          
        display: flex;      
    }   
    
    .container .label {          
        width: max-content;      
    } 
    
    .container-flex .label {          
        width: max-content;  
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

    .container-flex .value {          
        display: table-cell;          
        width: 100%;          
        height: 100%;          
        vertical-align: top;          
        position: relative;          
        box-sizing: border-box;          
        border-bottom: 1px dotted black;      
    }

    .container-flex {          
        width: 100%;          
        display: flex;      
    }   
    
    #table-thong-tin, #table-ket-luan, #table-kham-sang-loc-chuyen-khoa, #table-nguoi-thuc-hien {
        width: 100%;
    } 

    #table-ket-qua {
        width: 100%;
        border-collapse: collapse;
    }

    #table-ket-qua tr {
        border: 1px solid black;
    }

    #table-ket-qua td {
        border: 1px solid black;
        padding: 0.2rem !important;
    }
    
    .cell-6 {
        width: 60%;
    }

    .cell-4 {
        width: 40%;
    }

    .text-center {
        text-align: center;
    }

    .float-right {
        float: right;
    }

    .font-small {
        font-size: small;
    }

    .clear-border {
        border: 0px !important;
    }

    span.square {
        vertical-align: bottom;
        border: solid 1px;
        width: 20px;
        height: 20px;
        font-weight: normal;
        display: inline-block;
        text-align: center;
    }

    .remove-border-bottom {
        border-bottom: none !important;
    }
</style>  

<div>         
    <table style="width: 100%;">          
        <tbody>              
            <tr>                  
                <td style="width: 33.33%; font-size: large;text-align: center;">
                    <img src="{{ImageSrc}}" style="width: 100%; height: 100px;">
                </td>                       
                <td colspan="2" class="text-center" style="width: 66.66%;font-size: large;">                      
                    <b>CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM</b>
                    <br>
                    <b>Độc lập - Tự do - Hạnh phúc</b>        
                    <hr style="width: 20%;border-bottom: 2px solid black;border-left: none;border-top: none;border-right: none;">                  
                </td>              
            </tr>
            <tr>
                <td colspan="2"></td>
                <td style="width: 33.33%;" class="text-center font-small">
                    {{HoTen}}
                    <br>
                    <img
                        style="margin: 0 auto; height: 200%"
                        src="data:image/png;base64,{{Barcode}}"
                        alt="barcode"
                    />
                    <br>
                    {{MaTiepNhan}}
                </td>
            </tr>         
            <tr>                  
                <td colspan="3" style="font-size: 1.25rem;text-align: center;">                      
                    <br>                      
                    <b>BẢNG KIỂM TRƯỚC TIÊM CHỦNG ĐỐI VỚI ĐỐI TƯỢNG &ge; 1 THÁNG TUỔI TẠI CÁC CƠ SỞ TIÊM CHỦNG NGOÀI BỆNH VIỆN</b>                  
                </td>              
            </tr>          
        </tbody>      
    </table>

    <br>

    <table id="table-thong-tin">
        <tbody>
            <tr>
                <td colspan="2">
                    <div class="container">
                        <div class="label">Họ và tên trẻ:&nbsp;</div>
                        <div class="value remove-border-bottom">{{HoTen}}</div>
                        <div class="label" style="display: flex;">
                            &nbsp;           
                            Nam      
                            <span class="square">{{GioiTinhNam}}</span>       
                            &nbsp;&nbsp;           
                            Nữ           
                            <span class="square">{{GioiTinhNu}}</span>  
                        </div>
                    </div>
                </td>
            </tr>
            <tr>
                <td style="width: 50%;">
                    <div class="container">
                        <div class="label">Tuổi:&nbsp;</div>
                        <div class="value remove-border-bottom">{{Tuoi}}</div>
                    </div>
                </td>
                <td style="width: 50%;">
                    <div class="container" style="width: fit-content;">
                        <div class="label">sinh ngày&nbsp;</div>
                        <div class="value remove-border-bottom" style="width:fit-content">{{NgaySinh}}&nbsp;</div>
                        <div class="label">tháng&nbsp;</div>
                        <div class="value remove-border-bottom" style="width:fit-content">{{ThangSinh}}&nbsp;</div>
                        <div class="label">năm&nbsp;</div>
                        <div class="value remove-border-bottom" style="width:fit-content">{{NamSinh}}</div>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <div class="container">
                        <div class="label">Địa chỉ:&nbsp;</div>
                        <div class="value breakword remove-border-bottom">{{DiaChi}}</div>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <div class="container">
                        <div class="label">Họ tên bố/mẹ:&nbsp;</div>
                        <div class="value remove-border-bottom">{{HoTenBoMe}}</div>
                    </div>
                </td>
                <td>
                    <div class="container">
                        <div class="label">Điện thoại:&nbsp;</div>
                        <div class="value remove-border-bottom">{{DienThoaiBoMe}}</div>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <div class="container-flex">
                        <div class="label">Cân nặng:&nbsp;</div>
                        <div class="value text-center remove-border-bottom" style="width:fit-content">{{CanNang}}</div>
                        <div>g</div>
                    </div>
                </td>
                <td>
                    <div class="container-flex">
                        <div class="label">Thân nhiệt:&nbsp;</div>
                        <div class="value text-center remove-border-bottom" style="width:fit-content">{{ThanNhiet}}</div>
                        <div>&deg;C</div>
                    </div>
                </td>
            </tr>
        </tbody>
    </table>

    <table id="table-ket-qua">
        <tbody>
            <tr class="clear-border">
                <td colspan="3" class="clear-border">
                    <b>I. Khám sàng lọc:</b>
                </td>
            </tr>
            <tr>
                <td style="width: 70%;">
                    1. Sốc, phẩn ứng nặng sau lần tiêm chủng trước
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group1Khong}}</span>  
                    <label for="khong">Không</label>
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group1Co}}</span>  
                    <label for="co">Có</label>
                </td>
            </tr>
            <tr>
                <td style="width: 70%;">
                    2. Đang mắc bệnh cấp tính hoặc bệnh mạn tính tiến triển*
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group2Khong}}</span>  
                    <label for="khong">Không</label>
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group2Co}}</span>  
                    <label for="co">Có</label>
                </td>
            </tr>
            <tr>
                <td style="width: 70%;">
                    3. Đang hoặc mới kết thúc đợt điều trị corticoid liều cao (tương đương prednison > 2mg/kg/ngày), hoá trị, xạ trị, gammaglobulin**
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group3Khong}}</span>  
                    <label for="khong">Không</label>
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group3Co}}</span>  
                    <label for="co">Có</label>
                </td>
            </tr>
            <tr>
                <td style="width: 70%;">
                    4. Sốt/Hạ thân nhiệt (sốt nhiệt độ &ge; 37,5&deg;C; Hạ thân nhiệt, nhiệt độ &le; 35,5&deg;C)
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group4Khong}}</span>  
                    <label for="khong">Không</label>
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group4Co}}</span>  
                    <label for="co">Có</label>
                </td>
            </tr>
            <tr>
                <td style="width: 70%;">
                    5. Nghe tim bất thường***
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group5Khong}}</span>  
                    <label for="khong">Không</label>
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group5Co}}</span>  
                    <label for="co">Có</label>
                </td>
            </tr>
            <tr>
                <td style="width: 70%;">
                    6. Nhịp thở, nghe phổi bất thường
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group6Khong}}</span>  
                    <label for="khong">Không</label>
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group6Co}}</span>  
                    <label for="co">Có</label>
                </td>
            </tr>
            <tr>
                <td style="width: 70%;">
                    7. Tri giác bất thường (li bì hoặc kích thích)
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group7Khong}}</span>  
                    <label for="khong">Không</label>
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group7Co}}</span>  
                    <label for="co">Có</label>
                </td>
            </tr>
            <tr>
                <td style="width: 70%;">
                    8. Cân nặng < 2000g
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group8Khong}}</span>  
                    <label for="khong">Không</label>
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group8Co}}</span>  
                    <label for="co">Có</label>
                </td>
            </tr>
            <tr>
                <td style="width: 70%;">
                    9. Các chống chỉ định/tạm hoãn khác, nếu có ghi rõ:
                    <br>
                    <span style="display: block; border-bottom: 1px dotted black;">{{Group9Text}}</span>
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group9Khong}}</span>  
                    <label for="khong">Không</label>
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group9Co}}</span>  
                    <label for="co">Có</label>
                </td>
            </tr>
            <tr class="clear-border">
                <td class="clear-border" colspan="3">
                    <span style="font-size: small;">
                        <i>*: Không hoãn tiêm vắc xin đối với trẻ có bệnh nhẹ (ho, sổ mũi, tiêu chảy mức độ nhẹ... và không sốt), bú tốt, ăn tốt.</i>
                    </span>
                </td>
            </tr>
            <tr class="clear-border">
                <td class="clear-border" colspan="3">
                    <span style="font-size: small;">
                        <i>**: Trừ kháng huyết thanh viêm gan B. Tiêu chuẩn này chỉ áp dụng với vắc xin sống giảm độc lực.</i>
                    </span>
                </td>
            </tr>
        </tbody>
    </table>

    <table id="table-ket-luan">
        <tr>
            <td>
                <b>II. Kết luận</b>
            </td>
        </tr>
        <tr>
            <td>
                <b>- Đủ điều kiện tiêm chủng ngay</b> (Tất cả đều <b>KHÔNG</b> có điểm bất thường)
                <span class="square">{{KhongCoBatThuong}}</span>  
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <div class="container-flex">
                    - Loại vắc xin tiêm chủng lần này:&nbsp;
                    <span style="display: block; width: 50%; border-bottom: 1px dotted black; ">{{TenVacxin}}</span>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                - Chống chỉ định tiêm chủng (Khi <b>CÓ</b> điểm bất thường tại mục 1,9)
                <span class="square">{{ChongChiDinh}}</span>  
            </td>
        </tr>
        <tr>
            <td>
                - Tạm hoãn tiêm chủng (Khi <b>CÓ</b> bất kỳ một điểm bất thường tại các mục 2,3,4,5,6,7,8) 
                <span class="square">{{TamHoan}}</span>  
            </td>
        </tr>
        <tr>
            <td>
                <div class="container">
                    <div class="label">
                        Đề nghị khám sàng lọc tại bệnh viện
                        &nbsp;           
                        Không           
                        <span class="square">{{KhongKhamSangLoc}}</span>  
                        &nbsp;&nbsp;           
                        Có           
                        <span class="square">{{CoKhamSangLoc}}</span>  
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="container">
                    <div class="label">+ Lý do:&nbsp;</div>
                    <div class="value breakword">{{LyDoKhamSangLoc}}</div>
                </div>
            </td>
        </tr>
    </table>

    <br>

    <table id="table-nguoi-thuc-hien">
        <tr>
            <td style="width: 50%;"></td>
            <td class="text-center" style="width: 50%;">
                Hồi {{Hoi}}, {{NgayThangHienTai}}
                <br>
                Người thực hiện sàng lọc
                <br>
                (ký, ghi rõ họ và tên)
                <br><br><br><br>
                <span style="text-transform: uppercase;">{{HoTenBacSi}}</span>
            </td>
        </tr>
    </table>
</div>'
WHERE [Name] = N'BanKiemTruocTiemChungNgoaiBenhVienTren1Thang'

UPDATE Template
SET Body = N'<style>      
    table, th, td {          
        border-collapse: collapse;          
        font-family: Times New Roman;          
        font-size: medium;          
    } 

    th, td {          
        padding: 1px;      
    }
    
    .breakword {     
        word-break: break-all;
    }
    
    .container {          
        width: 100%;          
        display: table;      
    }    
    
    .container-flex {          
        width: 100%;          
        display: flex;      
    }   
    
    .container .label {          
        width: max-content;      
    } 
    
    .container-flex .label {          
        width: max-content;  
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

    .container-flex .value {          
        display: table-cell;          
        width: 100%;          
        height: 100%;          
        vertical-align: top;          
        position: relative;          
        box-sizing: border-box;          
        border-bottom: 1px dotted black;      
    }

    .container-flex {          
        width: 100%;          
        display: flex;      
    }   
    
    #table-thong-tin, #table-ket-luan, #table-kham-sang-loc-chuyen-khoa, #table-nguoi-thuc-hien {
        width: 100%;
    } 

    #table-ket-qua {
        width: 100%;
        border-collapse: collapse;
    }

    #table-ket-qua tr {
        border: 1px solid black;
    }

    #table-ket-qua td {
        border: 1px solid black;
        padding: 0.2rem !important;
    }
    
    .cell-6 {
        width: 60%;
    }

    .cell-4 {
        width: 40%;
    }

    .text-center {
        text-align: center;
    }

    .float-right {
        float: right;
    }

    .font-small {
        font-size: small;
    }

    .clear-border {
        border: 0px !important;
    }

    span.square {
        vertical-align: bottom;
        border: solid 1px;
        width: 20px;
        height: 20px;
        font-weight: normal;
        display: inline-block;
        text-align: center;
    }

    .remove-border-bottom {
        border-bottom: none !important;
    }
</style>  

<div>         
    <table style="width: 100%;">          
        <tbody>              
            <tr>                  
                <td style="width: 33.33%; font-size: large;text-align: center;">      
                    <img src="{{ImageSrc}}" style="width: 100%; height: 100px;">
                </td>                       
                <td colspan="2" class="text-center" style="width: 66.66%;font-size: large;">                      
                    <b>CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM</b>
                    <br>
                    <b>Độc lập - Tự do - Hạnh phúc</b>        
                    <hr style="width: 20%;border-bottom: 2px solid black;border-left: none;border-top: none;border-right: none;">                  
                </td>              
            </tr>
            <tr>
                <td colspan="2"></td>
                <td style="width: 33.33%;" class="text-center font-small">
                    {{HoTen}}
                    <br>
                    <img
                        style="margin: 0 auto; height: 200%"
                        src="data:image/png;base64,{{Barcode}}"
                        alt="barcode"
                    />
                    <br>
                    {{MaTiepNhan}}
                </td>
            </tr>         
            <tr>                  
                <td colspan="3" style="font-size: 1.25rem;text-align: center;">                      
                    <br>                      
                    <b>BẢNG KIỂM TRƯỚC TIÊM CHỦNG ĐỐi VỚI TRẺ SƠ SINH TẠI CÁC CƠ SỞ TIÊM CHỦNG THUỘC BỆNH VIỆN</b>                  
                </td>              
            </tr>          
        </tbody>      
    </table>

    <table id="table-thong-tin">
        <tbody>
            <tr>
                <td colspan="2">
                    <div class="container">
                        <div class="label">Họ và tên trẻ:&nbsp;</div>
                        <div class="value remove-border-bottom">{{HoTen}}</div>
                        <div class="label" style="display: flex;">
                            &nbsp;           
                            Nam
                            <span class="square">{{GioiTinhNam}}</span>            
                            &nbsp;&nbsp;           
                            Nữ
                            <span class="square">{{GioiTinhNu}}</span>            
                        </div>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <div style="width: 50%;">
                        <div class="container">
                            <div class="label">Sinh:&nbsp;</div>
                            <div class="value text-center remove-border-bottom" style="width: fit-content;">{{GioSinh}}&nbsp;</div>
                            <div class="label">giờ&nbsp;</div>
                            <div class="value text-center remove-border-bottom" style="width: fit-content;">{{PhutSinh}}&nbsp;</div>
                            <div class="label">ngày&nbsp;</div>
                            <div class="value text-center remove-border-bottom" style="width: fit-content;">{{NgaySinh}}&nbsp;</div>
                            <div class="label">tháng&nbsp;</div>
                            <div class="value text-center remove-border-bottom" style="width: fit-content;">{{ThangSinh}}&nbsp;</div>
                            <div class="label">năm&nbsp;</div>
                            <div class="value text-center remove-border-bottom" style="width: fit-content;">{{NamSinh}}&nbsp;</div>
                        </div>
                    </div>
                </td>
                <td>
                    <div class="container" style="width: fit-content;">
                        <div class="label">Tuổi thai khi sinh:&nbsp;</div>
                        <div class="value remove-border-bottom" style="width: fit-content;">{{TuoiThaiKhiSinh}}</div>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <div class="container">
                        <div class="label">Địa chỉ:&nbsp;</div>
                        <div class="value breakword remove-border-bottom">{{DiaChi}}</div>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <div class="container">
                        <div class="label">Họ tên bố/mẹ:&nbsp;</div>
                        <div class="value remove-border-bottom">{{HoTenBoMe}}</div>
                    </div>
                </td>
                <td>
                    <div class="container">
                        <div class="label">Điện thoại:&nbsp;</div>
                        <div class="value remove-border-bottom">{{DienThoaiBoMe}}</div>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <div class="container-flex">
                        <div class="label">Cân nặng:&nbsp;</div>
                        <div class="value text-center remove-border-bottom" style="width:fit-content">{{CanNang}}</div>
                        <div>g</div>
                    </div>
                </td>
                <td>
                    <div class="container-flex">
                        <div class="label">Thân nhiệt:&nbsp;</div>
                        <div class="value text-center remove-border-bottom" style="width:fit-content">{{ThanNhiet}}</div>
                        <div>&deg;C</div>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <div class="container-flex">
                        <div class="label" style="display: flex;">
                            Mẹ đã xét nghiệm HbsAg:
                            Không  
                            &nbsp;
                            <span class="square">{{GroupXetNghiemHbsAgKhong}}</span>          
                            &nbsp;&nbsp;           
                            Có      
                            &nbsp; 
                            <span class="square">{{GroupXetNghiemHbsAgCo}}</span>     
                        </div>
                    </div>
                </td>
                <td>
                    <div class="container">
                        <div class="label" style="display: flex;">
                            Kết quả:    
                            Dương tính  
                            &nbsp;
                            <span class="square">{{GroupKetQuaHbsAgCoChild}}</span>          
                            &nbsp;&nbsp;           
                            Âm tính   
                            &nbsp;    
                            <span class="square">{{GroupKetQuaHbsAgKhongChild}}</span>     
                        </div>
                    </div>
                </td>
            </tr>
        </tbody>
    </table>

    <table id="table-ket-qua">
        <tbody>
            <tr class="clear-border">
                <td colspan="3" class="clear-border">
                    <b>I. Khám sàng lọc:</b>
                </td>
            </tr>
            <tr class="clear-border">
                <td colspan="3" class="clear-border">
                    Các dấu hiệu hiện tại:
                </td>
            </tr>
            <tr>
                <td style="width: 70%;">
                    1. Tình trạng sức khoẻ chưa ổn định
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group1Khong}}</span> 
                    <label for="khong">Không</label>
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group1Co}}</span> 
                    <label for="co">Có</label>
                </td>
            </tr>
            <tr>
                <td style="width: 70%;">
                    2. Sốt/Hạ thân nhiệt (Sốt nhiệt độ &ge; 37,5&deg;C; Hạ thân nhiệt: nhiệt độ &le; 35,5&deg;C)
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group2Khong}}</span> 
                    <label for="khong">Không</label>
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group2Co}}</span> 
                    <label for="co">Có</label>
                </td>
            </tr>
            <tr>
                <td style="width: 70%;">
                    3. Khóc bé hoặc không khóc được
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group3Khong}}</span> 
                    <label for="khong">Không</label>
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group3Co}}</span> 
                    <label for="co">Có</label>
                </td>
            </tr>
            <tr>
                <td style="width: 70%;">
                    4. Da, môi không hồng
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group4Khong}}</span> 
                    <label for="khong">Không</label>
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group4Co}}</span> 
                    <label for="co">Có</label>
                </td>
            </tr>
            <tr>
                <td style="width: 70%;">
                    5. Bú kém hoặc bỏ bú
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group5Khong}}</span> 
                    <label for="khong">Không</label>
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group5Co}}</span> 
                    <label for="co">Có</label>
                </td>
            </tr>
            <tr>
                <td style="width: 70%;">
                    6. Tuổi thai < 28 tuần
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group6Khong}}</span> 
                    <label for="khong">Không</label>
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group6Co}}</span> 
                    <label for="co">Có</label>
                </td>
            </tr>
            <tr>
                <td style="width: 70%;">
                    7. Trẻ < 34 tuần tuổi *
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group7Khong}}</span> 
                    <label for="khong">Không</label>
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group7Co}}</span> 
                    <label for="co">Có</label>
                </td>
            </tr>
            <tr>
                <td style="width: 70%;">
                    8. Trẻ < 2000g, mẹ có HBsAg (-)
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group8Khong}}</span> 
                    <label for="khong">Không</label>
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group8Co}}</span> 
                    <label for="co">Có</label>
                </td>
            </tr>
            <tr>
                <td style="width: 70%;">
                    9. Các chống chỉ định khác, nếu có ghi rõ:
                    <br>
                    <span style="display: block; border-bottom: 1px dotted black;">{{Group9Text}}</span>
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group9Khong}}</span> 
                    <label for="khong">Không</label>
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group9Co}}</span> 
                    <label for="co">Có</label>
                </td>
            </tr>
            <tr class="clear-border">
                <td class="clear-border" colspan="3">
                    <span style="font-size: small;"><i>*: Chỉ áp dụng với vắc xin sống giảm độc lực</i></span>
                </td>
            </tr>
        </tbody>
    </table>

    <table id="table-kham-sang-loc-chuyen-khoa">
        <tbody>
            <tr>
                <td style="width: 50%;">
                    <div class="container">
                        <div class="label">
                            <b>_ Khám sàng lọc theo chuyên khoa</b>
                            &nbsp;           
                            Không         
                            <span class="square">{{GroupKhamSangLocChuyenKhoaKhong}}</span>   
                            &nbsp;&nbsp;           
                            Có           
                            <span class="square">{{GroupKhamSangLocChuyenKhoaCo}}</span> 
                        </div>
                    </div>
                </td>
                <td>
                    <div class="container">
                        <div class="label">chuyên khoa:&nbsp;</div>       
                        <div class="value">{{GroupChuyenKhoaText}}</div>    
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <div class="container">
                        <div class="label">+ Lý do:&nbsp;</div>
                        <div class="value" style="width:95%">{{LyDo}}</div>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <div class="container">
                        <div class="label">+ Kết quả:&nbsp;</div>
                        <div class="value" style="width:95%">{{KetQua}}</div>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <div class="container">
                        <div class="label">+ Kết luận:&nbsp;</div>
                        <div class="value" style="width:95%">{{KetLuan}}</div>
                    </div>
                </td>
            </tr>
        </tbody>
    </table>

    <table id="table-ket-luan">
        <tr>
            <td>
                <b>II. Kết luận</b>
            </td>
        </tr>
        <tr>
            <td>
                <b>- Đủ điều kiện tiêm chủng ngay</b> (Tất cả đều <b>KHÔNG</b> có điểm bất thường)
                <span class="square">{{KhongCoBatThuong}}</span> 
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <div class="container-flex">
                    - Loại vắc xin tiêm chủng lần này:&nbsp;
                    <span style="display: block; width: 50%; border-bottom: 1px dotted black; ">{{TenVacxin}}</span>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                - Chống chỉ định tiêm chủng (Khi <b>CÓ</b> điểm bất thường tại mục 9)
                <span class="square">{{ChongChiDinh}}</span> 
            </td>
        </tr>
        <tr>
            <td>
                - Tạm hoãn tiêm chủng (Khi <b>CÓ</b> bất kỳ một điểm bất thường tại các mục 1,2,3,4,5,6,7,8) 
                <span class="square">{{TamHoan}}</span> 
            </td>
        </tr>
    </table>

    <table id="table-nguoi-thuc-hien">
        <tr>
            <td style="width: 50%;"></td>
            <td class="text-center" style="width: 50%;">
                Hồi {{Hoi}}, {{NgayThangHienTai}}
                <br>
                Người thực hiện sàng lọc
                <br>
                (ký, ghi rõ họ và tên)
                <br><br><br><br>
                <span style="text-transform: uppercase;">{{HoTenBacSi}}</span>
            </td>
        </tr>
    </table>
</div>'
WHERE [Name] = N'BanKiemTruocTiemChungTrongBenhVienSoSinh'

UPDATE Template
SET Body = N'<style>      
    table, th, td {          
        border-collapse: collapse;          
        font-family: Times New Roman;          
        font-size: medium;          
    } 

    th, td {          
        padding: 1px;      
    }
    
    .breakword {     
        word-break: break-all;
    }
    
    .container {          
        width: 100%;          
        display: table;      
    }    
    
    .container-flex {          
        width: 100%;          
        display: flex;      
    }   
    
    .container .label {          
        width: max-content;      
    } 
    
    .container-flex .label {          
        width: max-content;  
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

    .container-flex .value {          
        display: table-cell;          
        width: 100%;          
        height: 100%;          
        vertical-align: top;          
        position: relative;          
        box-sizing: border-box;          
        border-bottom: 1px dotted black;      
    }

    .remove-border-bottom {
        border-bottom: none !important;
    }

    .container-flex {          
        width: 100%;          
        display: flex;      
    }   
    
    #table-thong-tin, #table-ket-luan, #table-kham-sang-loc-chuyen-khoa, #table-nguoi-thuc-hien {
        width: 100%;
    } 

    #table-ket-qua {
        width: 100%;
        border-collapse: collapse;
    }

    #table-ket-qua tr {
        border: 1px solid black;
    }

    #table-ket-qua td {
        border: 1px solid black;
        padding: 0.2rem !important;
    }
    
    .cell-6 {
        width: 60%;
    }

    .cell-4 {
        width: 40%;
    }

    .text-center {
        text-align: center;
    }

    .float-right {
        float: right;
    }

    .font-small {
        font-size: small;
    }

    .clear-border {
        border: 0px !important;
    }

    span.square {
        vertical-align: bottom;
        border: solid 1px;
        width: 20px;
        height: 20px;
        font-weight: normal;
        display: inline-block;
        text-align: center;
    }
</style>  

<div>         
    <table style="width: 100%;">          
        <tbody>              
            <tr>                  
                <td style="width: 33.33%; font-size: large;text-align: center;">         
                    <img src="{{ImageSrc}}" style="width: 100%; height: 100px;">
                </td>                       
                <td colspan="2" class="text-center" style="width: 66.66%;font-size: large;">                      
                    <b>CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM</b>
                    <br>
                    <b>Độc lập - Tự do - Hạnh phúc</b>        
                    <hr style="width: 20%;border-bottom: 2px solid black;border-left: none;border-top: none;border-right: none;">                  
                </td>              
            </tr>
            <tr>
                <td colspan="2"></td>
                <td style="width: 33.33%;" class="text-center font-small">
                    {{HoTen}}
                    <br>
                    <img
                        style="margin: 0 auto; height: 200%"
                        src="data:image/png;base64,{{Barcode}}"
                        alt="barcode"
                    />
                    <br>
                    {{MaTiepNhan}}
                </td>
            </tr>         
            <tr>                  
                <td colspan="3" style="font-size: 1.25rem;text-align: center; padding-left: 20%; padding-right: 20%;">                      
                    <br>                      
                    <b>BẢNG KIỂM TRƯỚC TIÊM CHỦNG ĐỐi VỚI TRẺ SƠ SINH TẠI CÁC CƠ SỞ TIÊM CHỦNG NGOÀI BỆNH VIỆN</b>                  
                </td>              
            </tr>          
        </tbody>      
    </table>

    <br>

    <table id="table-thong-tin">
        <tbody>
            <tr>
                <td colspan="2">
                    <div class="container">
                        <div class="label">Họ và tên trẻ:&nbsp;</div>
                        <div class="value remove-border-bottom">{{HoTen}}</div>
                        <div class="label" style="display: flex;">
                            &nbsp;           
                            Nam          
                            <span class="square">{{GioiTinhNam}}</span>   
                            &nbsp;&nbsp;           
                            Nữ          
                            <span class="square">{{GioiTinhNu}}</span>   
                        </div>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <div style="width: 50%;">
                        <div class="container">
                            <div class="label">Sinh:&nbsp;</div>
                            <div class="value text-center remove-border-bottom" style="width: fit-content;">{{GioSinh}}&nbsp;</div>
                            <div class="label">giờ&nbsp;</div>
                            <div class="value text-center remove-border-bottom" style="width: fit-content;">{{PhutSinh}}&nbsp;</div>
                            <div class="label">ngày&nbsp;</div>
                            <div class="value text-center remove-border-bottom" style="width: fit-content;">{{NgaySinh}}&nbsp;</div>
                            <div class="label">tháng&nbsp;</div>
                            <div class="value text-center remove-border-bottom" style="width: fit-content;">{{ThangSinh}}&nbsp;</div>
                            <div class="label">năm&nbsp;</div>
                            <div class="value text-center remove-border-bottom" style="width: fit-content;">{{NamSinh}}&nbsp;</div>
                        </div>
                    </div>
                </td>
                <td>
                    <div class="container" style="width: fit-content;">
                        <div class="label">Tuổi thai khi sinh:&nbsp;</div>
                        <div class="value remove-border-bottom" style="width: fit-content;">{{TuoiThaiKhiSinh}}</div>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <div class="container">
                        <div class="label">Địa chỉ:&nbsp;</div>
                        <div class="value breakword remove-border-bottom">{{DiaChi}}</div>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <div class="container">
                        <div class="label">Họ tên bố/mẹ:&nbsp;</div>
                        <div class="value remove-border-bottom">{{HoTenBoMe}}</div>
                    </div>
                </td>
                <td>
                    <div class="container">
                        <div class="label">Điện thoại:&nbsp;</div>
                        <div class="value remove-border-bottom">{{DienThoaiBoMe}}</div>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <div class="container-flex">
                        <div class="label">Cân nặng:&nbsp;</div>
                        <div class="value text-center remove-border-bottom" style="width:fit-content">{{CanNang}}</div>
                        <div>g</div>
                    </div>
                </td>
                <td>
                    <div class="container-flex">
                        <div class="label">Thân nhiệt:&nbsp;</div>
                        <div class="value text-center remove-border-bottom" style="width:fit-content">{{ThanNhiet}}</div>
                        <div>&deg;C</div>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <div class="container-flex">
                        <div class="label" style="display: flex;">
                            Mẹ đã xét nghiệm HbsAg:
                            Không 
                            &nbsp;     
                            <span class="square">{{GroupXetNghiemHbsAgKhong}}</span>       
                            &nbsp;&nbsp;           
                            Có    
                            &nbsp;      
                            <span class="square">{{GroupXetNghiemHbsAgCo}}</span>   
                        </div>
                    </div>
                </td>
                <td>
                    <div class="container">
                        <div class="label" style="display: flex;">
                            Kết quả:
                            Dương tính      
                            &nbsp;
                            <span class="square">{{GroupKetQuaHbsAgCoChild}}</span>       
                            &nbsp;&nbsp;           
                            Âm tính    
                            &nbsp;
                            <span class="square">{{GroupKetQuaHbsAgKhongChild}}</span>         
                        </div>
                    </div>
                </td>
            </tr>
        </tbody>
    </table>

    <table id="table-ket-qua">
        <tbody>
            <tr class="clear-border">
                <td colspan="3" class="clear-border">
                    <b>I. Khám sàng lọc:</b>
                </td>
            </tr>
            <tr class="clear-border">
                <td colspan="3" class="clear-border">
                    Các dấu hiệu hiện tại:
                </td>
            </tr>
            <tr>
                <td style="width: 70%;">
                    1. Tình trạng sức khoẻ chưa ổn định
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group1Khong}}</span>  
                    <label for="khong">Không</label>
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group1Co}}</span>  
                    <label for="co">Có</label>
                </td>
            </tr>
            <tr>
                <td style="width: 70%;">
                    2. Sốt/Hạ thân nhiệt (Sốt nhiệt độ &ge; 37,5&deg;C; Hạ thân nhiệt: nhiệt độ &le; 35,5&deg;C)
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group2Khong}}</span>  
                    <label for="khong">Không</label>
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group2Co}}</span>  
                    <label for="co">Có</label>
                </td>
            </tr>
            <tr>
                <td style="width: 70%;">
                    3. Khóc bé hoặc không khóc được
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group3Khong}}</span>  
                    <label for="khong">Không</label>
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group3Co}}</span>  
                    <label for="co">Có</label>
                </td>
            </tr>
            <tr>
                <td style="width: 70%;">
                    4. Da, môi không hồng
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group4Khong}}</span>  
                    <label for="khong">Không</label>
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group4Co}}</span>  
                    <label for="co">Có</label>
                </td>
            </tr>
            <tr>
                <td style="width: 70%;">
                    5. Bú kém hoặc bỏ bú
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group5Khong}}</span>  
                    <label for="khong">Không</label>
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group5Co}}</span>  
                    <label for="co">Có</label>
                </td>
            </tr>
            <tr>
                <td style="width: 70%;">
                    6. Tuổi thai < 34 tuần
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group6Khong}}</span>  
                    <label for="khong">Không</label>
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group6Co}}</span>  
                    <label for="co">Có</label>
                </td>
            </tr>
            <tr>
                <td style="width: 70%;">
                    7. Trẻ < 2000g
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group7Khong}}</span>  
                    <label for="khong">Không</label>
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group7Co}}</span>  
                    <label for="co">Có</label>
                </td>
            </tr>
            <tr>
                <td style="width: 70%;">
                    8. Các chống chỉ định khác, nếu có ghi rõ:
                    <br>
                    <span style="display: block; border-bottom: 1px dotted black;"></span>
                    <!-- <span style="display: block; border-bottom: 1px dotted black;">{{Group8Text}}</span> -->
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group8Khong}}</span>  
                    <label for="khong">Không</label>
                </td>
                <td style="width: 15%; text-align: center;">
                    <span class="square">{{Group8Co}}</span>  
                    <label for="co">Có</label>
                </td>
            </tr>
        </tbody>
    </table>

    <br>

    <table id="table-ket-luan">
        <tr>
            <td>
                <b>II. Kết luận</b>
            </td>
        </tr>
        <tr>
            <td>
                <b>- Đủ điều kiện tiêm chủng ngay</b> (Tất cả đều <b>KHÔNG</b> có điểm bất thường)
                <span class="square">{{KhongCoBatThuong}}</span>  
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <div class="container-flex">
                    - Loại vắc xin tiêm chủng lần này:&nbsp;
                    <span style="display: block; width: 50%; border-bottom: 1px dotted black; ">{{TenVacxin}}</span>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                - Chống chỉ định tiêm chủng (Khi <b>CÓ</b> điểm bất thường tại mục 8)
                <span class="square">{{ChongChiDinh}}</span>  
            </td>
        </tr>
        <tr>
            <td>
                - Tạm hoãn tiêm chủng (Khi <b>CÓ</b> bất kỳ một điểm bất thường tại các mục 1,2,3,4,5,6,7) 
                <span class="square">{{TamHoan}}</span>  
            </td>
        </tr>
        <tr>
            <td>
                <div class="container">
                    <div class="label">
                        Đề nghị khám sàng lọc tại bệnh viện
                        &nbsp;           
                        Không           
                        <span class="square">{{KhongKhamSangLoc}}</span>  
                        &nbsp;&nbsp;           
                        Có           
                        <span class="square">{{CoKhamSangLoc}}</span>  
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="container">
                    <div class="label">+ Lý do:&nbsp;</div>
                    <div class="value breakword">{{LyDoKhamSangLoc}}</div>
                </div>
            </td>
        </tr>
    </table>

    <br>

    <table id="table-nguoi-thuc-hien">
        <tr>
            <td style="width: 50%;"></td>
            <td class="text-center" style="width: 50%;">
                Hồi {{Hoi}}, {{NgayThangHienTai}}
                <br>
                Người thực hiện sàng lọc
                <br>
                (ký, ghi rõ họ và tên)
                <br><br><br><br>
                <span style="text-transform: uppercase;">{{HoTenBacSi}}</span>
            </td>
        </tr>
    </table>
</div>'
WHERE [Name] = N'BanKiemTruocTiemChungNgoaiBenhVienSoSinh'

Update dbo.CauHinh
Set [Value] = '2.9.7' where [Name] = 'CauHinhHeThong.DatabaseVesion'