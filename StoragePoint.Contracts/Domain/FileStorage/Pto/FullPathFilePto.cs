namespace StoragePoint.Contracts.Domain.FileStorage.Pto
{
    using StoragePoint.Contracts.Domain.FileStorage.Model;

    public class FullPathFilePto
    {
        public FullPathFilePto(int sourceStorageId, FileModel file, string fullPath)
        {
            this.SourceStorageId = sourceStorageId;
            this.File = file;
            this.FullPath = fullPath;
        }

        public int SourceStorageId { get; }

        public string FullPath { get; }

        public FileModel File { get; }
    }
}