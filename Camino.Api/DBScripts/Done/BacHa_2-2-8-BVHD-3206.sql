﻿ALTER TABLE HopDongKhamSucKhoe ALTER COLUMN NguoiKyHopDong NVARCHAR(100) NULL;
ALTER TABLE HopDongKhamSucKhoe ALTER COLUMN ChucDanhNguoiKy NVARCHAR(100) NULL;

Update dbo.CauHinh
Set [Value] = '2.2.8' where [Name] = 'CauHinhHeThong.DatabaseVesion'