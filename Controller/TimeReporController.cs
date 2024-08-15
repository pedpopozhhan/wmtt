using AutoMapper;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCDS.WebFuncions.Core.Context;
using WCDS.WebFuncions.Core.Entity;
using WCDS.WebFuncions.Core.Model;
using WCDS.WebFuncions.Core.Model.Services;
using WCDS.WebFuncions.Core.Services;
using WCDS.WebFuncions.Enums;

namespace WCDS.WebFuncions.Controller
{
    public interface ITimeReportController
    {
        public Task<Response<TimeReportCostDto>> GetApprovedTimeReports(TimeReportCostsRequest request);
        public Task<Response<TimeReportCostDetailDto>> GetTimeReportDetailsByIds(TimeReportDetailsRequest request);
    }

    public class TimeReportController : ITimeReportController
    {
        ApplicationDBContext _dbContext;
        private readonly ITimeReportingService _timeReportingService;
        ILogger _logger;
        IMapper _mapper;

        public TimeReportController(ITimeReportingService timeReportingService, ILogger log, IMapper mapper, ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
            _timeReportingService = timeReportingService;
            _logger = log;
            _mapper = mapper;
        }
        public async Task<Response<TimeReportCostDto>> GetApprovedTimeReports(TimeReportCostsRequest request)
        {
            var costs = await _timeReportingService.GetTimeReportCosts(request.ContractNumber, request.Status);
            if (!string.IsNullOrEmpty(costs.ErrorMessage))
            {
                return costs;
            }

            List<TimeReportCostDto> timeReports = costs.Data;
            if (!string.IsNullOrEmpty(request.InvoiceID))
            {
                var flightReportIds = _dbContext.InvoiceTimeReports.Where(p => p.InvoiceId.ToString() == request.InvoiceID).Select(inv => inv.FlightReportId).Distinct().ToHashSet();
                foreach (var item in flightReportIds)
                {
                    if (timeReports.Where(p => p.FlightReportId == item).Count() > 0)
                        continue;
                    else
                    {
                        TimeReportCostDto timeReportCostDto = new TimeReportCostDto();
                        timeReportCostDto.FlightReportId = item;

                        var invoicesWithConsumedCost = _dbContext.Invoice
                       .Where(p => p.InvoiceStatus == InvoiceStatus.Processed.ToString() || p.InvoiceStatus == InvoiceStatus.Draft.ToString())
                       .Select(inv => new
                       {
                           Invoice = inv,
                           ConsumedCost = inv.InvoiceTimeReportCostDetails.Where(invTRCD => invTRCD.FlightReportId == item).Sum(cd => cd.Cost),
                       }).ToList();

                        var totalConsumedCost = invoicesWithConsumedCost.Sum(o => o.ConsumedCost);
                        timeReportCostDto.TotalCost = totalConsumedCost;

                        var timeReportCostDetail = _dbContext.InvoiceTimeReportCostDetails.Where(p => p.InvoiceId.ToString() == request.InvoiceID && p.FlightReportId == item).FirstOrDefault();
                        timeReportCostDto.FlightReportDate = timeReportCostDetail.FlightReportDate;
                        timeReportCostDto.Ao02Number = timeReportCostDetail.Ao02Number;
                        timeReportCostDto.ContractRegistrationName = timeReportCostDetail.ContractRegistrationName;
                        timeReports.Add(timeReportCostDto);
                    }
                }
            }

            foreach (var item in timeReports)
            {
                var invoicesWithConsumedCost = _dbContext.Invoice
                .Where(p => p.InvoiceStatus == InvoiceStatus.Processed.ToString() || p.InvoiceStatus == InvoiceStatus.Draft.ToString())
                .Select(inv => new
                {
                    Invoice = inv,
                    ConsumedCost = inv.InvoiceTimeReportCostDetails.Where(invTRCD => invTRCD.FlightReportId == item.FlightReportId).Sum(cd => cd.Cost)
                }).ToList();
                var totalConsumedCost = invoicesWithConsumedCost.Sum(o => o.ConsumedCost);
                item.RemainingCost = item.TotalCost - totalConsumedCost;
            }
            costs.Data = timeReports.OrderBy(p => p.FlightReportId).ToList();
            return costs;
        }

        public async Task<Response<TimeReportCostDetailDto>> GetTimeReportDetailsByIds(TimeReportDetailsRequest request)
        {
            var details = await _timeReportingService.GetTimeReportByIds(request.TimeReportIds);            
            if (!string.IsNullOrEmpty(request.InvoiceID))
            {
                var timeReportCostDetails = _dbContext.InvoiceTimeReportCostDetails
                .Where(p => p.InvoiceId.ToString() == request.InvoiceID && request.TimeReportIds.Contains(p.FlightReportId)).ToList();

                foreach (var item in timeReportCostDetails)
                {
                    var mapped = _mapper.Map<InvoiceTimeReportCostDetails, TimeReportCostDetailDto>(item);
                    details.Data.Add(mapped);
                }
            }
            return details;
        }
    }
}
