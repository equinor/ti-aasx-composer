using System;
using System.Collections.Generic;
using System.Text;

namespace AASc
{
    public class CreatePackageS
    {
        public string PlantProjectCode { get; set; }
        public string ProjectNo { get; set; }
        public string ContractorCode { get; set; }
        public string PlantCode { get; set; }
        public string MetaSubmodelId { get; set; }
        public string AasId { get; set; }
        public string AssetId { get; set; }
        public string PayloadsSubmodelId { get; set; }

        public string TicketID
        {
            get;
        }

        public CreatePackageS(
              string plantProjectCode,
              string projectNo,
              string contractorCode,
              string plantCode,
              string metaSubmodelId,
              string aasid,
              string assetId,
              string payloadsSubmodelId)
        {
            TicketID = Guid.NewGuid().ToString();
            PlantProjectCode = plantProjectCode;
            MetaSubmodelId = metaSubmodelId;
            PlantCode = plantCode;
            ContractorCode = contractorCode;
            ProjectNo = projectNo;
            AasId = aasid;
            AssetId = assetId;
            PayloadsSubmodelId = payloadsSubmodelId;
        }

    }
}
