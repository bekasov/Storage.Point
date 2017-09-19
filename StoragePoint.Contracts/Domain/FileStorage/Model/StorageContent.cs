namespace StoragePoint.Contracts.Domain.FileStorage.Model
{
    using System.Collections.Generic;

    public class StorageContent
    {
        public StorageContent(int storageId, IReadOnlyList<FileModel> files)
        {
            this.StorageId = storageId;
            this.Files = files;
        }

        public int StorageId { get; }

        public IReadOnlyList<FileModel> Files { get; }
    }
}