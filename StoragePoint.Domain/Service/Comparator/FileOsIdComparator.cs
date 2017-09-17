namespace StoragePoint.Domain.Service.Comparator
{
    using System.Collections.Generic;

    using StoragePoint.Domain.Model;

    public class FileOsIdComparator : IEqualityComparer<FileModel>
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