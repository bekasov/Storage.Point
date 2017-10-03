namespace StoragePoint.Domain.Changes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using StoragePoint.Contracts.Domain.Changes.Model;
    using StoragePoint.Contracts.Domain.Changes.Service;
    using StoragePoint.Contracts.Domain.Exceptions;
    using StoragePoint.Contracts.Domain.FileStorage.Model;

    public class MixedChangesProcessor : IMixedChangesProcessor
    {
        private int storageId;

        public IList<ChangedFile> SplitChanges(MixedChanges changes)
        {
            if (changes.Updated.Any(f => f.FileType == FileType.FOLDER))
            {
                throw new WrongDetectedChangeKind();
            }

            this.storageId = changes.StorageId;

            List<ChangedFile> result = new List<ChangedFile>();

            result.AddRange(this.SplitMixedAddedAndRemoved(changes, FileType.FOLDER));
            result.AddRange(this.SplitMixedChanges(changes, FileType.FOLDER));
            result.AddRange(this.SplitMixedChanges(changes, FileType.FILE));
            result.AddRange(this.SplitMixedAddedAndRemoved(changes, FileType.FILE));
            
            return result;
        }

        private IList<ChangedFile> SplitMixedAddedAndRemoved(MixedChanges changes, FileType fileType)
        {
            IList<ChangedFile> result = new List<ChangedFile>();

            foreach (FileModel addedFile in changes.Added.Where(f => f.FileType == fileType))
            {
                IList<DetectedChange> allChanges = this.GetAllChanges(addedFile, changes, fileType);
                this.EnsureNoOtherChanges(DetectedChange.ADDED, allChanges, addedFile);
                result.Add(new ChangedFile(addedFile, FileChange.ADDED));
            }

            foreach (FileModel removedFile in changes.Removed.Where(f => f.FileType == fileType))
            {
                IList<DetectedChange> allChanges = this.GetAllChanges(removedFile, changes, fileType);
                this.EnsureNoOtherChanges(DetectedChange.REMOVED, allChanges, removedFile);
                result.Add(new ChangedFile(removedFile, FileChange.REMOVED));
            }

            return result;
        }

        private IList<DetectedChange> GetAllChanges(FileModel file, MixedChanges changes, FileType fileType)
        {
            IList<DetectedChange> result = new List<DetectedChange>();

            void FindFile(DetectedChange updateKind, IReadOnlyList<FileModel> changeSet)
            {
                List<FileModel> foundFiles = changeSet.Where(f => f.FileOsId == file.FileOsId && f.FileType == fileType)
                    .ToList();

                if (foundFiles.Any())
                {
                    if (foundFiles.Count > 1)
                    {
                        throw new SeveralChangesForFileInOneStorage(this.storageId, foundFiles.First());
                    }

                    result.Add(updateKind);
                }
            }

            FindFile(DetectedChange.ADDED, changes.Added);
            FindFile(DetectedChange.REMOVED, changes.Removed);
            FindFile(DetectedChange.UPDATED, changes.Updated);
            FindFile(DetectedChange.RENAMED, changes.Renamed);
            FindFile(DetectedChange.MOVED, changes.Moved);

            return result;
        }

        private void EnsureNoOtherChanges(DetectedChange filteredArea, IList<DetectedChange> changes, FileModel currentFile)
        {
            if (!changes.Contains(filteredArea))
            {
                throw new ArgumentOutOfRangeException();
            }

            changes.Remove(filteredArea);
            foreach (DetectedChange changeKey in Enum.GetValues(typeof(DetectedChange)))
            {
                if (changes.Contains(changeKey))
                {
                    throw new SeveralChangesForFileInOneStorage(this.storageId, currentFile);
                }
            }
        }

        private IList<ChangedFile> SplitMixedChanges(MixedChanges changes, FileType fileType)
        {
            IDictionary<DetectedChange, IReadOnlyList<FileModel>> mixedChanges = new Dictionary<DetectedChange, IReadOnlyList<FileModel>>
            {
                { DetectedChange.MOVED, changes.Moved },
                { DetectedChange.UPDATED, changes.Updated },
                { DetectedChange.RENAMED, changes.Renamed }
            };

            IList<ChangedFile> result = new List<ChangedFile>();
            
            foreach (IReadOnlyList<FileModel> currentChangeSet in mixedChanges.Values)
            {
                foreach (FileModel currentFile in currentChangeSet.Where(f => f.FileType == fileType))
                {
                    IList<DetectedChange> allChanges = this.GetAllChanges(currentFile, changes, fileType);

                    foreach (IList<DetectedChange> currentMixedKind in ChangesConstants.MIXED_CHANGES_MAP.Keys)
                    {
                        IList<DetectedChange> negativeKinds = mixedChanges.Keys.Except(currentMixedKind).ToList();
                        if (currentMixedKind.All(k => allChanges.Contains(k))
                            && !allChanges.Any(negativeKinds.Contains)
                            && result.All(f => f.File.FileOsId != currentFile.FileOsId))
                        {
                            result.Add(new ChangedFile(currentFile, ChangesConstants.MIXED_CHANGES_MAP[currentMixedKind]));
                        }
                    }
                }
            }

            return result;
        }
    }
}