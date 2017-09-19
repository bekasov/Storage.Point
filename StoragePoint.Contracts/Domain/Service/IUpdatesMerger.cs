namespace StoragePoint.Contracts.Domain.Service
{
    using System.Collections.Generic;

    using StoragePoint.Contracts.Domain.FileStorage.Model;

    public interface IUpdatesMerger
    {
        RepositoryUpdates Merge(IReadOnlyList<RepositoryUpdates> updates);
    }
}