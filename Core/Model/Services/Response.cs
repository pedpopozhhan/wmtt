namespace WCDS.WebFuncions.Core.Model.Services
{
    public class Response<T>
    {
        public bool Status { get; set; }
        public string ErrorCodeId { get; set; }
        public string ErrorMessage { get; set; }
        public PaginationResponseInfo PaginationInfo { get; set; }
        public T[] Data { get; set; }
    }
}


/*{
    "status": "true",
    "errorCodeId": "0",
    "errorMessage": "",
    "paginationInfo": {
        "perPage": 2000,
        "page": 1,
        "totalPages": 1,
        "total": 33
    },
    "data": [
        {
            "rateUnitId": "f33834b9-5b4a-433a-a25f-4038806d2601",
            "oracleId": 44,
            "type": "CFU/g",
            "createTimestamp": "1998-04-02T16:35:31",
            "createUserId": "FIRE_PROD",
            "updateTimestamp": null,
            "updateUserId": null,
            "effectiveDate": "2023-06-05T14:30:42",
            "terminationDate": "2024-06-05T14:30:42"
        },
    ]
}*/