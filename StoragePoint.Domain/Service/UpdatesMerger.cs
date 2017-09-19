namespace StoragePoint.Domain.Service
{
    using System.Collections.Generic;
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

        public RepositoryUpdates Merge(IReadOnlyList<RepositoryUpdates> updates)
        {
            List<FullPathFilePto> added = new List<FullPathFilePto>();
            List<FullPathFilePto> removed = new List<FullPathFilePto>();
            List<FullPathFilePto> changed = new List<FullPathFilePto>();
            List<FullPathFilePto> renamed = new List<FullPathFilePto>();
            List<FullPathFilePto> moved = new List<FullPathFilePto>();

            Parallel.ForEach(
                updates, 
                u => 
                {
                    added.AddRange(this.pathBuilder.GetPaths(u.Added));
                    removed.AddRange(this.pathBuilder.GetPaths(u.Removed));
                    changed.AddRange(this.pathBuilder.GetPaths(u.Changed));
                    renamed.AddRange(this.pathBuilder.GetPaths(u.Renamed));
                    moved.AddRange(this.pathBuilder.GetPaths(u.Moved));
                });

            return null;
        }
    }
}