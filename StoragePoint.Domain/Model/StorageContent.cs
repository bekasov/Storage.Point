namespace StoragePoint.Domain.Model
{
    using System.Collections.Generic;

    public class StorageContent
    {
        public IReadOnlyList<FileModel> Files { get; set; }
    }
}