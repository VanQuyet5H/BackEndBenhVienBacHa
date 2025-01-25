ALTER TABLE CongTyBaoHiemTuNhan
ADD MaSoThue NVARCHAR(20) NULL

ALTER TABLE CongTyBaoHiemTuNhan
ADD DonVi NVARCHAR(200) NULL

Update CauHinh
Set [Value] = '1.1.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'