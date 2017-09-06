using StoragePoint.Domain;

namespace StoragePoint.UnitTests
{
    public class SyncService
    {
        public void Sync(IFileRepository source, IFileRepository destination)
        {
            if (destination.IsRootEmpty)
            {
                destination.GetAllFrom(source);
            }
        }
    }
}