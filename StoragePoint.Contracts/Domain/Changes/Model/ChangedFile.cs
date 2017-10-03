namespace StoragePoint.Contracts.Domain.Changes.Model
{
    using StoragePoint.Contracts.Domain.FileStorage.Model;

    public enum FileChange
    {
        ADDED,
        REMOVED,
        UPDATED,
        RENAMED,
        MOVED,

        RENAMED_UPDATED,
        MOVED_UPDATED,
        RENAMED_MOVED,
        RENAMED_MOVED_UPDATED
    }

    public class ChangedFile
    {
        public ChangedFile(FileModel file, FileChange changeKind)
        {
            this.File = file;
            this.ChangeKind = changeKind;
        }

        public FileChange ChangeKind { get; }

        public FileModel File { get; }
    }
}