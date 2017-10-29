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
        private readonly IMixedChangesSplitter mixedChangesSplitter;

        public SyncService(IMixedChangesSplitter mixedChangesSplitter)
        {
            this.mixedChangesSplitter = mixedChangesSplitter;
        }

        public void InitReference(IFileReferenceStorage syncReference, IFileStorage source)
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

        public void InitRepositories(IReadOnlyList<IFileStorage> sources, IFileReferenceStorage syncReference)
        {
            this.EnsureSourcesAndReferenceIsNotNull(sources, syncReference);
            this.EnsureRepositoryIsInitialized(syncReference);

            IList<IFileStorage> noInitRepositories = sources
                .Where(s => s.IsInitialized == false)
                .ToList();

            Parallel.ForEach(noInitRepositories, r => r.CopyAll(syncReference));
        }

        public void Sync(IReadOnlyList<IFileStorage> sources, IFileReferenceStorage syncReference)
        {
            this.EnsureSourcesAndReferenceIsNotNull(sources, syncReference);
            this.EnsureRepositoryIsInitialized(syncReference);

            if (sources.Any(repository => repository.IsInitialized))
            {
                throw new AllRepositoriesMustBeInitialized();
            }
            
            IList<(IFileStorage Storage, MixedChanges MixedChanges)> mixedChanges = sources
                .Select(repository => (Storage: repository, MixedChanges: syncReference.DetectUpdates(repository)))
                .AsParallel()
                .ToList();

            // (Storage: c.Storage, SplitChanges: this.mixedChangesSplitter.Split(c.MixedChanges))
            IList<StorageChanges> splitChanges = mixedChanges
                .Select(c => new StorageChanges(c.Storage, this.mixedChangesSplitter.Split(c.MixedChanges)))
                .AsParallel()
                .ToList();

            foreach (StorageChanges splitChange in splitChanges)
            {
                foreach (StorageChanges otherSplitChange in splitChanges)
                {
                    if (ReferenceEquals(splitChange, otherSplitChange))
                    {
                        continue;
                    }


                }
            }


            // Parallel.ForEach(sources, s => s.Update(mergedChanges));

            // syncReference.Update(mergedChanges);
        }

        class StorageChanges
        {
            public StorageChanges(IFileStorage storage, IReadOnlyList<ChangedFile> splitChanges)
            {
                this.Storage = storage;
                this.SplitChanges = splitChanges;
            }

            public IFileStorage Storage { get; }

            public IReadOnlyList<ChangedFile> SplitChanges { get; }
        }

        private void EnsureSourcesAndReferenceIsNotNull(IReadOnlyList<IFileStorage> sources, IFileReferenceStorage syncReference)
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

        private void EnsureRepositoryIsInitialized(IFileStorage source)
        {
            if (!source.IsInitialized)
            {
                throw new RepositoryNotInitialized();
            }
        }
    }
}