using WDN.Blob.ArchiveManagement.Models;

namespace WDN.Blob.ArchiveManagement.Provider
{
    public interface IBlobRehydrationProvider
    {
        HydrationStatusModel CheckHydrationStatus(BlobHydrateModel model, string queueName);
        void RehydrateBlob(string containerName, string blobName, string queueName);
    }
}