using AutoMapper;
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
            // are any of the costs, flightReportIds in the invoiceTimeReport table? and is that invoice a draft
            var allFlightReportIds = costs.Data.Select(x => x.FlightReportId).Distinct();
            var flightReportIdsSelected = new List<int>();

            foreach (var flightReportId in allFlightReportIds)
            {
                var itrs = await _dbContext.InvoiceTimeReports.Where(x => x.FlightReportId == flightReportId).ToListAsync();
                if (itrs.Count == 0)
                {
                    continue;
                }
                foreach (var itr in itrs)
                {
                    var invoice = await _dbContext.Invoice.Where(x => x.InvoiceId == itr.InvoiceId &&
                    (x.InvoiceStatus == InvoiceStatus.Draft.ToString() || x.InvoiceStatus == InvoiceStatus.Processed.ToString()))
                    .FirstOrDefaultAsync();
                    if (invoice != null)
                    {
                        //remove this flightreport from costs.data
                        flightReportIdsSelected.Add(flightReportId);
                    }
                }
            }
            // if the flight report is selected, mark the costs as selected
            foreach (var cost in costs.Data)
            {
                if (flightReportIdsSelected.Contains(cost.FlightReportId))
                {
                    cost.IsInUse = true;
                }
                else
                {
                    cost.IsInUse = false;
                }
            }
            //costs.Data.RemoveAll(x => flightReportIdsToRemove.Contains(x.FlightReportId));

            // Updating the remaining cost if any of the cost details are already consumed
            List<TimeReportCostDto> timeReports = costs.Data;
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
            costs.Data = timeReports;
            return costs;
        }
    }
}
