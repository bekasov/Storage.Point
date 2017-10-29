namespace StoragePoint.Contracts.Domain.Changes.Service
{
    using System.Collections.Generic;

    using StoragePoint.Contracts.Domain.Changes.Model;

    public interface IMixedChangesSplitter
    {
        IReadOnlyList<ChangedFile> Split(MixedChanges changes);
    }
}