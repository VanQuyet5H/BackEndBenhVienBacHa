using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.NhapKhoQuaTangs;
using Camino.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Services.NhapKhoQuaTangMarketing
{
    [ScopedDependency(ServiceType = typeof(INhapKhoQuaTangMarketingService))]
    public partial class NhapKhoQuaTangMarketingService : MasterFileService<NhapKhoQuaTang>, INhapKhoQuaTangMarketingService
    {
        private readonly IRepository<NhapKhoQuaTangChiTiet> _nhapKhoQuaTangChiTietRepository;
        private readonly IRepository<Core.Domain.Entities.Users.User> _userRepository;
        private readonly IRepository<Core.Domain.Entities.QuaTangs.QuaTang> _quaTangRepository;
        public NhapKhoQuaTangMarketingService(IRepository<NhapKhoQuaTang> repository , 
            IRepository<NhapKhoQuaTangChiTiet> nhapKhoQuaTangChiTietRepository,
            IRepository<Core.Domain.Entities.Users.User> userRepository,
            IRepository<Core.Domain.Entities.QuaTangs.QuaTang> quaTangRepository) : base(repository)
        {
            _nhapKhoQuaTangChiTietRepository = nhapKhoQuaTangChiTietRepository;
            _userRepository = userRepository;
            _quaTangRepository = quaTangRepository;
        }
    }
}
