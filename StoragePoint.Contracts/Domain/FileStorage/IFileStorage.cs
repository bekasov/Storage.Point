namespace StoragePoint.Contracts.Domain.FileStorage
{
    using System.Collections.Generic;

    using StoragePoint.Contracts.Domain.Changes.Model;
    using StoragePoint.Contracts.Domain.FileStorage.Model;

    public interface IFileStorage
    {
        bool IsInitialized { get; }

        void CopyAll(IFileStorage source);

        StorageContent GetAll();

        void Update(IReadOnlyList<ChangedFile> changes);
    }
}