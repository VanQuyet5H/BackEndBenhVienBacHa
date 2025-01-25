ALTER TABLE PhienXetNghiem
ADD ChoKetQua BIT NULL
GO

update PhienXetNghiem set choketqua = 0 where ThoiDiemKetLuan is not null
GO
update PhienXetNghiem set choketqua = 1
where Id in (
select distinct A.Id from PhienXetNghiem A
left join PhienXetNghiemChiTiet B on B.PhienXetNghiemId = A.Id
left join KetQuaXetNghiemChiTiet C on C.PhienXetNghiemChiTietId = B.Id
where C.Id is not null and A.choketqua is null
)
GO
update PhienXetNghiem set choketqua = 0
where Id in (
select distinct A.Id from PhienXetNghiem A
left join PhienXetNghiemChiTiet B on B.PhienXetNghiemId = A.Id
left join  KetQuaXetNghiemChiTiet C on C.PhienXetNghiemChiTietId = B.Id
where C.Id is not null and A.choketqua = 1 and b.ThoiDiemCoKetQua is not null
)
GO
Update dbo.CauHinh
Set [Value] = '3.0.6' where [Name] = 'CauHinhHeThong.DatabaseVesion'