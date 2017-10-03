namespace StoragePoint.Domain.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using StoragePoint.Contracts.Domain.Changes.Model;
    using StoragePoint.Contracts.Domain.Changes.Service;
    using StoragePoint.Contracts.Domain.FileStorage.Model;
    using StoragePoint.Domain.Service.Helper;

    public class MixedChangesMerger : IMixedChangesMerger
    {
        public IUpdatedFilesJoiner FilesJoiner { private get; set; } = new UpdatedFilesJoiner();

        public MixedChanges Merge(IReadOnlyList<MixedChanges> mixedChanges)
        {
            List<FileModel> allAdded = new List<FileModel>();
            List<FileModel> allRemoved = new List<FileModel>();
            List<FileModel> allChanged = new List<FileModel>();
            List<FileModel> allRenamed = new List<FileModel>();
            List<FileModel> allMoved = new List<FileModel>();

            Array.ForEach( // Parallel.ForEach failed for allAdded.AddRange: 1 item in result instead of 2 for UpdatesMerger_CorrectArgs_ItMustCallUpdatedFilesJoiner
                mixedChanges.ToArray(),
                u =>
                {
                    allAdded.AddRange(u.Added);
                    allRemoved.AddRange(u.Removed);
                    allChanged.AddRange(u.Updated);
                    allRenamed.AddRange(u.Renamed);
                    allMoved.AddRange(u.Moved);
                });

            // Parallel.Invoke
            IList<FileModel> joinedAdded = this.FilesJoiner.JoinTheSame(allAdded);
            IList<FileModel> joinedRemoved = this.FilesJoiner.JoinTheSame(allRemoved);
            IList<FileModel> joinedChanged = this.FilesJoiner.JoinTheSame(allChanged);
            IList<FileModel> joinedRenamed = this.FilesJoiner.JoinTheSame(allRenamed);
            IList<FileModel> joinedMoved = this.FilesJoiner.JoinTheSame(allMoved);

            return null;
        }
    }
}