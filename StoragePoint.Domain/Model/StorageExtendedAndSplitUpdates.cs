namespace StoragePoint.Domain.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using StoragePoint.Contracts.Domain.Exceptions;
    using StoragePoint.Contracts.Domain.FileStorage.Model;

    public class ExtendedUpdatesForFolders
    {
        protected readonly List<FileModel> added = new List<FileModel>();
        protected readonly List<FileModel> removed = new List<FileModel>();
        protected readonly List<FileModel> renamed = new List<FileModel>();
        protected readonly List<FileModel> moved = new List<FileModel>();
        protected readonly List<FileModel> renamedAndMoved = new List<FileModel>();

        protected readonly IList<UpdateKind> UPDATED = new List<UpdateKind> { UpdateKind.UPDATED };
        protected readonly IList<UpdateKind> RENAMED = new List<UpdateKind> { UpdateKind.RENAMED };
        protected readonly IList<UpdateKind> MOVED = new List<UpdateKind> { UpdateKind.MOVED };
        protected readonly IList<UpdateKind> RENAMED_UPDATED =
            new List<UpdateKind> { UpdateKind.RENAMED, UpdateKind.UPDATED };

        protected readonly IList<UpdateKind> MOVED_UPDATED =
            new List<UpdateKind> { UpdateKind.MOVED, UpdateKind.UPDATED };

        protected readonly IList<UpdateKind> RENAMED_MOVED = 
            new List<UpdateKind> { UpdateKind.RENAMED, UpdateKind.MOVED };
        
        protected readonly IList<UpdateKind> RENAMED_MOVED_UPDATED = 
            new List<UpdateKind> { UpdateKind.RENAMED, UpdateKind.MOVED, UpdateKind.UPDATED };

        public ExtendedUpdatesForFolders(StorageUpdates updates)
        {
            this.StorageId = updates.StorageId;

            if (updates.Updated.Any(f => f.FileType == FileType.FOLDER))
            {
                throw new WrongTypeOfUpdates();
            }

            this.ProcessAddedAndRemoved(updates, FileType.FOLDER);
            this.ProcessRenamedAndMovedForFolders(updates);
        }

        public enum UpdateKind
        {
            ADDED,
            REMOVED,
            UPDATED,
            RENAMED,
            MOVED
        }

        public int StorageId { get; }

        public IReadOnlyList<FileModel> Added => this.added;

        public IReadOnlyList<FileModel> Removed => this.removed;

        public IReadOnlyList<FileModel> Renamed => this.renamed;

        public IReadOnlyList<FileModel> Moved => this.moved;

        public IReadOnlyList<FileModel> RenamedAndMoved => this.renamedAndMoved;

        protected void ProcessAddedAndRemoved(StorageUpdates updates, FileType fileType)
        {
            foreach (FileModel addedFile in updates.Added.Where(f => f.FileType == fileType))
            {
                IList<UpdateKind> changes = this.SearchChangesForFile(addedFile, updates, fileType);
                this.EnsureNoOtherChanges(UpdateKind.ADDED, changes, addedFile);
                this.added.Add(addedFile);
            }

            foreach (FileModel removedFile in updates.Removed.Where(f => f.FileType == fileType))
            {
                IList<UpdateKind> changes = this.SearchChangesForFile(removedFile, updates, fileType);
                this.EnsureNoOtherChanges(UpdateKind.REMOVED, changes, removedFile);
                this.removed.Add(removedFile);
            }
        }

        protected IList<UpdateKind> SearchChangesForFile(FileModel file, StorageUpdates updates, FileType fileType)
        {
            IList<UpdateKind> result = new List<UpdateKind>();

            void FindFile(UpdateKind updateKind, IReadOnlyList<FileModel> changeSet)
            {
                List<FileModel> foundFiles = changeSet.Where(f => f.FileOsId == file.FileOsId && f.FileType == fileType)
                    .ToList();

                if (foundFiles.Any())
                {
                    if (foundFiles.Count > 1)
                    {
                        throw new SeveralChangesForFileInOneStorage(this.StorageId, foundFiles.First());
                    }

                    result.Add(updateKind);
                }
            }

            FindFile(UpdateKind.ADDED, updates.Added);
            FindFile(UpdateKind.REMOVED, updates.Removed);
            FindFile(UpdateKind.UPDATED, updates.Updated);
            FindFile(UpdateKind.RENAMED, updates.Renamed);
            FindFile(UpdateKind.MOVED, updates.Moved);

            return result;
        }

        protected IDictionary<IList<UpdateKind>, IList<FileModel>> SplitAndExtendUpdates(
            StorageUpdates updates,
            FileType fileType)
        {
            IDictionary<UpdateKind, IReadOnlyList<FileModel>> allKinds 
                = new Dictionary<UpdateKind, IReadOnlyList<FileModel>>
            {
                { UpdateKind.MOVED, updates.Moved },
                { UpdateKind.UPDATED, updates.Updated },
                { UpdateKind.RENAMED, updates.Renamed }
            };

            IList<IList<UpdateKind>> allCombinations = new List<IList<UpdateKind>>
            {
                this.UPDATED,
                this.RENAMED,
                this.MOVED,
                this.RENAMED_UPDATED,
                this.MOVED_UPDATED,
                this.RENAMED_MOVED,
                this.RENAMED_MOVED_UPDATED
            };

            IDictionary<IList<UpdateKind>, IList<FileModel>> result 
                = new Dictionary<IList<UpdateKind>, IList<FileModel>>();

            foreach (IList<UpdateKind> currentMixedKind in allCombinations)
            {
                result.Add(currentMixedKind, new List<FileModel>());
            }

            foreach (IReadOnlyList<FileModel> currentChangeSet in allKinds.Values)
            {
                foreach (FileModel currentFile in currentChangeSet.Where(f => f.FileType == fileType))
                {
                    IList<UpdateKind> changes = this.SearchChangesForFile(currentFile, updates, fileType);

                    foreach (IList<UpdateKind> currentMixedKind in allCombinations)
                    {
                        IList<FileModel> resultSubset = result[currentMixedKind];

                        IList<UpdateKind> negativeKinds = allKinds.Keys.Except(currentMixedKind).ToList();

                        if (currentMixedKind.All(k => changes.Contains(k)) 
                            && !changes.Any(negativeKinds.Contains)
                            && resultSubset.All(f => f.FileOsId != currentFile.FileOsId))
                        {
                            resultSubset.Add(currentFile);
                        }
                    }
                }
            }

            return result;

            //IList<UpdateKind> rmcPositiveKinds = new List<UpdateKind>(allKinds);

            //IList<UpdateKind> rcPositiveKinds = new List<UpdateKind> { UpdateKind.RENAMED, UpdateKind.UPDATED };
            //IList<UpdateKind> rcNegativeKinds = allKinds.Except(rcPositiveKinds).ToList();

            //IList<UpdateKind> mcPositiveKinds = new List<UpdateKind> { UpdateKind.MOVED, UpdateKind.UPDATED };
            //IList<UpdateKind> mcNegativeKinds = allKinds.Except(mcPositiveKinds).ToList();

            //IList<UpdateKind> rmPositiveKinds = new List<UpdateKind> { UpdateKind.RENAMED, UpdateKind.MOVED };
            //IList<UpdateKind> rmNegativeKinds = allKinds.Except(rmPositiveKinds).ToList();

            //IList<UpdateKind> resultPositiveKinds = new List<UpdateKind> { resultKind };
            //IList<UpdateKind> resultNegativeKinds = allKinds.Except(rmPositiveKinds).ToList();

            //            foreach (FileModel currentFile in changeSet.Where(f => f.FileType == fileType))
            //            {
            //                IList<UpdateKind> changes = this.SearchChangesForFile(currentFile, updates, fileType);
            //
            //                if (rmcPositiveKinds.All(k => changes.Contains(k)) 
            //                    && this.RenamedAndMovedAndUpdated.All(f => f.FileOsId != currentFile.FileOsId))
            //                {
            //                    this.renamedAndMovedAndUpdated.Add(currentFile);
            //                }
            //
            //                if (mcPositiveKinds.All(changes.Contains) && !changes.Any(mcNegativeKinds.Contains) 
            //                    && this.movedAndUpdated.All(f => f.FileOsId != currentFile.FileOsId))
            //                {
            //                    this.movedAndUpdated.Add(currentFile);
            //                }
            //
            //                if (rcPositiveKinds.All(changes.Contains) && !changes.Any(rcNegativeKinds.Contains) 
            //                    && this.renamedAndUpdated.All(f => f.FileOsId != currentFile.FileOsId))
            //                {
            //                    this.renamedAndUpdated.Add(currentFile);
            //                }
            //
            //                if (rmPositiveKinds.All(changes.Contains) && !changes.Any(rmNegativeKinds.Contains) 
            //                    && this.renamedAndMoved.All(f => f.FileOsId != currentFile.FileOsId))
            //                {
            //                    this.renamedAndMoved.Add(currentFile);
            //                }
            //
            //                if (resultPositiveKinds.All(changes.Contains) && !changes.Any(resultNegativeKinds.Contains)
            //                    && resultAcc.All(f => f.FileOsId != currentFile.FileOsId))
            //                {
            //                    resultAcc.Add(currentFile);
            //                }
            //            }
        }

        private void EnsureNoOtherChanges(UpdateKind filteredArea, IList<UpdateKind> changes, FileModel currentFile)
        {
            if (!changes.Contains(filteredArea))
            {
                throw new ArgumentOutOfRangeException();
            }

            changes.Remove(filteredArea);
            foreach (UpdateKind changeKey in Enum.GetValues(typeof(UpdateKind)))
            {
                if (changes.Contains(changeKey))
                {
                    throw new SeveralChangesForFileInOneStorage(this.StorageId, currentFile);
                }
            }
        }

        private void ProcessRenamedAndMovedForFolders(StorageUpdates updates)
        {
            IDictionary<IList<UpdateKind>, IList<FileModel>> allChanges = this.SplitAndExtendUpdates(
                updates,
                FileType.FOLDER);

            this.renamed.AddRange(allChanges[this.RENAMED]);
            this.moved.AddRange(allChanges[this.MOVED]);
            this.renamedAndMoved.AddRange(allChanges[this.RENAMED_MOVED]);
        }
    }

    public class ExtendedUpdates : ExtendedUpdatesForFolders
    {
        protected readonly List<FileModel> updated = new List<FileModel>();
        protected readonly List<FileModel> renamedAndUpdated = new List<FileModel>();
        protected readonly List<FileModel> movedAndUpdated = new List<FileModel>();
        protected readonly List<FileModel> renamedAndMovedAndUpdated = new List<FileModel>();

        public ExtendedUpdates(StorageUpdates updates)
            : base(updates)
        {
            this.ProcessAddedAndRemoved(updates, FileType.FILE);
            this.ProcessRenamedAndMovedAndUpdatedForFiles(updates);
        }

        public IReadOnlyList<FileModel> Updated => this.updated;

        public IReadOnlyList<FileModel> RenamedAndUpdated => this.renamedAndUpdated;

        public IReadOnlyList<FileModel> MovedAndUpdated => this.movedAndUpdated;

        public IReadOnlyList<FileModel> RenamedAndMovedAndUpdated => this.renamedAndMovedAndUpdated;

        private void ProcessRenamedAndMovedAndUpdatedForFiles(StorageUpdates updates)
        {
            IDictionary<IList<UpdateKind>, IList<FileModel>> allChanges = this.SplitAndExtendUpdates(
                updates,
                FileType.FILE);

            this.updated.AddRange(allChanges[this.UPDATED]);
            this.renamed.AddRange(allChanges[this.RENAMED]);
            this.moved.AddRange(allChanges[this.MOVED]);

            this.renamedAndUpdated.AddRange(allChanges[this.RENAMED_UPDATED]);
            this.movedAndUpdated.AddRange(allChanges[this.MOVED_UPDATED]);
            this.renamedAndMoved.AddRange(allChanges[this.RENAMED_MOVED]);
            this.renamedAndMovedAndUpdated.AddRange(allChanges[this.RENAMED_MOVED_UPDATED]);
        }
    }
}