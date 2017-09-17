namespace StoragePoint.Domain.Service.Comparator
{
    using System.Collections.Generic;

    using StoragePoint.Domain.Model;

    public class FileOsIdsAndUpdateDateComparator : IEqualityComparer<FileModel>
    {
        public bool Equals(FileModel x, FileModel y)
        {
            return x.FileOsId == y.FileOsId && x.UpdateTime == y.UpdateTime;
        }

        public int GetHashCode(FileModel obj)
        {
            return obj.FileOsId.GetHashCode() + obj.UpdateTime.GetHashCode();
        }
    }
}