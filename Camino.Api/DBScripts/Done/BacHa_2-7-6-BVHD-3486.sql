CREATE FUNCTION [dbo].[LaySoPhieuNhapKhoDuocPham] (@kho_id bigint, @nhap_kho_duoc_pham_id bigint, @nam nvarchar(2), @thang nvarchar(2))    
RETURNS nvarchar(100)
AS  
BEGIN 
	DECLARE @ma_kho nvarchar(50);
	DECLARE @so_phieu_truoc nvarchar(100);
	DECLARE @so_phieu bigint;
	DECLARE @result nvarchar(100);


	SELECT TOP 1 @ma_kho = CASE WHEN k.MaKhoBenhVien IS NULL THEN '' ELSE k.MaKhoBenhVien END
    FROM [Kho] as k
    WHERE k.Id = @kho_id;

	SELECT TOP 1 @so_phieu_truoc = RIGHT(n.SoPhieu, 7)
    FROM [NhapKhoDuocPham] as n
    WHERE n.Id < @nhap_kho_duoc_pham_id AND KhoId = @kho_id
	ORDER BY Id desc;
	
	set @so_phieu =	CASE WHEN ISNUMERIC(@so_phieu_truoc) = 1 THEN CAST(@so_phieu_truoc AS bigint) + 1 ELSE 1 END

	set @result = 'KD-' + trim(CHAR(13) + CHAR(10) from trim(@ma_kho)) + 'N' + @thang + @nam  + right('0000000'+ltrim(str(@so_phieu)),(7))

	RETURN @result;
END;
GO
CREATE FUNCTION [dbo].[LaySoPhieuXuatKhoDuocPham] (@kho_xuat_id bigint, @xuat_kho_duoc_pham_id bigint, @nam nvarchar(2), @thang nvarchar(2))    
RETURNS nvarchar(100)
AS  
BEGIN 
	DECLARE @ma_kho nvarchar(50);
	DECLARE @so_phieu_truoc nvarchar(100);
	DECLARE @so_phieu bigint;
	DECLARE @result nvarchar(100);


	SELECT TOP 1 @ma_kho = CASE WHEN k.MaKhoBenhVien IS NULL THEN '' ELSE k.MaKhoBenhVien END
    FROM [Kho] as k
    WHERE k.Id = @kho_xuat_id;

	SELECT TOP 1 @so_phieu_truoc = RIGHT(n.SoPhieu, 7)
    FROM [XuatKhoDuocPham] as n
    WHERE n.Id < @xuat_kho_duoc_pham_id AND KhoXuatId = @kho_xuat_id
	ORDER BY Id desc;
	
	set @so_phieu =	CASE WHEN ISNUMERIC(@so_phieu_truoc) = 1 THEN CAST(@so_phieu_truoc AS bigint) + 1 ELSE 1 END

	set @result = 'KD-' + trim(CHAR(13) + CHAR(10) from trim(@ma_kho)) + 'X' + @thang + @nam  + right('0000000'+ltrim(str(@so_phieu)),(7))

	RETURN @result;
END;
GO
CREATE FUNCTION [dbo].[LaySoPhieuNhapKhoVatTu] (@kho_id bigint, @nhap_kho_vat_tu_id bigint, @nam nvarchar(2), @thang nvarchar(2))    
RETURNS nvarchar(100)
AS  
BEGIN 
	DECLARE @ma_kho nvarchar(50);
	DECLARE @so_phieu_truoc nvarchar(100);
	DECLARE @so_phieu bigint;
	DECLARE @result nvarchar(100);


	SELECT TOP 1 @ma_kho = CASE WHEN k.MaKhoBenhVien IS NULL THEN '' ELSE k.MaKhoBenhVien END
    FROM [Kho] as k
    WHERE k.Id = @kho_id;

	SELECT TOP 1 @so_phieu_truoc = RIGHT(n.SoPhieu, 7)
    FROM [NhapKhoVatTu] as n
    WHERE n.Id < @nhap_kho_vat_tu_id AND KhoId = @kho_id
	ORDER BY Id desc;
	
	set @so_phieu =	CASE WHEN ISNUMERIC(@so_phieu_truoc) = 1 THEN CAST(@so_phieu_truoc AS bigint) + 1 ELSE 1 END

	set @result = 'VTTY-' + trim(CHAR(13) + CHAR(10) from trim(@ma_kho)) + 'N' + @thang + @nam  + right('0000000'+ltrim(str(@so_phieu)),(7))

	RETURN @result;
END;
GO
CREATE FUNCTION [dbo].[LaySoPhieuXuatKhoVatTu] (@kho_xuat_id bigint, @xuat_kho_vat_tu_id bigint, @nam nvarchar(2), @thang nvarchar(2))    
RETURNS nvarchar(100)
AS  
BEGIN 
	DECLARE @ma_kho nvarchar(50);
	DECLARE @so_phieu_truoc nvarchar(100);
	DECLARE @so_phieu bigint;
	DECLARE @result nvarchar(100);


	SELECT TOP 1 @ma_kho = CASE WHEN k.MaKhoBenhVien IS NULL THEN '' ELSE k.MaKhoBenhVien END
    FROM [Kho] as k
    WHERE k.Id = @kho_xuat_id;

	SELECT TOP 1 @so_phieu_truoc = RIGHT(n.SoPhieu, 7)
    FROM [XuatKhoVatTu] as n
    WHERE n.Id < @xuat_kho_vat_tu_id AND KhoXuatId = @kho_xuat_id
	ORDER BY Id desc;
	
	set @so_phieu =	CASE WHEN ISNUMERIC(@so_phieu_truoc) = 1 THEN CAST(@so_phieu_truoc AS bigint) + 1 ELSE 1 END

	set @result = 'VTYT-' + trim(CHAR(13) + CHAR(10) from trim(@ma_kho)) + 'X' + @thang + @nam  + right('0000000'+ltrim(str(@so_phieu)),(7))

	RETURN @result;
END;
GO


ALTER TABLE [NhapKhoDuocPham]
DROP COLUMN [SoPhieu]
ALTER TABLE [NhapKhoDuocPham]
Add [SoPhieu] [nvarchar](100) NULL
GO
Create trigger [dbo].[trg_NhapKhoDuocPham_Set_SoPhieu] on [dbo].[NhapKhoDuocPham] after insert as
begin

	update A
	set [SoPhieu] = B.SoPhieuGen
	
	from [dbo].[NhapKhoDuocPham] A inner join 
	(select i.Id, ([dbo].[LaySoPhieuNhapKhoDuocPham](i.[KhoId], i.[Id],right(datepart(year,i.[CreatedOn]),(2)), right('00'+ltrim(str(datepart(month,i.[CreatedOn]))),(2)))) as SoPhieuGen from inserted i) B on A.Id = B.Id

end
GO

ALTER TABLE [dbo].[NhapKhoDuocPham] ENABLE TRIGGER [trg_NhapKhoDuocPham_Set_SoPhieu]
GO

ALTER TABLE [XuatKhoDuocPham]
DROP COLUMN [SoPhieu]
ALTER TABLE [XuatKhoDuocPham]
Add [SoPhieu] [nvarchar](100) NULL
GO
Create trigger [dbo].[trg_XuatKhoDuocPham_Set_SoPhieu] on [dbo].[XuatKhoDuocPham] after insert as
begin

	update A
	set [SoPhieu] = B.SoPhieuGen
	
	from [dbo].[XuatKhoDuocPham] A inner join 
	(select i.Id, ([dbo].[LaySoPhieuXuatKhoDuocPham](i.[KhoXuatId], i.[Id],right(datepart(year,i.[CreatedOn]),(2)), right('00'+ltrim(str(datepart(month,i.[CreatedOn]))),(2)))) as SoPhieuGen from inserted i) B on A.Id = B.Id

end
GO

ALTER TABLE [dbo].[XuatKhoDuocPham] ENABLE TRIGGER [trg_XuatKhoDuocPham_Set_SoPhieu]
GO
ALTER TABLE [NhapKhoVatTu]
DROP COLUMN [SoPhieu]
ALTER TABLE [NhapKhoVatTu]
Add [SoPhieu] [nvarchar](100) NULL
GO
Create trigger [dbo].[trg_NhapKhoVatTu_Set_SoPhieu] on [dbo].[NhapKhoVatTu] after insert as
begin

	update A
	set [SoPhieu] = B.SoPhieuGen
	
	from [dbo].[NhapKhoVatTu] A inner join 
	(select i.Id, ([dbo].[LaySoPhieuNhapKhoVatTu](i.[KhoId], i.[Id],right(datepart(year,i.[CreatedOn]),(2)), right('00'+ltrim(str(datepart(month,i.[CreatedOn]))),(2)))) as SoPhieuGen from inserted i) B on A.Id = B.Id

end
GO

ALTER TABLE [dbo].[NhapKhoVatTu] ENABLE TRIGGER [trg_NhapKhoVatTu_Set_SoPhieu]
GO

ALTER TABLE [XuatKhoVatTu]
DROP COLUMN [SoPhieu]
ALTER TABLE [XuatKhoVatTu]
Add [SoPhieu] [nvarchar](100) NULL
GO
Create trigger [dbo].[trg_XuatKhoVatTu_Set_SoPhieu] on [dbo].[XuatKhoVatTu] after insert as
begin

	update A
	set [SoPhieu] = B.SoPhieuGen
	
	from [dbo].[XuatKhoVatTu] A inner join 
	(select i.Id, ([dbo].[LaySoPhieuXuatKhoVatTu](i.[KhoXuatId], i.[Id],right(datepart(year,i.[CreatedOn]),(2)), right('00'+ltrim(str(datepart(month,i.[CreatedOn]))),(2)))) as SoPhieuGen from inserted i) B on A.Id = B.Id

end
GO

ALTER TABLE [dbo].[XuatKhoVatTu] ENABLE TRIGGER [trg_XuatKhoVatTu_Set_SoPhieu]
GO


DECLARE @CursorID INT; 
DECLARE CUR CURSOR FAST_FORWARD FOR
    SELECT Id
    FROM   [dbo].[NhapKhoDuocPham]
    ORDER BY Id;
 
OPEN CUR
FETCH NEXT FROM CUR INTO @CursorID
 
WHILE @@FETCH_STATUS = 0
BEGIN
   update [NhapKhoDuocPham] 
   set SoPhieu = ([dbo].[LaySoPhieuNhapKhoDuocPham]([KhoId], [Id],right(datepart(year,[CreatedOn]),(2)), right('00'+ltrim(str(datepart(month,[CreatedOn]))),(2))))
   where Id = @CursorID
   
   FETCH NEXT FROM CUR INTO @CursorID
END
CLOSE CUR
DEALLOCATE CUR
GO

DECLARE @CursorID INT; 
DECLARE CUR CURSOR FAST_FORWARD FOR
    SELECT Id
    FROM   [dbo].[XuatKhoDuocPham]
    ORDER BY Id;
 
OPEN CUR
FETCH NEXT FROM CUR INTO @CursorID
 
WHILE @@FETCH_STATUS = 0
BEGIN
   update [XuatKhoDuocPham] 
   set SoPhieu = ([dbo].[LaySoPhieuXuatKhoDuocPham]([KhoXuatId], [Id],right(datepart(year,[CreatedOn]),(2)), right('00'+ltrim(str(datepart(month,[CreatedOn]))),(2))))
   where Id = @CursorID
   
   FETCH NEXT FROM CUR INTO @CursorID
END
CLOSE CUR
DEALLOCATE CUR
GO

DECLARE @CursorID INT; 
DECLARE CUR CURSOR FAST_FORWARD FOR
    SELECT Id
    FROM   [dbo].[NhapKhoVatTu]
    ORDER BY Id;
 
OPEN CUR
FETCH NEXT FROM CUR INTO @CursorID
 
WHILE @@FETCH_STATUS = 0
BEGIN
   update [NhapKhoVatTu] 
   set SoPhieu = ([dbo].[LaySoPhieuNhapKhoVatTu]([KhoId], [Id],right(datepart(year,[CreatedOn]),(2)), right('00'+ltrim(str(datepart(month,[CreatedOn]))),(2))))
   where Id = @CursorID
   
   FETCH NEXT FROM CUR INTO @CursorID
END
CLOSE CUR
DEALLOCATE CUR
GO

DECLARE @CursorID INT; 
DECLARE CUR CURSOR FAST_FORWARD FOR
    SELECT Id
    FROM   [dbo].[XuatKhoVatTu]
    ORDER BY Id;
 
OPEN CUR
FETCH NEXT FROM CUR INTO @CursorID
 
WHILE @@FETCH_STATUS = 0
BEGIN
   update [XuatKhoVatTu] 
   set SoPhieu = ([dbo].[LaySoPhieuXuatKhoVatTu]([KhoXuatId], [Id],right(datepart(year,[CreatedOn]),(2)), right('00'+ltrim(str(datepart(month,[CreatedOn]))),(2))))
   where Id = @CursorID
   
   FETCH NEXT FROM CUR INTO @CursorID
END
CLOSE CUR
DEALLOCATE CUR
GO

ALTER TABLE [dbo].[YeuCauNhapKhoDuocPham]
DROP COLUMN [SoPhieu]
GO

ALTER TABLE [dbo].[YeuCauNhapKhoVatTu]
DROP COLUMN [SoPhieu]
GO

ALTER FUNCTION [dbo].[LaySoPhieuNhapKho] (@yeu_cau_nhap_kho_id bigint, @type int)    
RETURNS nvarchar(100)
AS  
BEGIN 
	DECLARE @so_phieu_duoc_pham nvarchar(100);
	DECLARE @so_phieu_vat_tu nvarchar(100);
	DECLARE @result nvarchar(100);


	SELECT TOP 1 @so_phieu_duoc_pham = db.SoPhieu
    FROM [NhapKhoDuocPham] as db
    WHERE db.YeuCauNhapKhoDuocPhamId = @yeu_cau_nhap_kho_id;

	SELECT TOP 1 @so_phieu_vat_tu = db.SoPhieu
    FROM [NhapKhoVatTu] as db
    WHERE db.YeuCauNhapKhoVatTuId = @yeu_cau_nhap_kho_id;
	
	IF @type = 1
		set @result = CASE WHEN @so_phieu_duoc_pham IS NULL THEN '' ELSE @so_phieu_duoc_pham END;
	IF @type = 2
		set @result = CASE WHEN @so_phieu_vat_tu IS NULL THEN '' ELSE @so_phieu_vat_tu END;

	RETURN @result;
END;
GO
ALTER TABLE [dbo].[YeuCauNhapKhoDuocPham]
ADD [SoPhieu]  AS ([dbo].[LaySoPhieuNhapKho]([Id],(1)))
GO
ALTER TABLE [dbo].[YeuCauNhapKhoVatTu]
ADD [SoPhieu]  AS ([dbo].[LaySoPhieuNhapKho]([Id],(2)))
GO
Update dbo.CauHinh
Set [Value] = '2.7.6' where [Name] = 'CauHinhHeThong.DatabaseVesion'
