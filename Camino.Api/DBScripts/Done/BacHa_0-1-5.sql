UPDATE A
SET
    A.YeuCauTiepNhanId =	case when B.YeuCauKhamBenhId is not null then C.YeuCauTiepNhanId
							  else 
								case when B.YeuCauDichVuKyThuatId is not null then D.YeuCauTiepNhanId
								  else 
									case when B.YeuCauDichVuGiuongBenhVienId is not null then E.YeuCauTiepNhanId
									  else 
										case when B.YeuCauDuocPhamBenhVienId is not null then F.YeuCauTiepNhanId
										  else 
											case when B.YeuCauVatTuBenhVienId is not null then G.YeuCauTiepNhanId
											  else 
												case when B.DonThuocThanhToanChiTietId is not null then I.YeuCauTiepNhanId
												  else null
												end
											end
										end
									end
								end
							end
FROM
    [DuyetBaoHiem] AS A
    INNER JOIN [DuyetBaoHiemChiTiet] AS B ON B.DuyetBaoHiemId = A.Id
	LEFT JOIN [YeuCauKhamBenh] AS C ON C.Id = B.YeuCauKhamBenhId
	LEFT JOIN [YeuCauDichVuKyThuat] AS D ON D.Id = B.YeuCauDichVuKyThuatId
	LEFT JOIN [YeuCauDichVuGiuongBenhVien] AS E ON E.Id = B.YeuCauDichVuGiuongBenhVienId
	LEFT JOIN [YeuCauDuocPhamBenhVien] AS F ON F.Id = B.YeuCauDuocPhamBenhVienId
	LEFT JOIN [YeuCauVatTuBenhVien] AS G ON G.Id = B.YeuCauVatTuBenhVienId
	LEFT JOIN [DonThuocThanhToanChiTiet] AS H ON H.Id = B.DonThuocThanhToanChiTietId
	LEFT JOIN [DonThuocThanhToan] AS I ON I.Id = H.DonThuocThanhToanId

GO

ALTER TABLE [DuyetBaoHiem]
ALTER COLUMN [YeuCauTiepNhanId] bigint NOT NULL;

GO

Update CauHinh
Set [Value] = '0.1.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'