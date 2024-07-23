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
        ApplicationDBContext dbContext;
        private readonly ITimeReportingService _timeReportingService;
        ILogger _logger;
        IMapper _mapper;

        public TimeReportController(ITimeReportingService timeReportingService, ILogger log, IMapper mapper)
        {
            dbContext = new ApplicationDBContext();
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
                var itrs = await dbContext.InvoiceTimeReports.Where(x => x.FlightReportId == flightReportId).ToListAsync();
                if (itrs.Count == 0)
                {
                    continue;
                }
                foreach (var itr in itrs)
                {
                    var invoice = await dbContext.Invoice.Where(x => x.InvoiceId == itr.InvoiceId &&
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
            // costs.Data.RemoveAll(x => flightReportIdsSelected.Contains(x.FlightReportId));
            return costs;
        }

    }
}
