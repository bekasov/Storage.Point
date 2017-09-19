namespace StoragePoint.Domain.Service.Comparator
{
    using System;
    using System.Collections.Generic;

    using StoragePoint.Contracts.Domain.FileStorage.Model;

    public class FileOsIdsAndNamesComparator : IEqualityComparer<FileModel>
    {
        public bool Equals(FileModel x, FileModel y)
        {
            return x.FileOsId == y.FileOsId && string.Equals(x.Name, y.Name, StringComparison.CurrentCulture);
        }

        public int GetHashCode(FileModel obj)
        {
            return obj.FileOsId.GetHashCode() + (obj.Name?.GetHashCode(StringComparison.CurrentCulture) ?? 0);
        }
    }
}