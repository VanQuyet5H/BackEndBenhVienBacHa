ALTER TABLE [dbo].[YeuCauTiepNhan]
ADD [NguoiLienHeDiaChiDayDu] AS dbo.[LayDiaChiDayDu](NguoiLienHeDiaChi,NguoiLienHePhuongXaId,NguoiLienHeQuanHuyenId,NguoiLienHeTinhThanhId);
GO

Update dbo.CauHinh
Set [Value] = '2.9.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'