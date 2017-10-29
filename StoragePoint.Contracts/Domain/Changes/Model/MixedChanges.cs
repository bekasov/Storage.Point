namespace StoragePoint.Contracts.Domain.Changes.Model
{
    using System.Collections.Generic;

    using StoragePoint.Contracts.Domain.FileStorage.Model;

    public class MixedChanges
    {
        public MixedChanges(
            // int storageId, 
            IReadOnlyList<FileModel> added, 
            IReadOnlyList<FileModel> removed, 
            IReadOnlyList<FileModel> updated, 
            IReadOnlyList<FileModel> renamed, 
            IReadOnlyList<FileModel> moved)
        {
            // this.StorageId = storageId;
            this.Added = added;
            this.Removed = removed;
            this.Updated = updated;
            this.Renamed = renamed;
            this.Moved = moved;
        }

        // public int StorageId { get; }

        public IReadOnlyList<FileModel> Added { get; }

        public IReadOnlyList<FileModel> Removed { get; }

        public IReadOnlyList<FileModel> Updated { get; }

        public IReadOnlyList<FileModel> Renamed { get; }

        public IReadOnlyList<FileModel> Moved { get; }
    }
}