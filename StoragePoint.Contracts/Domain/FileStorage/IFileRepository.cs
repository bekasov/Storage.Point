namespace StoragePoint.Contracts.Domain.FileStorage
{
    using StoragePoint.Contracts.Domain.FileStorage.Model;

    public interface IFileRepository
    {
        bool IsInitialized { get; }

        void CopyAll(IFileRepository source);

        StorageContent GetAll();

        RepositoryUpdates DetectUpdates(IFileRepository source);

        void Update(RepositoryUpdates updates);
    }
}