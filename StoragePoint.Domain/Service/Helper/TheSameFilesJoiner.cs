using System.Collections.Generic;
using System.Linq;
using StoragePoint.Contracts.Domain.FileStorage.Model;

namespace StoragePoint.Domain.Service.Helper
{
    internal interface IListFilesJoiner
    {
        IList<FileModel> JoinTheSame(IReadOnlyList<FileModel> files);
    }

    internal class TheSameFilesJoiner : IListFilesJoiner
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