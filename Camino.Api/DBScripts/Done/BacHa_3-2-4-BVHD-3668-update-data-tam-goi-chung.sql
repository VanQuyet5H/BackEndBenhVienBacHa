BEGIN
-- lấy thông tin tất cả nhân viên đã tiếp nhận khám sức khỏe
DECLARE @HopDongNhanVien TABLE(
	Id int identity(1,1),
	YeuCauTiepNhanId BIGINT,
	HopDongKhamSucKhoeNhanVienId BIGINT,
	GoiKhamSucKhoeId BIGINT
);

INSERT INTO @HopDongNhanVien
select t.Id, t.HopDongKhamSucKhoeNhanVienId, h.GoiKhamSucKhoeId
from YeuCauTiepNhan t
LEFT JOIN HopDongKhamSucKhoeNhanVien h on t.HopDongKhamSucKhoeNhanVienId = h.Id
where HopDongKhamSucKhoeNhanVienId IS NOT NULL AND TrangThaiYeuCauTiepNhan <> 3
AND t.HopDongKhamSucKhoeNhanVienId in  ((SELECT HopDongKhamSucKhoeNhanVienId FROM GoiKhamSucKhoeChungDichVuKhamBenhNhanVien)
											UNION (SELECT HopDongKhamSucKhoeNhanVienId FROM GoiKhamSucKhoeChungDichVuKyThuatNhanVien))

-- xử lý xóa các data cũ
DELETE FROM GoiKhamSucKhoeChungDichVuKhamBenhNhanVien WHERE HopDongKhamSucKhoeNhanVienId IN (SELECT HopDongKhamSucKhoeNhanVienId FROM @HopDongNhanVien)
DELETE FROM GoiKhamSucKhoeChungDichVuKyThuatNhanVien WHERE HopDongKhamSucKhoeNhanVienId IN (SELECT HopDongKhamSucKhoeNhanVienId FROM @HopDongNhanVien)

DECLARE @countTiepNhanNhanVien INT = (SELECT COUNT(1) FROM @HopDongNhanVien), @idxHopDong int = 1;
DECLARE	@YeuCauTiepNhanId BIGINT = NULL,
		@HopDongKhamSucKhoeNhanVienId BIGINT = NULL,
		@GoiKhamSucKhoeId BIGINT = NULL
---------------------------------------------------------------------------------
-- tạo bảng tạm lưu thông tin dịch vụ
DECLARE @DichVuNgoaiGoi TABLE(
	Id int identity(1,1),
	DichVuBenhVienId BIGINT,
	NhomGiaId BIGINT,
	GoiKhamSucKhoeDichVuId BIGINT,
	ThoiDiemChiDinh DATETIME
);
DECLARE	@DichVuBenhVienId BIGINT = NULL,
		@NhomGiaId BIGINT = NULL,
		@GoiKhamSucKhoeDichVuId BIGINT = NULL,
		@ThoiDiemChiDinh DATETIME = NULL,
		@countDichVu INT = 0,
		@idxDichVu int = 0;
----------------------------------------------------------------------------------

BEGIN TRY
	BEGIN TRANSACTION
		WHILE (@idxHopDong <= @countTiepNhanNhanVien)
		BEGIN
			SELECT TOP 1
				@YeuCauTiepNhanId = YeuCauTiepNhanId,
				@HopDongKhamSucKhoeNhanVienId = HopDongKhamSucKhoeNhanVienId,
				@GoiKhamSucKhoeId = GoiKhamSucKhoeId
			FROM @HopDongNhanVien
			WHERE Id = @idxHopDong;

			-- xử lý dv khám
			SET @countDichVu = 0
			DELETE FROM @DichVuNgoaiGoi
			INSERT INTO @DichVuNgoaiGoi
			SELECT DichVuKhamBenhBenhVienId, NhomGiaDichVuKhamBenhBenhVienId, GoiKhamSucKhoeId, ThoiDiemChiDinh
			FROM YeuCauKhamBenh 
			WHERE YeuCauTiepNhanId = @YeuCauTiepNhanId AND GoiKhamSucKhoeId IS NOT NULL AND GoiKhamSucKhoeId <> @GoiKhamSucKhoeId AND TrangThai <> 6

			SET @countDichVu = (SELECT COUNT(1) FROM @DichVuNgoaiGoi)
			IF @countDichVu > 0
			BEGIN
				WHILE (@countDichVu > 0)
				BEGIN
					SELECT TOP 1
						@DichVuBenhVienId = DichVuBenhVienId,
						@NhomGiaId = NhomGiaId,
						@GoiKhamSucKhoeDichVuId = GoiKhamSucKhoeDichVuId,
						@ThoiDiemChiDinh = ThoiDiemChiDinh
					FROM @DichVuNgoaiGoi

					INSERT INTO GoiKhamSucKhoeChungDichVuKhamBenhNhanVien(DichVuKhamBenhBenhVienId, GoiKhamSucKhoeDichVuKhamBenhId, GoiKhamSucKhoeId, HopDongKhamSucKhoeNhanVienId, CreatedOn, CreatedById, LastTime, LastUserId)
					VALUES(@DichVuBenhVienId, (SELECT TOP 1 Id FROM GoiKhamSucKhoeDichVuKhamBenh WHERE GoiKhamSucKhoeId = @GoiKhamSucKhoeDichVuId AND DichVuKhamBenhBenhVienId = @DichVuBenhVienId AND NhomGiaDichVuKhamBenhBenhVienId = @NhomGiaId)
							, @GoiKhamSucKhoeDichVuId, @HopDongKhamSucKhoeNhanVienId, @ThoiDiemChiDinh, 1, @ThoiDiemChiDinh, 1)

					DELETE TOP(1) FROM @DichVuNgoaiGoi
					SET @countDichVu = (SELECT COUNT(1) FROM @DichVuNgoaiGoi)
					SELECT	@DichVuBenhVienId = NULL,
							@NhomGiaId = NULL,
							@GoiKhamSucKhoeDichVuId = NULL
				END
			END

			-- xử lý dịch vụ kỹ thuật
			SET @countDichVu = 0
			DELETE FROM @DichVuNgoaiGoi
			INSERT INTO @DichVuNgoaiGoi
			SELECT DichVuKyThuatBenhVienId, NhomGiaDichVuKyThuatBenhVienId, GoiKhamSucKhoeId, ThoiDiemChiDinh
			FROM YeuCauDichVuKyThuat 
			WHERE YeuCauTiepNhanId = @YeuCauTiepNhanId AND GoiKhamSucKhoeId IS NOT NULL AND GoiKhamSucKhoeId <> @GoiKhamSucKhoeId AND TrangThai <> 4

			SET @countDichVu = (SELECT COUNT(1) FROM @DichVuNgoaiGoi)
			IF @countDichVu > 0
			BEGIN
				SET @idxDichVu = (SELECT TOP 1 Id FROM @DichVuNgoaiGoi)
				WHILE (@countDichVu > 0)
				BEGIN
					SELECT TOP 1
						@DichVuBenhVienId = DichVuBenhVienId,
						@NhomGiaId = NhomGiaId,
						@GoiKhamSucKhoeDichVuId = GoiKhamSucKhoeDichVuId,
						@ThoiDiemChiDinh = ThoiDiemChiDinh
					FROM @DichVuNgoaiGoi

					INSERT INTO GoiKhamSucKhoeChungDichVuKyThuatNhanVien(DichVuKyThuatBenhVienId, GoiKhamSucKhoeDichVuDichVuKyThuatId, GoiKhamSucKhoeId, HopDongKhamSucKhoeNhanVienId, CreatedOn, CreatedById, LastTime, LastUserId)
					VALUES(@DichVuBenhVienId, (SELECT TOP 1 Id FROM GoiKhamSucKhoeDichVuDichVuKyThuat WHERE GoiKhamSucKhoeId = @GoiKhamSucKhoeDichVuId AND DichVuKyThuatBenhVienId = @DichVuBenhVienId AND NhomGiaDichVuKyThuatBenhVienId = @NhomGiaId)
							, @GoiKhamSucKhoeDichVuId, @HopDongKhamSucKhoeNhanVienId, @ThoiDiemChiDinh, 1, @ThoiDiemChiDinh, 1)

					DELETE TOP(1) FROM @DichVuNgoaiGoi
					SET @countDichVu = (SELECT COUNT(1) FROM @DichVuNgoaiGoi)

					SELECT	@DichVuBenhVienId = NULL,
							@NhomGiaId = NULL,
							@GoiKhamSucKhoeDichVuId = NULL
				END
			END


			SELECT	@idxHopDong = @idxHopDong + 1,
					@YeuCauTiepNhanId = NULL,
					@HopDongKhamSucKhoeNhanVienId = NULL,
					@GoiKhamSucKhoeId = NULL,
					@DichVuBenhVienId = NULL,
					@NhomGiaId = NULL,
					@GoiKhamSucKhoeDichVuId = NULL,
					@idxDichVu = 0,
					@ThoiDiemChiDinh = NULL
		END
		COMMIT TRANSACTION;
	END TRY 
	BEGIN CATCH
	SELECT   
        ERROR_NUMBER() AS ErrorNumber  
       ,ERROR_MESSAGE() AS ErrorMessage;
		ROLLBACK TRANSACTION;
	END CATCH
	SELECT @idxHopDong
END

Update dbo.CauHinh
Set [Value] = '3.2.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'