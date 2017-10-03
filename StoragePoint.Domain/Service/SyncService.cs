namespace StoragePoint.Domain.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using StoragePoint.Contracts.Domain.Changes.Model;
    using StoragePoint.Contracts.Domain.Changes.Service;
    using StoragePoint.Contracts.Domain.Exceptions;
    using StoragePoint.Contracts.Domain.FileStorage;

    public class SyncService
    {
        private readonly IMixedChangesMerger mixedChangesMerger;

        public SyncService(IMixedChangesMerger mixedChangesMerger)
        {
            this.mixedChangesMerger = mixedChangesMerger;
        }

        public void InitReference(IFileReferenceRepository syncReference, IFileRepository source)
        {
            if (syncReference == null)
            {
                throw new ArgumentNullException(nameof(syncReference));
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (syncReference.IsInitialized)
            {
                return;
            }

            syncReference.CopyAll(source);
        }

        public void InitRepositories(IReadOnlyList<IFileRepository> sources, IFileReferenceRepository syncReference)
        {
            this.EnsureSourcesAndReferenceIsNotNull(sources, syncReference);
            this.EnsureRepositoryIsInitialized(syncReference);

            IList<IFileRepository> uninitRepos = sources
                .Where(s => s.IsInitialized == false)
                .ToList();

            Parallel.ForEach(uninitRepos, r => r.CopyAll(syncReference));
        }

        public void Sync(IReadOnlyList<IFileRepository> sources, IFileReferenceRepository syncReference)
        {
            this.EnsureSourcesAndReferenceIsNotNull(sources, syncReference);
            this.EnsureRepositoryIsInitialized(syncReference);

            if (sources.Any(repository => repository.IsInitialized))
            {
                throw new AllRepositoriesMustBeInitialized();
            }
            
            IList<(IFileRepository Repo, MixedChanges Updates)> reposUpdates = sources
                .Select(repository => (Repo: repository, Updates: syncReference.DetectUpdates(repository)))
                .AsParallel()
                .ToList();

            MixedChanges mergedChanges = this.mixedChangesMerger.Merge(reposUpdates.Select(u => u.Updates).ToList());

            Parallel.ForEach(sources, s => s.Update(mergedChanges));

            syncReference.Update(mergedChanges);
        }

        private void EnsureSourcesAndReferenceIsNotNull(
                IReadOnlyList<IFileRepository> sources, 
                IFileReferenceRepository syncReference)
        {
            if (sources == null)
            {
                throw new ArgumentNullException(nameof(sources));
            }

            if (syncReference == null)
            {
                throw new ArgumentNullException(nameof(syncReference));
            }
        }

        private void EnsureRepositoryIsInitialized(IFileRepository source)
        {
            if (!source.IsInitialized)
            {
                throw new RepositoryNotInitialized();
            }
        }
    }
}