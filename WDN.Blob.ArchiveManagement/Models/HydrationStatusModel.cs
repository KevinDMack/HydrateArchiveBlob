using System;
using System.Collections.Generic;
using System.Text;

namespace WDN.Blob.ArchiveManagement.Models
{
    public enum HydrationStatus
    {
        NotHydrated,
        Hydrated
    }
    
    public class HydrationStatusModel
    {
        public string BlobName { get; set; }
        public string ContainerName { get; set; }
        public DateTime HydrateRequestDateTime { get; set; }
        public DateTime? HydratedFileDataTime { get; set; }

        public HydrationStatus Status { get; set; }

        public void Hydrate(BlobHydrateModel model)
        {
            this.BlobName = model.BlobName;
            this.ContainerName = model.ContainerName;
            this.HydrateRequestDateTime = model.HydrateRequestDateTime;
        }
    }
}
