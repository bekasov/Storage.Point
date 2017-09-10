namespace StoragePoint.Domain.Repository
{
    using StoragePoint.Domain.Model;

    public interface IFileRepository
    {
        bool IsInitialized { get; }

        void CopyAll(IFileRepository source);

        StorageContent GetAll();

        RepositoryUpdates DetectUpdates(IFileRepository source);

        void Update(RepositoryUpdates updates);
    }
}