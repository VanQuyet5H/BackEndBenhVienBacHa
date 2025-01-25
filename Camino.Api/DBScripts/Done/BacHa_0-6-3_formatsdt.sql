CREATE FUNCTION [dbo].[LayFormatSoDienThoai] (@so_dien_thoai nvarchar(100))  
RETURNS nvarchar(100)
AS  
BEGIN 
	DECLARE @result nvarchar(100);
	DECLARE @lengthSoDienThoai int;

	set @lengthSoDienThoai = LEN(@so_dien_thoai);

    IF @so_dien_thoai = '' or @so_dien_thoai is NULL
		set @result = '';
	If  @lengthSoDienThoai > 10
		set @result = CONCAT(SUBSTRING(@so_dien_thoai, 1, 3), ' ', SUBSTRING(@so_dien_thoai, 4, 3), ' ', SUBSTRING(@so_dien_thoai, 7, 4),' ', SUBSTRING(@so_dien_thoai, 10, @lengthSoDienThoai - 9));
	If @lengthSoDienThoai = 10
		set @result = CONCAT(SUBSTRING(@so_dien_thoai, 1, 3), ' ', SUBSTRING(@so_dien_thoai, 4, 3), ' ', SUBSTRING(@so_dien_thoai, 7, 4))
	If @result = '' or @result is NULL
		set @result = @so_dien_thoai
	RETURN @result;
END;
GO

Alter TABLE [User] Add SoDienThoaiDisplay AS ([dbo].[LayFormatSoDienThoai]([SoDienThoai]))

Update dbo.CauHinh
Set [Value] = '0.6.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'