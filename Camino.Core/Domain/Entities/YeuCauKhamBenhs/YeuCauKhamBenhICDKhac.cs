using Camino.Core.Domain.Entities.ICDs;

namespace Camino.Core.Domain.Entities.YeuCauKhamBenhs
{
    public class YeuCauKhamBenhICDKhac : BaseEntity
    {
        public long YeuCauKhamBenhId { get; set; }
        public long ICDId { get; set; }
        /// <summary>
        /// Update 30/03/2020
        /// </summary>
        public string GhiChu { get; set; }
        //

        public virtual ICD ICD { get; set; }
        public virtual YeuCauKhamBenh YeuCauKhamBenh { get; set; }
    }
}
