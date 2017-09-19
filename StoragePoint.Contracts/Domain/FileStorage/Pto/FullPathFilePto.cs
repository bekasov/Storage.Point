namespace StoragePoint.Contracts.Domain.FileStorage.Pto
{
    using StoragePoint.Contracts.Domain.FileStorage.Model;

    public class FullPathFilePto
    {
        public FullPathFilePto(FileModel file, string fullPath)
        {
            this.File = file;
            this.FullPath = fullPath;
        }

        public string FullPath { get; }

        public FileModel File { get; }
    }
}