using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Export;
using Microsoft.AspNetCore.Mvc;
using AASc.Servies.CacheService;
using System.IO.Packaging;
using System.Timers;

namespace AASc.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AAScController : ControllerBase
    {
        private readonly ICacheService cacheService;

        public AAScController(ICacheService cacheService)
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
                { "PlantProjectCode", (packageS.PlantProjectCode, "https://ns.imfid.org/aas/PlantProjectCode") },
                { "ProjectNo", (packageS.ProjectNo, "https://ns.imfid.org/aas/ProjectNo") },
                { "ContractorCode", (packageS.ContractorCode, "https://ns.imfid.org/aas/ContractorCode") },
                { "PlantCode", (packageS.PlantCode, "https://ns.imfid.org/aas/PlantCode") }
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
            return Ok("File has been successfully uploaded");
        }
        [HttpGet("/{ticketId}/aasx")]
        public IActionResult GetAasx(string ticketId)
{
            IAssetAdministrationShell aas = cacheService.GetAASByTickedId(ticketId);
            string aasxFileName = aas.IdShort + ".aasx";

            var files = cacheService.GetFilesByTicketId(ticketId);

            using (var packageStream = new MemoryStream())
            {
                using var package = Package.Open(packageStream, FileMode.Create, FileAccess.ReadWrite);

                using (AASX aasx = new AASX(package))
                {
                    AssetAdministrationShellEnvironment_V2_0 env = new AssetAdministrationShellEnvironment_V2_0(aas);
                    aasx.AddEnvironment(aas.Identification, env, ExportType.Xml);

                    foreach (var file in files)
                    {

                        //Had to copy stream to MemoryStream to get read/write access
                        var fileStream = new MemoryStream(file.PayloadData);
                        aasx.AddStreamToAASX($"/aasx/{file.FileName}", fileStream, file.ContentType);

                    }
                }

                packageStream.Position = 0;

                return File(packageStream.ToArray(), "application/asset-administration-shell-package", aasxFileName);
            }

        }

        [HttpDelete("/{ticketId}")]
        public IActionResult DeleteTicket(string ticketId)
        {
            var ok = cacheService.DeleteTicket(ticketId);
            if(ok)
                return Ok($"Ticket {ticketId} successfully removed");
            return BadRequest("Error: deleting ticket has failed.");
        }
        #endregion
    }
}