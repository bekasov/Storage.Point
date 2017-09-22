﻿using StoragePoint.Domain.Service.Helper;

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
        //private readonly IPathBuilder pathBuilder;

        public UpdatesMerger()
        {
            //this.pathBuilder = pathBuilder;
        }

        public IUpdatedFilesJoiner FilesJoiner { private get; set; } = new TheSameFilesJoiner();

        public StorageUpdates Merge(IReadOnlyList<StorageUpdates> updates)
        {
            List<FileModel> allAdded = new List<FileModel>();
            List<FileModel> allRemoved = new List<FileModel>();
            List<FileModel> allChanged = new List<FileModel>();
            List<FileModel> allRenamed = new List<FileModel>();
            List<FileModel> allMoved = new List<FileModel>();

            Parallel.ForEach(
                updates,
                u =>
                {
                    allAdded.AddRange(u.Added);
                    allRemoved.AddRange(u.Removed);
                    allChanged.AddRange(u.Updated);
                    allRenamed.AddRange(u.Renamed);
                    allMoved.AddRange(u.Moved);
                });

            IList<FileModel> joinedAdded = this.FilesJoiner.JoinTheSame(allAdded);
            IList<FileModel> joinedRemoved;
            IList<FileModel> joinedChanged;
            IList<FileModel> joinedRenamed;
            IList<FileModel> joinedMoved;

//            Parallel.Invoke(
//                () => joinedAdded = this.FilesJoiner.JoinTheSame(allAdded), 
//                () => joinedRemoved = this.FilesJoiner.JoinTheSame(allRemoved), 
//                () => joinedChanged = this.FilesJoiner.JoinTheSame(allChanged), 
//                () => joinedRenamed = this.FilesJoiner.JoinTheSame(allRenamed), 
//                () => joinedMoved = this.FilesJoiner.JoinTheSame(allMoved));

            
            
            
            
            

//            Parallel.ForEach(
//                updates, 
//                u => 
//                {
//                    allAdded.AddRange(this.pathBuilder.GetPaths(u.Added));
//                    allRemoved.AddRange(this.pathBuilder.GetPaths(u.Removed));
//                    allChanged.AddRange(this.pathBuilder.GetPaths(u.Updated));
//                    allRenamed.AddRange(this.pathBuilder.GetPaths(u.Renamed));
//                    allMoved.AddRange(this.pathBuilder.GetPaths(u.Moved));
//                });

            return null;
        }
    }
}