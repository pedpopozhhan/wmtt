using System;


namespace WCDS.WebFuncions.Core.Model.Services
{
    public class DomainBase
    {
        public int OracleId { get; set; }
        public string Type { get; set; }
        public DateTime CreateTimestamp { get; set; }
        public string CreateUserId { get; set; }
        public DateTime? UpdateTimestamp { get; set; }
        public string UpdateUserId { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? TerminationDate { get; set; }
    }
}


/*
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

}*/