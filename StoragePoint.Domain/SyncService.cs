namespace StoragePoint.Domain
{
    using System;

    public class SyncService
    {
        private readonly IDifferencesMerger differencesMerger;

        public SyncService(IDifferencesMerger differencesMerger)
        {
            this.differencesMerger = differencesMerger;
        }

        public void Sync(IFileRepository source, IFileRepository destination, IFileRepository syncReference = null)
        {
            if (destination.IsRootEmpty)
            {
                destination.GetAllFrom(source);
                return;
            }

            if (syncReference == null)
            {
                throw new ArgumentNullException(nameof(syncReference));
            }

            RepositoryUpdates sourceUpdates = syncReference.DetectUpdates(source);
            RepositoryUpdates destinationUpdates = syncReference.DetectUpdates(destination);

            RepositoryUpdates mergedUpdates = this.differencesMerger.Merge(new[] { sourceUpdates, destinationUpdates });
        }
    }
}