namespace Camino.Core.Domain.Entities.ICDs
{
    public class ChuanDoanLienKetICD : BaseEntity
    {
        public long ChuanDoanId { get; set; }
        public long ICDId { get; set; }

        public virtual ICD ICD { get; set; }
        public virtual ChuanDoan ChuanDoan { get; set; }
    }
}
