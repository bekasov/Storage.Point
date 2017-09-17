namespace StoragePoint.Domain.Service
{
    using System.Collections.Generic;

    using StoragePoint.Domain.Model;

    public interface IUpdatesMerger
    {
        RepositoryUpdates Merge(IReadOnlyList<RepositoryUpdates> updates);
    }
}