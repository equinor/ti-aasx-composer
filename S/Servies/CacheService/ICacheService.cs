using BaSyx.Models.Core.AssetAdministrationShell.Implementations;

namespace S.Servies.CacheService
{
    public interface ICacheService
    {

        AssetAdministrationShell GetAASByTickedId(string tickedId);

        void UpdateAASByTicketId(string ticketId, AssetAdministrationShell aas); 
        public void AddAssetAdministrationshell(string TicketId, AssetAdministrationShell aas);
        public void AddFile(string TicketId, string PayloadType, IFormFile file);
        public List<Payload> GetFilesByTicketId(string ticketId);

    }
}
