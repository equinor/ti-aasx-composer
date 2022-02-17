using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.Common;
using System.Text.RegularExpressions;

namespace S.Servies.CacheService
{
    class CacheService : ICacheService
    {
        public Dictionary<string, AssetAdministrationShell> AasDictionary = new();
        public Dictionary<string, Dictionary<string, IFormFile>> Files = new(); 

        public AssetAdministrationShell GetAASByTickedId(string tickedId)
        {
            return AasDictionary[tickedId];
        }
        public void UpdateAASByTicketId(string ticketId, AssetAdministrationShell aas)
        {
            AasDictionary[ticketId] = aas;
        }

        public void AddAssetAdministrationshell(string TicketId, AssetAdministrationShell aas)
        {
            if (AasDictionary.ContainsKey(TicketId))
            {
                throw new Exception("Server Error. An AAS with this Ticket ID already exists."); 
            }
            AasDictionary[TicketId] = aas;
            Files[TicketId] = new Dictionary<string, IFormFile>();
        }

        public void AddFile(string TicketId, string payloadType, IFormFile file)
        {

            if (Files[TicketId].ContainsKey(file.FileName))
            {
                throw new Exception("Error: Duplicate files with same name");
            }
            Files[TicketId].Add(file.FileName, file);

            var aas = GetAASByTickedId(TicketId);
            var payloadsSubmodel = aas.Submodels["Payloads"];

            string idShort = $"Payload_{Regex.Replace(file.FileName, "[^a-zA-Z0-9]", "")}";
            var submodelElementCollection = new SubmodelElementCollection(idShort);
            string submodelCollectionSemanticId = "https://ns.imfid.org/aas/SMC_Payload"; 
            submodelElementCollection.SemanticId = new Reference(new GlobalKey(KeyElements.GlobalReference, KeyType.IRI, submodelCollectionSemanticId));

            var stringDataType = new DataType(DataObjectType.String);

            var payloadTypeProperty = new Property("PayloadType", stringDataType, payloadType);
            string payloadTypeSemanticId = "https://ns.imfid.org/aas/Prop_PayloadType";
            payloadTypeProperty.SemanticId = new Reference(new GlobalKey(KeyElements.GlobalReference, KeyType.IRI, payloadTypeSemanticId));
            submodelElementCollection.Add(payloadTypeProperty);
            
            var payloadFile = new BaSyx.Models.Core.AssetAdministrationShell.Implementations.File("PayloadFile");
            string payloadFileSemanticId = "https://ns.imfid.org/aas/PayloadFile"; 
            payloadFile.SemanticId = new Reference(new GlobalKey(KeyElements.GlobalReference, KeyType.IRI, payloadFileSemanticId));
            payloadFile.Value = $"/aasx/{file.FileName}";
            payloadFile.MimeType = file.ContentType; 
            submodelElementCollection.Add(payloadFile);

            aas.Submodels["Payloads"].SubmodelElements.Add(submodelElementCollection); 
            
            UpdateAASByTicketId(TicketId, aas);
        }

        public List<IFormFile> GetFilesByTicketId(string ticketId)
        {
            return new List<IFormFile>(Files[ticketId].Values);
        }
    }
}
