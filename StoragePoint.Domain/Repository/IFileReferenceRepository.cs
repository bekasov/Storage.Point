namespace StoragePoint.Domain.Repository
{
    public interface IFileReferenceRepository : IFileRepository
    {
        bool IsInitialized { get; }

        bool ItHaveBeenSynchedWith(IFileRepository source);
    }
}