namespace StoragePoint.Domain.Repository
{
    using System.Collections.Generic;

    using StoragePoint.Domain.Model;

    public class StorageContent
    {
        public StorageContent(IReadOnlyList<StorageContent> subFolders = null, IReadOnlyList<FileModel> files = null)
        {
            this.SubFolders = subFolders ?? new StorageContent[0];
            this.Files = files ?? new FileModel[0];
        }

        public IReadOnlyList<StorageContent> SubFolders { get; private set; }

        public IReadOnlyList<FileModel> Files { get; private set; }
    }
}