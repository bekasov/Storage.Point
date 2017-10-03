namespace StoragePoint.Contracts.Domain.FileStorage
{
    using StoragePoint.Contracts.Domain.Changes.Model;
    using StoragePoint.Contracts.Domain.FileStorage.Model;

    public interface IFileRepository
    {
        bool IsInitialized { get; }

        void CopyAll(IFileRepository source);

        StorageContent GetAll();

        MixedChanges DetectUpdates(IFileRepository source);

        void Update(MixedChanges changes);
    }
}