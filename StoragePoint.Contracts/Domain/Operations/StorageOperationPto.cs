namespace StoragePoint.Contracts.Domain.Operations
{
    using StoragePoint.Contracts.Domain.FileStorage.Pto;

    public enum StorageOperation
    {
        COPY_FILE,
        REMOVE_FILE,
        RENAME_FILE,
        MOVE_FILE,

        CREATE_DIRECTORY,
        REMOVE_EMPTY_DIRECTORY,
        RENAME_DIRECTORY
    }

    public class StorageOperationPto
    {
        public StorageOperationPto(
            StorageOperation storageOperation, 
            FullPathFilePto sourceFile, 
            FullPathFilePto destinationFile)
        {
            this.StorageOperation = storageOperation;
            this.SourceFile = sourceFile;
            this.DestinationFile = destinationFile;
        }

        public StorageOperation StorageOperation { get; }

        public FullPathFilePto SourceFile { get; }

        public FullPathFilePto DestinationFile { get; }
    }
}