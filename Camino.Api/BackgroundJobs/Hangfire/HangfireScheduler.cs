using Hangfire;

namespace Camino.Api.BackgroundJobs.Hangfire
{
    public class HangfireScheduler
    {
        public static void ConfigureRecurringJobs()
        {
            RecurringJob.AddOrUpdate<ISendSmsJob>(job => job.Run(), "0/30 * * * * *");
            RecurringJob.AddOrUpdate<ISendEmailJob>(job => job.Run(), Cron.Minutely);
            RecurringJob.AddOrUpdate<ISendCloudMessageJob>(job => job.Run(), "0/30 * * * * *");
            RecurringJob.AddOrUpdate<IHoanThanhYeuCauTiepNhanJob>(job => job.Run(), "5/30 * * * *");
            RecurringJob.AddOrUpdate<IChuyenYeuCauKhamQuaKetLuanJob>(job => job.Run(), Cron.Minutely);

            RecurringJob.AddOrUpdate<ICapNhatNoiThucHienDichVuKyThuatUuTienJob>(job => job.Run(), Cron.Hourly);
            RecurringJob.AddOrUpdate<IHuyHoatDongNhanVienJob>(job => job.Run(), Cron.Minutely);
            RecurringJob.AddOrUpdate<IXuLyKetQuaXetNghiemJob>(job => job.Run(), "0/15 * * * * *");
            RecurringJob.AddOrUpdate<IKiemTraChoTamUngThemJob>(job => job.Run(), "20 6,12,18 * * *");
            RecurringJob.AddOrUpdate<ITongHopYLenhDichVuKyThuatJob>(job => job.Run(), "0 * * * * *");
            RecurringJob.AddOrUpdate<ITongHopYLenhTruyenMauJob>(job => job.Run(), "15 * * * * *");
            RecurringJob.AddOrUpdate<IQuyetToanGoiDichVuDuocBaoLanh>(job => job.Run(), "13/30 * * * *");
            RecurringJob.AddOrUpdate<ITongHopYLenhVatTuJob>(job => job.Run(), "30 * * * * *");
            RecurringJob.AddOrUpdate<ITongHopYLenhDuocPhamJob>(job => job.Run(), "45 * * * * *");

            //chỉ cần chạy 1 lần
            //RecurringJob.AddOrUpdate<ICapNhatMaDuocPhamBenhVienJob>(job => job.Run(), Cron.Minutely);
            //RecurringJob.AddOrUpdate<IXuLyCapNhatMaVatTuBenhVienJob>(job => job.Run(), Cron.Minutely);

            //BVHD-3754
            RecurringJob.AddOrUpdate<IXacNhanKhongDuocBHYTChiTraJob>(job => job.Run(), "3/10 * * * *");

            //BVHD-3575
            RecurringJob.AddOrUpdate<ITongHopYlenhKhamBenhJob>(job => job.Run(), "59 * * * * *");
        }
    }
}
