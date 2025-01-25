
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Anh.ngoc
-- Create date: 21/05/2021
-- Description: cập nhật nơi thực hiện ưu tiên
-- =============================================
CREATE PROCEDURE [dbo].[sp_update_noi_thuc_hien_uu_tien_dvkt]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	declare @data table(
		Id int identity(1,1),
		DichVuKyThuatBenhVienId BIGINT,
		PhongBenhVienId BIGINT,
		ToTal DECIMAL
	);

	INSERT INTO @data
	        (
				DichVuKyThuatBenhVienId, 
				PhongBenhVienId,
				ToTal
	        )
	SELECT DichVuKyThuatBenhVienId, NoiThucHienId, COUNT(1) as total 
	FROM YeuCauDichVuKyThuat yc
	LEFT JOIN PhongBenhVien p on yc.NoiThucHienId = p.Id
	WHERE TrangThai <> 4 AND NoiThucHienId IS NOT NULL  AND p.HopDongKhamSucKhoeId IS NULL
	GROUP BY DichVuKyThuatBenhVienId, NoiThucHienId
	ORDER BY DichVuKyThuatBenhVienId

	SELECT DISTINCT DichVuKyThuatBenhVienId
	INTO #tableDVBVId
	FROM @data

	-- xoa noi thuc hien uu tien he thong
	DELETE FROM DichVuKyThuatBenhVienNoiThucHienUuTien WHERE LoaiNoiThucHienUuTien = 1

	-- LoaiNoiThucHienUuTien: 1 - he thong, 2 - nguoi dung
	INSERT INTO DichVuKyThuatBenhVienNoiThucHienUuTien(DichVuKyThuatBenhVienId, PhongBenhVienId, LoaiNoiThucHienUuTien, CreatedById, LastUserId, LastTime, CreatedOn)
	SELECT v.DichVuKyThuatBenhVienId, (SELECT TOP 1 PhongBenhVienId FROM @data d1 WHERE d1.DichVuKyThuatBenhVienId = v.DichVuKyThuatBenhVienId ORDER BY ToTal DESC), 1, 1, 1, GETDATE(), GETDATE()
	FROM #tableDVBVId v
	
	--select * from #tableDVBVId
	DROP TABLE #tableDVBVId
END

GO
Update dbo.CauHinh
Set [Value] = '2.1.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'
