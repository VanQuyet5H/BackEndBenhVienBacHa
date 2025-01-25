alter table YeuCauTiepNhan add KetQuaKhamSucKhoeData NVARCHAR(Max) null;
UPDATE CauHinh
Set [Value] = '1.9.8' where [Name] = 'CauHinhHeThong.DatabaseVesion'