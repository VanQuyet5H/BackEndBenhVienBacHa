DROP TRIGGER IF EXISTS  [dbo].[trg_DichVuKhamBenhBenhVien_Into_DichVuBenhVienTongHop_Add];
Go

Create trigger trg_DichVuKhamBenhBenhVien_Into_DichVuBenhVienTongHop_Add on [dbo].[DichVuKhamBenhBenhVien] after insert as
begin
	insert into [dbo].[DichVuBenhVienTongHop](LoaiDichVuBenhVien, DichVuKhamBenhBenhVienId, Ma, Ten, HieuLuc ,CreatedById,LastUserId, CreatedOn, LastTime)
	select 1, i.[Id], i.Ma, i.Ten, i.HieuLuc, i.CreatedById, i.LastUserId, i.CreatedOn, i.LastTime
	from inserted i

end
Go

DROP TRIGGER IF EXISTS  [dbo].[trg_DichVuKhamBenhBenhVien_Into_DichVuBenhVienTongHop_Delete];
Go

Create trigger trg_DichVuKhamBenhBenhVien_Into_DichVuBenhVienTongHop_Delete on [dbo].[DichVuKhamBenhBenhVien] after delete as
begin
	delete from [dbo].[DichVuBenhVienTongHop] where [dbo].[DichVuBenhVienTongHop].DichVuKhamBenhBenhVienId = (select d.[Id] from deleted d)
end
Go

DROP TRIGGER IF EXISTS  [dbo].[trg_DichVuKhamBenhBenhVien_Into_DichVuBenhVienTongHop_Update];
Go

Create trigger trg_DichVuKhamBenhBenhVien_Into_DichVuBenhVienTongHop_Update on [dbo].[DichVuKhamBenhBenhVien] after update as
begin
	update [dbo].[DichVuBenhVienTongHop]
	set Ten = T1.Ten, Ma = T1.Ma, HieuLuc = T1.HieuLuc
	
	from [dbo].[DichVuBenhVienTongHop] T inner join 
	(select i.Id, i.[Ten], i.[Ma], i.HieuLuc, i.CreatedById, i.LastUserId, i.CreatedOn, i.LastTime from inserted i) T1 on T.DichVuKhamBenhBenhVienId = T1.Id

end
Go
--DVKT--
DROP TRIGGER IF EXISTS  [dbo].[trg_DichVuKyThuatBenhVien_Into_DichVuBenhVienTongHop_Add];
Go

Create trigger trg_DichVuKyThuatBenhVien_Into_DichVuBenhVienTongHop_Add on [dbo].[DichVuKyThuatBenhVien] after insert as
begin
	insert into [dbo].[DichVuBenhVienTongHop](LoaiDichVuBenhVien, DichVuKyThuatBenhVienId, Ma, Ten, HieuLuc ,CreatedById,LastUserId, CreatedOn, LastTime)
	select 2, i.[Id], i.Ma, i.Ten, i.HieuLuc, i.CreatedById, i.LastUserId, i.CreatedOn, i.LastTime
	from inserted i

end
Go

DROP TRIGGER IF EXISTS  [dbo].[trg_DichVuKyThuatBenhVien_Into_DichVuBenhVienTongHop_Delete];
Go

Create trigger trg_DichVuKyThuatBenhVien_Into_DichVuBenhVienTongHop_Delete on [dbo].[DichVuKyThuatBenhVien] after delete as
begin
	delete from [dbo].[DichVuBenhVienTongHop] where [dbo].[DichVuBenhVienTongHop].DichVuKyThuatBenhVienId = (select d.[Id] from deleted d)
end
Go

DROP TRIGGER IF EXISTS  [dbo].[trg_DichVuKyThuatBenhVien_Into_DichVuBenhVienTongHop_Update];
Go

Create trigger trg_DichVuKyThuatBenhVien_Into_DichVuBenhVienTongHop_Update on [dbo].[DichVuKyThuatBenhVien] after update as
begin
	update [dbo].[DichVuBenhVienTongHop]
	set Ten = T1.Ten, Ma = T1.Ma, HieuLuc = T1.HieuLuc
	
	from [dbo].[DichVuBenhVienTongHop] T inner join 
	(select i.Id, i.[Ten], i.[Ma], i.HieuLuc, i.CreatedById, i.LastUserId, i.CreatedOn, i.LastTime from inserted i) T1 on T.DichVuKyThuatBenhVienId = T1.Id

end
Go
--DVG--
DROP TRIGGER IF EXISTS  [dbo].[trg_DichVuGiuongBenhVien_Into_DichVuBenhVienTongHop_Add];
Go

Create trigger trg_DichVuGiuongBenhVien_Into_DichVuBenhVienTongHop_Add on [dbo].[DichVuGiuongBenhVien] after insert as
begin
	insert into [dbo].[DichVuBenhVienTongHop](LoaiDichVuBenhVien, DichVuGiuongBenhVienId, Ma, Ten, HieuLuc ,CreatedById,LastUserId, CreatedOn, LastTime)
	select 3, i.[Id], i.Ma, i.Ten, i.HieuLuc, i.CreatedById, i.LastUserId, i.CreatedOn, i.LastTime
	from inserted i

end
Go

DROP TRIGGER IF EXISTS  [dbo].[trg_DichVuGiuongBenhVien_Into_DichVuBenhVienTongHop_Delete];
Go

Create trigger trg_DichVuGiuongBenhVien_Into_DichVuBenhVienTongHop_Delete on [dbo].[DichVuGiuongBenhVien] after delete as
begin
	delete from [dbo].[DichVuBenhVienTongHop] where [dbo].[DichVuBenhVienTongHop].DichVuGiuongBenhVienId = (select d.[Id] from deleted d)
end
Go

DROP TRIGGER IF EXISTS  [dbo].[trg_DichVuGiuongBenhVien_Into_DichVuBenhVienTongHop_Update];
Go

Create trigger trg_DichVuGiuongBenhVien_Into_DichVuBenhVienTongHop_Update on [dbo].[DichVuGiuongBenhVien] after update as
begin
	update [dbo].[DichVuBenhVienTongHop]
	set Ten = T1.Ten, Ma = T1.Ma, HieuLuc = T1.HieuLuc
	
	from [dbo].[DichVuBenhVienTongHop] T inner join 
	(select i.Id, i.[Ten], i.[Ma], i.HieuLuc, i.CreatedById, i.LastUserId, i.CreatedOn, i.LastTime from inserted i) T1 on T.DichVuGiuongBenhVienId = T1.Id

end
Go