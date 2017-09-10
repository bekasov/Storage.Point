namespace StoragePoint.Domain.Service
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using StoragePoint.Domain.Model;
    using StoragePoint.Domain.Repository;

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

            this.EnsureContentHasRootAndOnlyOneRootFolder(referenceContent, nameof(referenceContent));
            this.EnsureContentHasRootAndOnlyOneRootFolder(source, nameof(source));

            List<FileModel> addedFiles = this.DetectNewFiles(referenceContent, source);

            RepositoryUpdates result = new RepositoryUpdates
            {
                Added = addedFiles
            };

            return result;
        }

        private void EnsureContentHasRootAndOnlyOneRootFolder(StorageContent content, string argumentName)
        {
            if (content.Files == null || content.Files.Count(f => f.IsRootFolder) != 1)
            {
                throw new ArgumentException(argumentName);
            }
        }

        private List<FileModel> DetectNewFiles(StorageContent referenceContent, StorageContent source)
        {
            List<FileModel> result = source.Files.Except(referenceContent.Files, new FileByOsIdComparator()).ToList();

            return result;
        }
    }

    public class FileByOsIdComparator : IEqualityComparer<FileModel>
    {
        public bool Equals(FileModel x, FileModel y)
        {
            return x.FileOsId == y.FileOsId;
        }

        public int GetHashCode(FileModel obj)
        {
            return obj.FileOsId.GetHashCode();
        }
    }
}