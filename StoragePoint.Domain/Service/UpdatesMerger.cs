namespace StoragePoint.Domain.Service
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using StoragePoint.Contracts.Domain.FileStorage.Model;
    using StoragePoint.Contracts.Domain.FileStorage.Pto;
    using StoragePoint.Contracts.Domain.Service;
    using StoragePoint.Contracts.Infrastructure.Service;

    public class UpdatesMerger : IUpdatesMerger
    {
        private readonly IPathBuilder pathBuilder;

        public UpdatesMerger(IPathBuilder pathBuilder)
        {
            this.pathBuilder = pathBuilder;
        }

        public StorageUpdates Merge(IReadOnlyList<StorageUpdates> updates)
        {
            List<FullPathFilePto> allAdded = new List<FullPathFilePto>();
            List<FullPathFilePto> allRemoved = new List<FullPathFilePto>();
            List<FullPathFilePto> allChanged = new List<FullPathFilePto>();
            List<FullPathFilePto> allRenamed = new List<FullPathFilePto>();
            List<FullPathFilePto> allMoved = new List<FullPathFilePto>();

            Parallel.ForEach(
                updates, 
                u => 
                {
                    allAdded.AddRange(this.pathBuilder.GetPaths(u.Added));
                    allRemoved.AddRange(this.pathBuilder.GetPaths(u.Removed));
                    allChanged.AddRange(this.pathBuilder.GetPaths(u.Changed));
                    allRenamed.AddRange(this.pathBuilder.GetPaths(u.Renamed));
                    allMoved.AddRange(this.pathBuilder.GetPaths(u.Moved));
                });

            return null;
        }
    }
}