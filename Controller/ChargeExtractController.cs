using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCDS.WebFuncions.Core.Context;
using WCDS.WebFuncions.Core.Model;

namespace WCDS.WebFuncions.Controller
{
    internal class ChargeExtractController : IChargeExtractController
    {
        ApplicationDBContext dbContext;
        ILogger _logger;
        IMapper _mapper;
        private const string DEFAULT_USER = "System";

        public ChargeExtractController(ILogger log, IMapper mapper)
        {
            dbContext = new ApplicationDBContext();
            _logger = log;
            _mapper = mapper;
        }

        public Task<Guid> CreateChargeExtract(ChargeExtractDto chargeExtract)
        {
            throw new NotImplementedException();
        }
    }
}
