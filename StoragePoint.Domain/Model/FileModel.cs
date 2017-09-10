namespace StoragePoint.Domain.Model
{
    using System;

    public enum FileType
    {
        FILE,
        FOLDER
    }

    public class FileModel
    {
        public int FileOsId { get; set; }

        public FileType FileType { get; set; }

        public int ParentFileOsId { get; set; }

        public string Name { get; set; }

        public string ParentPath { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}