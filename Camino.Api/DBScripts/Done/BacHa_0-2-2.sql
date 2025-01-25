EXEC sp_RENAME 'YeuCauKhamBenh.ChuanDoanSoBoICD' , 'ChanDoanSoBoICDId', 'COLUMN'
EXEC sp_RENAME 'YeuCauKhamBenh.ChuanDoanSoBoGhiChu' , 'ChanDoanSoBoGhiChu', 'COLUMN'

ALTER TABLE KetQuaSinhHieu
ADD Glassgow FLOAT NULL

Update CauHinh
Set [Value] = '0.2.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'