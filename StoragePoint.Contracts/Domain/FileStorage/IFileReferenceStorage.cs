namespace StoragePoint.Contracts.Domain.FileStorage
{
    using StoragePoint.Contracts.Domain.Changes.Model;

    public interface IFileReferenceStorage : IFileStorage
    {
        // bool ItHaveBeenSynchedWith(IFileRepository source);

        MixedChanges DetectUpdates(IFileStorage source);


    }
}