using System;
using System.Collections.Generic;
using System.Text;

namespace WDN.Blob.ArchiveManagement.Models
{
    public class BlobHydrateModel
    {
        public string BlobName { get; set; }
        public string ContainerName { get; set; }
        public DateTime HydrateRequestDateTime { get; set; }
    }
}
