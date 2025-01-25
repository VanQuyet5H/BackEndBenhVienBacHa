UPDATE NhomDichVuBenhVien SET Ten = N'PHẪU THUẬT/THỦ THUẬT' WHERE Ma = N'PTTT'
UPDATE NhomDichVuBenhVien SET Ten = N'XÉT NGHIỆM' WHERE Ma = N'XN'
UPDATE NhomDichVuBenhVien SET Ten = N'CHẨN ĐOÁN HÌNH ẢNH' WHERE Ma = N'CĐHA'
UPDATE NhomDichVuBenhVien SET Ten = N'THĂM DÒ CHỨC NĂNG' WHERE Ma = N'TDCN'
UPDATE NhomDichVuBenhVien SET Ten = N'DỊCH VỤ Y TẾ' WHERE Ma = N'DVYT'
UPDATE NhomDichVuBenhVien SET Ten = N'MÁU VÀ CHẾ PHẨM MÁU' WHERE Ma = N'MVCP'
UPDATE NhomDichVuBenhVien SET Ten = N'DỊCH VỤ THEO YÊU CẦU' WHERE Ma = N'DVTYC'
UPDATE NhomDichVuBenhVien SET Ten = N'PHÒNG BỆNH, GIƯỜNG BỆNH' WHERE Ma = N'PBGB'

GO
Update CauHinh
Set [Value] = '1.8.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'