ALTER TABLE [dbo].[BenhNhan]
DROP COLUMN [MaBN]
 

ALTER TABLE [dbo].[BenhNhan]
Add [MaBN] AS CONCAT(Right(Year(CreatedOn),2), right( '000000' + ltrim( str( Id ) ), 6 ));

Update dbo.CauHinh
Set [Value] = '0.6.8' where [Name] = 'CauHinhHeThong.DatabaseVesion'