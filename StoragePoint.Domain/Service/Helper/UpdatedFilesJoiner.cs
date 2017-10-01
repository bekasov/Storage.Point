namespace StoragePoint.Domain.Service.Helper
{
    using System.Collections.Generic;
    using System.Linq;

    using StoragePoint.Contracts.Domain.FileStorage.Model;

    public interface IUpdatedFilesJoiner
    {
        IList<FileModel> JoinTheSame(IReadOnlyList<FileModel> files);
    }

    public class UpdatedFilesJoiner : IUpdatedFilesJoiner
    {
        public IList<FileModel> JoinTheSame(IReadOnlyList<FileModel> files)
        {
            IList<FileModel> result = new List<FileModel>();
            foreach (FileModel file in files)
            {
                FileModel existingFile = result.FirstOrDefault(f => f.FileStorageId == file.FileStorageId);
                if (existingFile != null)
                {
                    if (file.UpdateTime > existingFile.UpdateTime)
                    {
                        result.Remove(existingFile);
                        result.Add(file);
                    }
                }
                else
                {
                    result.Add(file);
                }
            }

            return result;
        }
    }
}