namespace StoragePoint.Domain.Model
{
    using System;

    public enum FileType
    {
        FILE = 0,
        FOLDER
    }

    public class FileModel
    {
        public const int ROOT_FOLDER_OS_ID = 0;
        public const int ROOT_FOLDER_PARENT_OS_ID = 0;
        
        public int FileOsId { get; set; }

        public FileType FileType { get; set; }

        public int ParentFileOsId { get; set; }

        public string Name { get; set; }

        public DateTime UpdateTime { get; set; }

        // public bool IsRootFolder => this.FileType == FileType.FOLDER && this.ParentFileOsId == ROOT_FOLDER_PARENT_OS_ID;
    }
}