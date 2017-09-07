namespace StoragePoint.Domain
{
    public interface IFileRepository
    {
        bool IsRootEmpty { get; }

        void GetAllFrom(IFileRepository source);

        RepositoryUpdates DetectUpdates(IFileRepository source);
    }
}