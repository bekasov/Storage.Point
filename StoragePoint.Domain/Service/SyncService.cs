namespace StoragePoint.Domain.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using StoragePoint.Domain.Exceptions;
    using StoragePoint.Domain.Model;
    using StoragePoint.Domain.Repository;

    public class SyncService
    {
        private readonly IDifferencesMerger differencesMerger;

        public SyncService(IDifferencesMerger differencesMerger)
        {
            this.differencesMerger = differencesMerger;
        }

        public void Sync(IReadOnlyList<IFileRepository> sources, IFileReferenceRepository syncReference)
        {
            if (sources == null)
            {
                throw new ArgumentNullException(nameof(sources));
            }

            if (syncReference == null)
            {
                throw new ArgumentNullException(nameof(syncReference));
            }

            if (!syncReference.IsInitialized)
            {
                throw new ReferenceNotInitialized();
            }

            if (sources.Any(repository => repository.IsEmpty))
            {
                throw new AllRepositoriesMustBeInitialized();
            }
            
            IList<(IFileRepository Repo, RepositoryUpdates Updates)> reposUpdates = sources
                .Select(repository => (Repo: repository, Updates: syncReference.DetectUpdates(repository)))
                .AsParallel()
                .ToList();

            RepositoryUpdates mergedUpdates = this.differencesMerger.Merge(reposUpdates.Select(u => u.Updates).ToList());

            Parallel.ForEach(sources, s => s.Update(mergedUpdates));
        }

        //private void InitRepository(IFileRepository source, IFileRepository destination)
        //{
        //    IList<(IFileRepository Repo, bool IsSynced)> reposSyncInfo = sources
        //        .Select(s => (Repo: s, IsSynced: syncReference.ItHaveBeenSynchedWith(s)))
        //        .ToList();
        //const int MIN_NUMBER_OF_EMPTY_REPOS_FOR_INIT_REFERENCE = 1;
        //    IList<(IFileRepository Repo, bool IsEmpty)> reposIsEmptyInfo = sources
        //        .Select(s => (Repo: s, IsEmpty: s.IsEmpty))
        //        .ToList();

        //    if (!reposSyncInfo.Any(i => i.IsSynced))
        //    {
        //        if (reposIsEmptyInfo.Count(i => i.IsEmpty) != MIN_NUMBER_OF_EMPTY_REPOS_FOR_INIT_REFERENCE)
        //        {
        //            //throw new 
        //        }
        //    }
        //}
    }
}