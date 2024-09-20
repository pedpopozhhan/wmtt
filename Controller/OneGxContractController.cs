using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WCDS.WebFuncions.Core.Context;
using WCDS.WebFuncions.Core.Entity;
using WCDS.WebFuncions.Core.Model;
using WCDS.WebFuncions.Core.Model.ContractManagement;


namespace WCDS.WebFuncions.Controller
{
    public interface IOneGxContractController
    {
        public Task<OneGxContractDetailDto> CreateOneGxContractDetail(OneGxContractDetailDto oneGxContractDetail, string user);
        public Task<OneGxContractDetailDto> UpdateOneGxContractDetail(OneGxContractDetailDto oneGxContractDetail, string user);
    }
    public class OneGxContractController : IOneGxContractController
    {
        ApplicationDBContext _dbContext;
        ILogger _logger;
        IMapper _mapper;
        
        public OneGxContractController(ILogger log, IMapper mapper, ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
            _logger = log;
            _mapper = mapper;
        }

        public async Task<OneGxContractDetailDto> CreateOneGxContractDetail(OneGxContractDetailDto oneGxContractDetail, string user)
        {
            var dt = DateTime.UtcNow;
            IDbContextTransaction transaction = _dbContext.Database.BeginTransaction();
            try
            {
                if (!oneGxContractDetail.OneGxContractId.HasValue || oneGxContractDetail.OneGxContractId == Guid.Empty)
                {
                    var entity = _mapper.Map<OneGxContractDetail>(oneGxContractDetail);
                    entity.CreatedByDateTime = dt;
                    entity.CreatedBy = user;
                    entity.UpdatedByDateTime = dt;
                    entity.UpdatedBy = user;
                    _dbContext.OneGxContractDetail.Add(entity);                   
                    _dbContext.SaveChanges();                    
                    transaction.Commit();                   
                    return _mapper.Map<OneGxContractDetail, OneGxContractDetailDto>(entity);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("CreateOneGxContractDetail: An error has occured while Creating detail record for contract number:  {0}, ErrorMessage: {1}, InnerException: {2}", oneGxContractDetail.ContractNumber, ex.Message, ex.InnerException));
                transaction.Rollback();
                throw;
            }
        }

        public async Task<OneGxContractDetailDto> UpdateOneGxContractDetail(OneGxContractDetailDto oneGxContractDetail, string user)
        {
            var dt = DateTime.UtcNow;
            IDbContextTransaction transaction = _dbContext.Database.BeginTransaction();
            try
            {
                if (oneGxContractDetail.OneGxContractId.HasValue && oneGxContractDetail.OneGxContractId != Guid.Empty)
                {
                    var entity = _mapper.Map<OneGxContractDetail>(oneGxContractDetail);
                    entity.UpdatedByDateTime = dt;
                    entity.UpdatedBy = user;
                    _dbContext.OneGxContractDetail.Update(entity);
                    _dbContext.SaveChanges();
                    transaction.Commit();
                    return _mapper.Map<OneGxContractDetail, OneGxContractDetailDto>(entity);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("CreateOneGxContractDetail: An error has occured while Updating detail record for contract number:  {0}, ErrorMessage: {1}, InnerException: {2}", oneGxContractDetail.ContractNumber, ex.Message, ex.InnerException));
                transaction.Rollback();
                throw;
            }
        }
    }
}
