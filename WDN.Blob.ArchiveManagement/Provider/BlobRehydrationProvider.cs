using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using WDN.Blob.ArchiveManagement.Models;

namespace WDN.Blob.ArchiveManagement.Provider
{
    public class BlobRehydrationProvider : IBlobRehydrationProvider
    {
        private string _cs;
        private double _visibilityTimeout;
        public BlobRehydrationProvider(string cs, double visibilityTimeOut)
        {
            _cs = cs;
            _visibilityTimeout = visibilityTimeOut;
        }

        public void RehydrateBlob(string containerName, string blobName, string queueName)
        {
            var accountClient = new BlobServiceClient(_cs);

            var containerClient = accountClient.GetBlobContainerClient(containerName);

            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            blobClient.SetAccessTier(AccessTier.Hot);

            var model = new BlobHydrateModel() { BlobName = blobName, ContainerName = containerName, HydrateRequestDateTime = DateTime.Now };

            QueueClient queueClient = new QueueClient(_cs, queueName);
            var json = JsonConvert.SerializeObject(model);
            string requeueMessage = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
            queueClient.SendMessage(requeueMessage);
        }

        public HydrationStatusModel CheckHydrationStatus(BlobHydrateModel model, string queueName)
        {
            var ret = new HydrationStatusModel();
            ret.Hydrate(model);

            var accountClient = new BlobServiceClient(_cs);

            var containerClient = accountClient.GetBlobContainerClient(model.ContainerName);

            BlobClient blobClient = containerClient.GetBlobClient(model.BlobName);

            var properties = blobClient.GetProperties();
            if (properties.Value.ArchiveStatus == "rehydrate-pending-to-hot")
            {
                ret.Status = HydrationStatus.NotHydrated;

                QueueClient queueClient = new QueueClient(_cs, queueName);
                var json = JsonConvert.SerializeObject(model);
                string requeueMessage = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
                queueClient.SendMessage(requeueMessage, visibilityTimeout: TimeSpan.FromMinutes(_visibilityTimeout));
            }
            else
            {
                ret.Status = HydrationStatus.Hydrated;
                ret.HydratedFileDataTime = DateTime.Now;
            }

            return ret;
        }
    }
}
