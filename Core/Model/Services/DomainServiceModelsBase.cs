using System;


namespace WCDS.WebFuncions.Core.Model.Services
{
    public class DomainServiceModelsBase
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


