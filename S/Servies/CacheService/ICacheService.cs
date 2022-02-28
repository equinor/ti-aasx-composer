using BaSyx.Models.Core.AssetAdministrationShell.Implementations;

namespace AASc.Servies.CacheService
{
    public interface ICacheService
    {

        AssetAdministrationShell GetAASByTickedId(string tickedId);

        void UpdateAASByTicketId(string ticketId, AssetAdministrationShell aas); 
        public void AddAssetAdministrationshell(string TicketId, AssetAdministrationShell aas);
        public void AddFile(string TicketId, string PayloadType, IFormFile file);
        public List<Payload> GetFilesByTicketId(string ticketId);
        public bool DeleteTicket(string TicketId);

    }
}
