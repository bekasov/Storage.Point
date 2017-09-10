namespace StoragePoint.Domain.Model
{
    using System.Collections.Generic;

    public class RepositoryUpdates
    {
        public IReadOnlyList<FileModel> Added { get; set; }

        public IReadOnlyList<FileModel> Removed { get; set; }

        public IReadOnlyList<FileModel> ChangedContent { get; set; }

        public IReadOnlyList<FileModel> Renamed { get; set; }
    }
}