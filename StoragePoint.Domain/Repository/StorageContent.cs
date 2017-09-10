namespace StoragePoint.Domain.Repository
{
    using System.Collections.Generic;

    using StoragePoint.Domain.Model;

    public class StorageContent
    {
        public IReadOnlyList<FileModel> Files { get; set; }
    }
}