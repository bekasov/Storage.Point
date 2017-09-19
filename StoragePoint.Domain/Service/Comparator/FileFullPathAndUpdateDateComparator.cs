namespace StoragePoint.Domain.Service.Comparator
{
    using System;
    using System.Collections.Generic;

    using StoragePoint.Contracts.Domain.FileStorage.Pto;

    public class FileFullPathAndUpdateDateComparator : IEqualityComparer<FullPathFilePto>
    {
        public bool Equals(FullPathFilePto x, FullPathFilePto y)
        {
            return string.Equals(x.FullPath, y.FullPath, StringComparison.CurrentCulture)
                   && DateTime.Equals(x.File.UpdateTime, y.File.UpdateTime);
        }

        public int GetHashCode(FullPathFilePto obj)
        {
            return obj.FullPath.GetHashCode() + obj.File.UpdateTime.GetHashCode();
        }
    }
}