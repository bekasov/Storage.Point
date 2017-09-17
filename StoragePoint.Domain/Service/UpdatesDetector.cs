namespace StoragePoint.Domain.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using StoragePoint.Domain.Model;
    using StoragePoint.Domain.Service.Comparator;

    public class UpdatesDetector
    {
        public RepositoryUpdates Detect(StorageContent referenceContent, StorageContent source)
        {
            if (referenceContent == null)
            {
                throw new ArgumentNullException(nameof(referenceContent));
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            // this.EnsureContentHasRootAndOnlyOneRootFolder(referenceContent, nameof(referenceContent));
            // this.EnsureContentHasRootAndOnlyOneRootFolder(source, nameof(source));

            List<FileModel> addedFiles = this.GetSource2ExceptSource1(
                referenceContent.Files, 
                source.Files, 
                new FileOsIdComparator());
            int[] addedFilesIds = addedFiles.Select(f => f.FileOsId).ToArray();
            List<FileModel> removedFiles = this.GetSource2ExceptSource1(
                source.Files, 
                referenceContent.Files, 
                new FileOsIdComparator());
            int[] removedFilesIds = removedFiles.Select(f => f.FileOsId).ToArray();
            List<FileModel> renamedFiles = this.GetSource2ExceptSource1(
                    referenceContent.Files,
                    source.Files,
                    new FileOsIdsAndNamesComparator())
                .Where(f => !addedFilesIds.Contains(f.FileOsId))
                .ToList();

            RepositoryUpdates result = new RepositoryUpdates
            {
                Added = addedFiles,
                Removed = removedFiles,
                Moved = this.DetectMovedFiles(referenceContent, source, removedFilesIds, addedFilesIds),
                Renamed = renamedFiles
            };

            return result;
        }

//        private void EnsureContentHasRootAndOnlyOneRootFolder(StorageContent content, string argumentName)
//        {
//            if (content.Files == null || content.Files.Count(f => f.IsRootFolder) != 1)
//            {
//                throw new ArgumentException(argumentName);
//            }
//        }

        private List<FileModel> GetSource2ExceptSource1(
            IReadOnlyList<FileModel> source1,
            IReadOnlyList<FileModel> source2,
            IEqualityComparer<FileModel> comparator)
        {
            List<FileModel> result = source2
                .Except(source1, comparator)
                .ToList();

            return result;
        }

        private List<FileModel> DetectMovedFiles(
            StorageContent referenceContent, 
            StorageContent source,
            int[] removedFilesIds,
            int[] newFilesIds)
        {
            List<FileModel> result = new List<FileModel>();

            void FindChildren(List<FileModel> parents)
            {
                foreach (FileModel parent in parents)
                {
                    List<FileModel> children = source.Files.Where(f => f.ParentFileOsId == parent.FileOsId).ToList();
                    if (children.Any())
                    {
                        result.AddRange(children);
                        FindChildren(children);
                    }
                }
            }

            List<FileModel> rootMovedFiles = this.GetSource2ExceptSource1(
                    source.Files,
                    referenceContent.Files,
                    new FileOsIdsAndParentsIdsComparator())
                .Where(f => !removedFilesIds.Contains(f.FileOsId))
                .ToList();
            
            result.AddRange(rootMovedFiles);
            FindChildren(rootMovedFiles);

            result = result
                .Distinct(new FileOsIdComparator())
                .Where(f => !newFilesIds.Contains(f.FileOsId))
                .ToList();
            return result;
        }
    }
}