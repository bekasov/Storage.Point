namespace StoragePoint.Domain.Service
{
    using StoragePoint.Domain.Model;
    using StoragePoint.Domain.Repository;

    public class UpdatesDetector
    {
        public RepositoryUpdates Detect(StorageContent referenceContent, StorageContent source)
        {
            RepositoryUpdates result = new RepositoryUpdates();

            return result;
        }
    }
}