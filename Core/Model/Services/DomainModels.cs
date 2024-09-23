namespace WCDS.WebFuncions.Core.Model.Services
{
    public class RateType : DomainServiceModelsBase
    {
        public string RateTypeId { get; set; }
    }
    public class RateUnit : DomainServiceModelsBase
    {
        public string RateUnitId { get; set; }
    }

    public class CorporateRegion : DomainServiceModelsBase
    {
        public string CorporateRegionId { get; set; }
        public string CorporateRegionTypeId { get; }
        public string Name { get; set; }
    }
}