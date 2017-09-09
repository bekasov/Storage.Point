namespace StoragePoint.Domain.Model
{
    using System;

    public class FileModel
    {
        public FileModel(int fileOsId, string name, DateTime updateTime)
        {
            this.FileOsId = fileOsId;
            this.Name = name;
            this.UpdateTime = updateTime;
        }

        public int FileOsId { get; private set; }

        public string Name { get; private set; }

        public DateTime UpdateTime { get; private set; }
    }
}