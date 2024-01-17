namespace WCDS.WebFuncions.Core.Model.Services
{
    public class FlightReportIds
    {
        public int[] FlightReportId { get; set; }
        public FlightReportIds(int[] ids)
        {
            FlightReportId = ids;
        }
    }
}
// export interface IDetailsTableRowData {
//   date: Date;
//   registrationNumber: string;
//   reportNumber: number;
//   aO02Number: string;
//   rateType: string;
//   numberOfUnits: number;
//   rateUnit: string;
//   ratePerUnit: number; //with $0.00
//   cost: number; //with $0.00
//   glAccountNumber: number;
//   profitCentre: string;
//   costCentre: string;
//   fireNumber: string;
//   internalOrder: string;
//   fund: number;
// }
// { 
//   "sortBy": "flightReportDate",
//   "sortOrder": "DESC",
//   "filterBy": {
//     "columnName": "",
//     "columnValue": "14AFD206",
//     "flightReportIds": {
//       "flightReportId": [
//         505, 506
//       ] 
//     }    
//   },
//   "paginationInfo": {
//     "perPage": 20,
//     "page": 1
//   }
// }

// {
//     "status": "true",
//     "errorCodeId": "0",
//     "errorMessage": "",
//     "paginationInfo": {
//         "perPage": 20,
//         "page": 1,
//         "totalPages": 1,
//         "total": 17
//     },
//     "data": [
//         {
//             "flightReportId": 506,
//             "flightReportDate": "2023-12-12T00:00:00",
//             "ao02Number": "5W6990",
//             "status": "Signed off",
//             "contractRegistrationId": 1083,
//             "contractRegistrationName": "FHLF",
//             "contractId": 81788,
//             "contractNumber": "20AFD557",
//             "vendorId": "558d5367-b1ee-42d7-b40b-2d2faf7bb6e0",
//             "vendorName": "PHOENIX HELI-FLIGHT INC.",
//             "financeVendorId": "0020093921",
//             "rateTypeId": "e7ca0d8d-135a-4753-92c0-80620ed65f25",
//             "noOfUnits": 1.00,
//             "rateUnitId": "6fc9d754-5fd0-4b79-b2fe-8e0d4629cc27",
//             "cost": 1000.00,
//             "account": "72",
//             "costCenter": "5",
//             "fireNumber": "GWF001",
//             "internalOrder": "5",
//             "fund": "6",
//             "ratePerUnit": 1000.00
//         },