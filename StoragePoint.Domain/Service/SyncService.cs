namespace StoragePoint.Domain.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using StoragePoint.Contracts.Domain.Exceptions;
    using StoragePoint.Contracts.Domain.FileStorage;
    using StoragePoint.Contracts.Domain.FileStorage.Model;
    using StoragePoint.Contracts.Domain.Service;

    public class SyncService
    {
        private readonly IUpdatesMerger updatesMerger;

        public SyncService(IUpdatesMerger updatesMerger)
        {
            this.updatesMerger = updatesMerger;
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
            
            IList<(IFileRepository Repo, StorageUpdates Updates)> reposUpdates = sources
                .Select(repository => (Repo: repository, Updates: syncReference.DetectUpdates(repository)))
                .AsParallel()
                .ToList();

            StorageUpdates mergedUpdates = this.updatesMerger.Merge(reposUpdates.Select(u => u.Updates).ToList());

            Parallel.ForEach(sources, s => s.Update(mergedUpdates));

            syncReference.Update(mergedUpdates);
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