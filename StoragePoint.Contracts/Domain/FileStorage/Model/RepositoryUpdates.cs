namespace StoragePoint.Contracts.Domain.FileStorage.Model
{
    using System.Collections.Generic;

    public class RepositoryUpdates
    {
        public IReadOnlyList<FileModel> Added { get; set; }

        public IReadOnlyList<FileModel> Removed { get; set; }

        public IReadOnlyList<FileModel> Changed { get; set; }

        public IReadOnlyList<FileModel> Renamed { get; set; }

        public IReadOnlyList<FileModel> Moved { get; set; }
    }
}