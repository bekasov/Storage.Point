namespace StoragePoint.Contracts.Domain.Exceptions
{
    using System.Collections.Generic;

    using StoragePoint.Contracts.Domain.FileStorage.Model;

    public class SeveralChangesForFileInOneStorage : BaseException
    {
        public SeveralChangesForFileInOneStorage(int storageId, FileModel file)
        {
            this.StorageId = storageId;
            this.File = file;
        }

        public int StorageId { get; }

        public FileModel File { get; }
    }
}