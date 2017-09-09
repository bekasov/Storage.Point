namespace StoragePoint.Domain
{
    using System.Collections.Generic;

    using StoragePoint.Domain.Model;

    public interface IDifferencesMerger
    {
        RepositoryUpdates Merge(IReadOnlyList<RepositoryUpdates> differences);
    }
}