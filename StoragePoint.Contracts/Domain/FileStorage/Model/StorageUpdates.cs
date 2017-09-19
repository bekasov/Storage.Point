namespace StoragePoint.Contracts.Domain.FileStorage.Model
{
    using System.Collections.Generic;

    public class StorageUpdates
    {
        public StorageUpdates(
            int storageId, 
            IReadOnlyList<FileModel> added, 
            IReadOnlyList<FileModel> removed, 
            IReadOnlyList<FileModel> changed, 
            IReadOnlyList<FileModel> renamed, 
            IReadOnlyList<FileModel> moved)
        {
            this.StorageId = storageId;
            this.Added = added;
            this.Removed = removed;
            this.Changed = changed;
            this.Renamed = renamed;
            this.Moved = moved;
        }

        public int StorageId { get; }

        public IReadOnlyList<FileModel> Added { get; }

        public IReadOnlyList<FileModel> Removed { get; }

        public IReadOnlyList<FileModel> Changed { get; }

        public IReadOnlyList<FileModel> Renamed { get; }

        public IReadOnlyList<FileModel> Moved { get; }
    }
}