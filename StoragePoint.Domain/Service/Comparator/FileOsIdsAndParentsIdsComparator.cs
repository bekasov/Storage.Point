namespace StoragePoint.Domain.Service.Comparator
{
    using System.Collections.Generic;

    using StoragePoint.Domain.Model;

    public class FileOsIdsAndParentsIdsComparator : IEqualityComparer<FileModel>
    {
        public bool Equals(FileModel x, FileModel y)
        {
            return x.FileOsId == y.FileOsId && x.ParentFileOsId == y.ParentFileOsId;
        }

        public int GetHashCode(FileModel obj)
        {
            int n1 = 99999997;
            int idHash = ((double)obj.FileOsId).GetHashCode() % n1;
            int parentIdHash = ((double)obj.ParentFileOsId).GetHashCode();
            return idHash ^ parentIdHash;

            //return obj.FileOsId.GetHashCode() ^ obj.ParentFileOsId.GetHashCode().RotateLeft(16);
        }
    }
}