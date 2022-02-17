using Microsoft.AspNetCore.Mvc;
using BaSyx.Models.Core;
using S.Servies.CacheService;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.Common;

namespace S.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SController : ControllerBase
    {
        private readonly ICacheService cacheService;

        public SController(ICacheService cacheService)
        {
            this.cacheService = cacheService;
        }
        #region Hackathon
        [HttpPost("/createPackage")]
        public IActionResult CreatePackage([FromBody] CreatePackageS packageS)
        {
            string aasIdShort = "DigitalizationPackage";
            Identifier aasId = new Identifier(packageS.AasId, KeyType.IRI); 
            var aas = new AssetAdministrationShell(aasIdShort, aasId);

            string projectMetaSubmodelIdShort = "ProjectMeta"; 
            Identifier projectMetaSubmodelId = new Identifier(packageS.MetaSubmodelId, KeyType.IRI);
            var projectMetaSubmodel = new Submodel(projectMetaSubmodelIdShort, projectMetaSubmodelId);
            projectMetaSubmodel.SemanticId = new Reference(new Key(KeyElements.GlobalReference, KeyType.IRI, "https://ns.imfid.org/aas/ProjectMeta", false));

             var properties = new Dictionary<string, (string, string)>()
            {
                { "PlantProjectCode", (packageS.PlantProjectCode, "https://ns.imfic.org/aas/PlantProjectCode") },
                { "ProjectNo", (packageS.ProjectNo, "https://ns.imfic.org/aas/ProjectNo") },
                { "ContractorCode", (packageS.ContractorCode, "https://ns.imfic.org/aas/ContractorCode") },
                { "PlantCode", (packageS.PlantCode, "https://ns.imfic.org/aas/PlantCode") }
            };
            var stringDataType = new DataType(DataObjectType.String);

            foreach( var prop in properties)
            {
                string idShort = prop.Key;
                string propValue = prop.Value.Item1;
                string semanticId = prop.Value.Item2;

                var property = new Property(idShort, stringDataType, propValue);
                property.SemanticId = new Reference(new GlobalKey(KeyElements.GlobalReference, KeyType.IRI, semanticId)); ;
                projectMetaSubmodel.SubmodelElements.Add(property);
            }

            string payloadsSubmodelIdShort = "Payloads";
            Identifier payloadsSubmodelId = new Identifier(packageS.PayloadsSubmodelId, KeyType.IRI);
            var payloadSubmodel = new Submodel(payloadsSubmodelIdShort, payloadsSubmodelId);
            payloadSubmodel.SemanticId = new Reference(new Key(KeyElements.GlobalReference, KeyType.IRI, "https://ns.imfid.org/aas/SM_Payloads", false));


            aas.Submodels.Add(payloadSubmodel);
            aas.Submodels.Add(projectMetaSubmodel);

            cacheService.AddAssetAdministrationshell(packageS.TicketID, aas); 

            return Ok(packageS.TicketID);
        }
        [HttpPost("/{ticketId}/file")]
        public IActionResult AddFile(string ticketId, 
                                    [FromForm] string PayloadType,
                                    [FromForm] IFormFile File
            )
        {
            cacheService.AddFile(ticketId, PayloadType, File);
            
            return Ok();
        }
        [HttpGet("/{ticketId}/aasx")]
        public IActionResult GetAasx(string ticketId)
        {
            
            return Ok();
        }

        [HttpDelete("/{ticketId}")]
        public IActionResult DeleteTicket(string ticketId)
        {
            return Ok();
        }



        #endregion
    }
}