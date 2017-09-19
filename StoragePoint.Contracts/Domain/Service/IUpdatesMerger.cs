namespace StoragePoint.Contracts.Domain.Service
{
    using System.Collections.Generic;

    using StoragePoint.Contracts.Domain.FileStorage.Model;

    public interface IUpdatesMerger
    {
        StorageUpdates Merge(IReadOnlyList<StorageUpdates> updates);
    }
}