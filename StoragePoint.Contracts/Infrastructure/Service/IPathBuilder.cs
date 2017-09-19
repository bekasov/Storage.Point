namespace StoragePoint.Contracts.Infrastructure.Service
{
    using System.Collections.Generic;

    using StoragePoint.Contracts.Domain.FileStorage.Model;
    using StoragePoint.Contracts.Domain.FileStorage.Pto;

    public interface IPathBuilder
    {
        IReadOnlyList<FullPathFilePto> GetPaths(IReadOnlyList<FileModel> files);
    }
}