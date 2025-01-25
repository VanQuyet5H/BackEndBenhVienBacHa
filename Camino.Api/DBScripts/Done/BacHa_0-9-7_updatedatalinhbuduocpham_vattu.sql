-- =============================================
-- Author:		Pham.Thach
-- Create date: 21/10/2020
-- Description:	update data cu linh  bu duoc pham
-- =============================================
CREATE PROC	[dbo].[update_data_linh_bu_duoc_pham]
AS
BEGIN
    SET NOCOUNT ON;
	DECLARE @data TABLE(
		Id INT IDENTITY(1,1),
		YeuCauDuocPhamBenhVienId BIGINT,
		DuocPhamBenhVienId BIGINT,
		SoLuong FLOAT,
		LaDuocPhamBHYT BIT,
		TrangThai INT,
		LoaiPhieuLinh INT,
		YeuCauLinhDuocPhamId BIGINT
	);
	INSERT INTO @data
	        (
				YeuCauDuocPhamBenhVienId,
				DuocPhamBenhVienId,
				SoLuong,
				LaDuocPhamBHYT,
				TrangThai,
				LoaiPhieuLinh,
				YeuCauLinhDuocPhamId
	        )
	SELECT
		d.Id,
		d.DuocPhamBenhVienId AS DuocPhamBenhVienId ,
		d.SoLuong AS SoLuong ,
		d.LaDuocPhamBHYT AS LaDuocPhamBHYT,
		d.TrangThai AS TrangThai ,
		d.LoaiPhieuLinh AS LoaiPhieuLinh,
		d.YeuCauLinhDuocPhamId AS YeuCauLinhDuocPhamId 
	FROM dbo.YeuCauDuocPhamBenhVien d
	WHERE d.TrangThai = 2 AND d.LoaiPhieuLinh = 2 AND d.YeuCauLinhDuocPhamId IS NOT NULL

	DECLARE @total INT = (SELECT COUNT(1) FROM @data), @idx INT = 1;

	DECLARE 
		@YeuCauDuocPhamBenhVienId BIGINT = NULL,
		@DuocPhamBenhVienId BIGINT = NULL,
		@SoLuong FLOAT = NULL,
		@LaDuocPhamBHYT BIT = NULL,
		@TrangThai INT = NULL,

		@YeuCauLinhDuocPhamId BIGINT = NULL,
		@CreatedById INT = 1,
		@LastUserId INT = 1,
		@LastTime DATETIME = GETDATE(),
		@CreatedOn DATETIME = GETDATE()

		BEGIN TRY
			BEGIN TRANSACTION
			DELETE ct FROM dbo.YeuCauLinhDuocPhamChiTiet  ct
					INNER JOIN  dbo.YeuCauLinhDuocPham yc ON ct.YeuCauLinhDuocPhamId = yc.Id
					WHERE (yc.DuocDuyet = 1 OR yc.DuocDuyet IS NULL) AND yc.LoaiPhieuLinh = 2
			WHILE(@idx <= @total)
			BEGIN
				SELECT TOP 1  
						@YeuCauLinhDuocPhamId = YeuCauLinhDuocPhamId,
						@DuocPhamBenhVienId = DuocPhamBenhVienId,
						@SoLuong = SoLuong,
						@LaDuocPhamBHYT = LaDuocPhamBHYT,
						@TrangThai = TrangThai,
						@YeuCauDuocPhamBenhVienId = YeuCauDuocPhamBenhVienId
				FROM @data
				WHERE Id = @idx;

				SELECT @YeuCauLinhDuocPhamId,
						@DuocPhamBenhVienId,
						@SoLuong,
						@LaDuocPhamBHYT,
						@TrangThai,
						@YeuCauDuocPhamBenhVienId
				BEGIN TRY
					INSERT dbo.YeuCauLinhDuocPhamChiTiet
					(
					    YeuCauLinhDuocPhamId,
					    DuocPhamBenhVienId,
					    LaDuocPhamBHYT,
					    SoLuong,
					    CreatedById,
					    LastUserId,
					    LastTime,
					    CreatedOn,
					    SoLuongCoTheXuat,
					    YeuCauDuocPhamBenhVienId
					)
					VALUES
					(   @YeuCauLinhDuocPhamId,         -- YeuCauLinhDuocPhamId - bigint
					    @DuocPhamBenhVienId,         -- DuocPhamBenhVienId - bigint
					    @LaDuocPhamBHYT,      -- LaDuocPhamBHYT - bit
					    @SoLuong,       -- SoLuong - float
					    @CreatedById,         -- CreatedById - bigint
					    @LastUserId,         -- LastUserId - bigint
					    @LastTime, -- LastTime - datetime
					    @CreatedOn, -- CreatedOn - datetime
					    @SoLuong,       -- SoLuongCoTheXuat - float
					    @YeuCauDuocPhamBenhVienId          -- YeuCauDuocPhamBenhVienId - bigint
					    )

						UPDATE  dbo.YeuCauDuocPhamBenhVien 
						SET	YeuCauLinhDuocPhamId = NULL, SoLuongDaLinhBu = @SoLuong
						WHERE Id = @YeuCauDuocPhamBenhVienId

				SELECT	@idx = @idx + 1,
								@YeuCauLinhDuocPhamId  = NULL,
								@DuocPhamBenhVienId = NULL,
								@SoLuong =	NULL,
								@LaDuocPhamBHYT = NULL,
								@TrangThai = NULL,
								@YeuCauDuocPhamBenhVienId = NULL
			END TRY
			BEGIN CATCH
				SELECT  
            ERROR_NUMBER() AS ErrorNumber  
            ,ERROR_SEVERITY() AS ErrorSeverity  
            ,ERROR_STATE() AS ErrorState  
            ,ERROR_PROCEDURE() AS ErrorProcedure  
            ,ERROR_LINE() AS ErrorLine  
            ,ERROR_MESSAGE() AS ErrorMessage; 
			END CATCH
			END
			COMMIT TRANSACTION;
		END TRY
		BEGIN CATCH
		ROLLBACK TRANSACTION;
		END CATCH
END
GO
--==========================================================================================

-- =============================================
-- Author:		Pham.Thach
-- Create date: 21/10/2020
-- Description:	update data cu linh  bu vat tu
-- =============================================

CREATE PROC	[dbo].[update_data_linh_bu_vat_tu]
AS
BEGIN
    SET NOCOUNT ON;
	DECLARE @data TABLE(
		Id INT IDENTITY(1,1),
		YeuCauVatTuBenhVienId BIGINT,
		VatTuBenhVienId BIGINT,
		SoLuong FLOAT,
		LaVatTuBHYT BIT,
		TrangThai INT,
		LoaiPhieuLinh INT,
		YeuCauLinhVatTuId BIGINT
	);
	INSERT INTO @data
	        (
				YeuCauVatTuBenhVienId,
				VatTuBenhVienId,
				SoLuong,
				LaVatTuBHYT,
				TrangThai,
				LoaiPhieuLinh,
				YeuCauLinhVatTuId
	        )
	SELECT
		d.Id,
		d.VatTuBenhVienId AS VatTuBenhVienId ,
		d.SoLuong AS SoLuong ,
		d.LaVatTuBHYT AS LaVatTuBHYT,
		d.TrangThai AS TrangThai ,
		d.LoaiPhieuLinh AS LoaiPhieuLinh,
		d.YeuCauLinhVatTuId AS YeuCauLinhVatTuId 
	FROM dbo.YeuCauVatTuBenhVien d
	WHERE d.TrangThai = 2 AND d.LoaiPhieuLinh = 2 AND d.YeuCauLinhVatTuId IS NOT NULL

	DECLARE @total INT = (SELECT COUNT(1) FROM @data), @idx INT = 1;

	DECLARE 
		@YeuCauVatTuBenhVienId BIGINT = NULL,
		@VatTuBenhVienId BIGINT = NULL,
		@SoLuong FLOAT = NULL,
		@LaVatTuBHYT BIT = NULL,
		@TrangThai INT = NULL,

		@YeuCauLinhVatTuId BIGINT = NULL,
		@CreatedById INT = 1,
		@LastUserId INT = 1,
		@LastTime DATETIME = GETDATE(),
		@CreatedOn DATETIME = GETDATE()

		BEGIN TRY
			BEGIN TRANSACTION
			DELETE ct FROM dbo.YeuCauLinhVatTuChiTiet  ct
					INNER JOIN  dbo.YeuCauLinhVatTu yc ON ct.YeuCauLinhVatTuId = yc.Id
					WHERE (yc.DuocDuyet = 1 OR yc.DuocDuyet IS NULL) AND yc.LoaiPhieuLinh = 2
			WHILE(@idx <= @total)
			BEGIN
				SELECT TOP 1  
						@YeuCauLinhVatTuId = YeuCauLinhVatTuId,
						@VatTuBenhVienId = VatTuBenhVienId,
						@SoLuong = SoLuong,
						@LaVatTuBHYT = LaVatTuBHYT,
						@TrangThai = TrangThai,
						@YeuCauVatTuBenhVienId = YeuCauVatTuBenhVienId
				FROM @data
				WHERE Id = @idx;

				SELECT @YeuCauLinhVatTuId,
						@VatTuBenhVienId,
						@SoLuong,
						@LaVatTuBHYT,
						@TrangThai,
						@YeuCauVatTuBenhVienId
				BEGIN TRY

				INSERT dbo.YeuCauLinhVatTuChiTiet
				(
				    YeuCauLinhVatTuId,
				    VatTuBenhVienId,
				    LaVatTuBHYT,
				    SoLuong,
				    CreatedById,
				    LastUserId,
				    LastTime,
				    CreatedOn,
				    SoLuongCoTheXuat,
				    YeuCauVatTuBenhVienId
				)
				VALUES
				(   @YeuCauLinhVatTuId,         -- YeuCauLinhVatTuId - bigint
				    @VatTuBenhVienId,         -- VatTuBenhVienId - bigint
				    @LaVatTuBHYT,      -- LaVatTuBHYT - bit
				    @SoLuong,       -- SoLuong - float
				    @CreatedById,         -- CreatedById - bigint
				    @LastUserId,         -- LastUserId - bigint
				    @LastTime, -- LastTime - datetime
					@CreatedOn, -- CreatedOn - datetime
				    @SoLuong,       -- SoLuongCoTheXuat - float
				    @YeuCauVatTuBenhVienId          -- YeuCauVatTuBenhVienId - bigint
				    )
					
						UPDATE  dbo.YeuCauVatTuBenhVien 
						SET	YeuCauLinhVatTuId = NULL, SoLuongDaLinhBu = @SoLuong
						WHERE Id = @YeuCauVatTuBenhVienId

				SELECT	@idx = @idx + 1,
								@YeuCauLinhVatTuId  = NULL,
								@VatTuBenhVienId = NULL,
								@SoLuong =	NULL,
								@LaVatTuBHYT = NULL,
								@TrangThai = NULL,
								@YeuCauVatTuBenhVienId = NULL
			END TRY
			BEGIN CATCH
				SELECT  
            ERROR_NUMBER() AS ErrorNumber  
            ,ERROR_SEVERITY() AS ErrorSeverity  
            ,ERROR_STATE() AS ErrorState  
            ,ERROR_PROCEDURE() AS ErrorProcedure  
            ,ERROR_LINE() AS ErrorLine  
            ,ERROR_MESSAGE() AS ErrorMessage; 
			END CATCH
			END
			COMMIT TRANSACTION;
		END TRY
		BEGIN CATCH
		ROLLBACK TRANSACTION;
		END CATCH
END
GO
EXEC [dbo].[update_data_linh_bu_duoc_pham]
EXEC [dbo].[update_data_linh_bu_vat_tu]
GO	
Update CauHinh
Set [Value] = '0.9.7' where [Name] = 'CauHinhHeThong.DatabaseVesion'