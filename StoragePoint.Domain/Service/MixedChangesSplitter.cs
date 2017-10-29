namespace StoragePoint.Domain.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using StoragePoint.Contracts.Domain.Changes.Model;
    using StoragePoint.Contracts.Domain.Changes.Service;
    using StoragePoint.Contracts.Domain.Exceptions;
    using StoragePoint.Contracts.Domain.FileStorage.Model;
    using StoragePoint.Domain.Changes;
    using StoragePoint.Domain.Service.Helper;

    public class MixedChangesSplitter : IMixedChangesSplitter
    {
        // public IUpdatedFilesJoiner FilesJoiner { private get; set; } = new UpdatedFilesJoiner();

        public IReadOnlyList<ChangedFile> Split(MixedChanges mixedChanges)
        {
            //List<FileModel> allAdded = new List<FileModel>();
            //List<FileModel> allRemoved = new List<FileModel>();
            //List<FileModel> allChanged = new List<FileModel>();
            //List<FileModel> allRenamed = new List<FileModel>();
            //List<FileModel> allMoved = new List<FileModel>();

            //Array.ForEach( // Parallel.ForEach failed for allAdded.AddRange: 1 item in result instead of 2 for UpdatesMerger_CorrectArgs_ItMustCallUpdatedFilesJoiner
            //    mixedChanges.ToArray(),
            //    u =>
            //    {
            //        allAdded.AddRange(u.Added);
            //        allRemoved.AddRange(u.Removed);
            //        allChanged.AddRange(u.Updated);
            //        allRenamed.AddRange(u.Renamed);
            //        allMoved.AddRange(u.Moved);
            //    });

            // Parallel.Invoke
            //MixedChanges joinedChanges = new MixedChanges(
            //    this.FilesJoiner.JoinTheSame(allAdded),
            //    this.FilesJoiner.JoinTheSame(allRemoved),
            //    this.FilesJoiner.JoinTheSame(allChanged),
            //    this.FilesJoiner.JoinTheSame(allRenamed),
            //    this.FilesJoiner.JoinTheSame(allMoved));

            List<ChangedFile> result = new List<ChangedFile>();

            result.AddRange(this.SplitMixedAddedAndRemoved(mixedChanges, FileType.FOLDER));
            result.AddRange(this.SplitMixedChanges(mixedChanges, FileType.FOLDER));
            result.AddRange(this.SplitMixedChanges(mixedChanges, FileType.FILE));
            result.AddRange(this.SplitMixedAddedAndRemoved(mixedChanges, FileType.FILE));

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
                        throw new SeveralChangesForFileInOneStorage();
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
                    throw new SeveralChangesForFileInOneStorage();
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