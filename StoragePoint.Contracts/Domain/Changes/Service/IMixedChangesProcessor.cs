namespace StoragePoint.Contracts.Domain.Changes.Service
{
    using System.Collections.Generic;

    using StoragePoint.Contracts.Domain.Changes.Model;

    public interface IMixedChangesProcessor
    {
        IList<ChangedFile> SplitChanges(MixedChanges mixedChanges);
    }
}