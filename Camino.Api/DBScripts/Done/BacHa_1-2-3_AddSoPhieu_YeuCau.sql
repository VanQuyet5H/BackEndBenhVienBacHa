CREATE FUNCTION [dbo].[LaySoPhieuNhapKho] (@yeu_cau_nhap_kho_id bigint, @type int)    
RETURNS nvarchar(100)
AS  
BEGIN 
	DECLARE @so_phieu_duoc_pham nvarchar(100);
	DECLARE @so_phieu_vat_tu nvarchar(100);
	DECLARE @result nvarchar(100);


	SELECT TOP 1 @so_phieu_duoc_pham = (concat('PN',right(datepart(year,db.CreatedOn),(2)),right('000000'+ltrim(str(db.Id)),(6))))
    FROM [NhapKhoDuocPham] as db
    WHERE db.YeuCauNhapKhoDuocPhamId = @yeu_cau_nhap_kho_id;

	SELECT TOP 1 @so_phieu_vat_tu = (concat('PN',right(datepart(year,db.CreatedOn),(2)),right('000000'+ltrim(str(db.Id)),(6))))
    FROM [NhapKhoVatTu] as db
    WHERE db.YeuCauNhapKhoVatTuId = @yeu_cau_nhap_kho_id;
	
	IF @type = 1
		set @result = CASE WHEN @so_phieu_duoc_pham IS NULL THEN '' ELSE @so_phieu_duoc_pham END;
	IF @type = 2
		set @result = CASE WHEN @so_phieu_vat_tu IS NULL THEN '' ELSE @so_phieu_vat_tu END;

	RETURN @result;
END;
GO

ALTER TABLE dbo.YeuCauNhapKhoDuocPham Add SoPhieu AS ([dbo].[LaySoPhieuNhapKho]([Id], 1))
ALTER TABLE dbo.YeuCauNhapKhoVatTu Add SoPhieu AS ([dbo].[LaySoPhieuNhapKho]([Id], 2))

GO
Update dbo.CauHinh
Set [Value] = '1.2.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'