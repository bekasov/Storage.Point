namespace StoragePoint.UnitTests.Tests.Model
{
    using System.Collections.Generic;
    using System.Linq;

    using StoragePoint.Contracts.Domain.Changes.Model;
    using StoragePoint.Contracts.Domain.Exceptions;
    using StoragePoint.Contracts.Domain.FileStorage.Model;
    using StoragePoint.Domain.Service;

    using Xunit;

    public class StorageExtendedAndSplitUpdatesForFoldersUnitTests
    {
        private readonly MixedChangesSplitter changesProcessor;

        public StorageExtendedAndSplitUpdatesForFoldersUnitTests()
        {
            this.changesProcessor = new MixedChangesSplitter();
        }

        [Fact]
        public void ExtendedUpdates_UpdatedFolderInUpdates_ItMustThrowWrongTypeOfUpdatesException()
        {
            MixedChanges changesWithChangedFolder = new MixedChanges(
                new FileModel[0],
                new FileModel[0],
                new FileModel[] { new FileModel { FileType = FileType.FILE }, new FileModel { FileType = FileType.FOLDER } },
                new FileModel[0],
                new FileModel[0]);

            Assert.Throws<WrongDetectedChangeKind>(() => this.changesProcessor.Split(changesWithChangedFolder));
        }

        [Fact]
        public void ExtendedUpdatesForFilesWithoutMix_CorrectParams_ItMustCreateCorrectExtendedUpdatesModel()
        {
            IReadOnlyList<ChangedFile> actualResult = this.changesProcessor.Split(this.CreateUpdates());

            this.AssetSeparatedResult(actualResult);
            Assert.False(actualResult.Any(f => f.ChangeKind == FileChange.RENAMED_UPDATED));
            Assert.False(actualResult.Any(f => f.ChangeKind == FileChange.MOVED_UPDATED));
            Assert.False(actualResult.Any(f => f.ChangeKind == FileChange.RENAMED_MOVED));
            Assert.False(actualResult.Any(f => f.ChangeKind == FileChange.RENAMED_MOVED_UPDATED));
        }

        [Fact]
        public void ExtendedUpdatesForFilesWithMix_CorrectParams_ItMustCreateCorrectExtendedUpdatesModel()
        {
            IReadOnlyList<ChangedFile> actualResult = this.changesProcessor.Split(this.CreateUpdates(true));

            this.AssetSeparatedResult(actualResult);

            Assert.True(actualResult.Count(f => f.ChangeKind == FileChange.RENAMED_UPDATED) == 2);
            Assert.Contains(actualResult, f => f.File.FileOsId == 11 && f.ChangeKind == FileChange.RENAMED_UPDATED);
            Assert.Contains(actualResult, f => f.File.FileOsId == 12 && f.ChangeKind == FileChange.RENAMED_UPDATED);

            Assert.True(actualResult.Count(f => f.ChangeKind == FileChange.MOVED_UPDATED) == 2);
            Assert.Contains(actualResult, f => f.File.FileOsId == 13 && f.ChangeKind == FileChange.MOVED_UPDATED);
            Assert.Contains(actualResult, f => f.File.FileOsId == 14 && f.ChangeKind == FileChange.MOVED_UPDATED);

            Assert.True(actualResult.Count(f => f.ChangeKind == FileChange.RENAMED_MOVED) == 4);
            Assert.Contains(actualResult, f => f.File.FileOsId == 15 && f.ChangeKind == FileChange.RENAMED_MOVED);
            Assert.Contains(actualResult, f => f.File.FileOsId == 16 && f.ChangeKind == FileChange.RENAMED_MOVED);
            Assert.Contains(actualResult, f => f.File.FileOsId == 17 && f.ChangeKind == FileChange.RENAMED_MOVED);
            Assert.Contains(actualResult, f => f.File.FileOsId == 18 && f.ChangeKind == FileChange.RENAMED_MOVED);

            Assert.True(actualResult.Count(f => f.ChangeKind == FileChange.RENAMED_MOVED_UPDATED) == 2);
            Assert.Contains(actualResult, f => f.File.FileOsId == 19 && f.ChangeKind == FileChange.RENAMED_MOVED_UPDATED);
            Assert.Contains(actualResult, f => f.File.FileOsId == 20 && f.ChangeKind == FileChange.RENAMED_MOVED_UPDATED);
        }

        private MixedChanges CreateUpdates(bool addMixedChanges = false)
        {
            FileModel addedFolder1 = new FileModel { FileOsId = 1, FileType = FileType.FOLDER };
            FileModel addedFile2 = new FileModel { FileOsId = 2, FileType = FileType.FILE };

            FileModel removedFolder1 = new FileModel { FileOsId = 3, FileType = FileType.FOLDER };
            FileModel removedFile2 = new FileModel { FileOsId = 4, FileType = FileType.FILE };

            FileModel updatedFile1 = new FileModel { FileOsId = 5, FileType = FileType.FILE };
            FileModel updatedFile2 = new FileModel { FileOsId = 6, FileType = FileType.FILE };

            FileModel renamedFolder1 = new FileModel { FileOsId = 7, FileType = FileType.FOLDER };
            FileModel renamedFile2 = new FileModel { FileOsId = 8, FileType = FileType.FILE };

            FileModel movedFolder1 = new FileModel { FileOsId = 9, FileType = FileType.FOLDER };
            FileModel movedFile2 = new FileModel { FileOsId = 10, FileType = FileType.FILE };

            List<FileModel> added = new List<FileModel> { addedFolder1, addedFile2 };
            List<FileModel> removed = new List<FileModel> { removedFolder1, removedFile2 };
            List<FileModel> updated = new List<FileModel> { updatedFile1, updatedFile2 };
            List<FileModel> renamed = new List<FileModel> { renamedFolder1, renamedFile2 };
            List<FileModel> moved = new List<FileModel> { movedFolder1, movedFile2 };

            if (addMixedChanges)
            {
                FileModel renamedAndUpdatedFile1 = new FileModel { FileOsId = 11, FileType = FileType.FILE };
                FileModel renamedAndUpdated2 = new FileModel { FileOsId = 12, FileType = FileType.FILE };
                renamed.AddRange(new[] { renamedAndUpdatedFile1, renamedAndUpdated2 });
                updated.AddRange(new[] { renamedAndUpdatedFile1, renamedAndUpdated2 });

                FileModel movedAndUpdatedFile1 = new FileModel { FileOsId = 13, FileType = FileType.FILE };
                FileModel movedAndUpdatedFile2 = new FileModel { FileOsId = 14, FileType = FileType.FILE };
                moved.AddRange(new[] { movedAndUpdatedFile1, movedAndUpdatedFile2 });
                updated.AddRange(new[] { movedAndUpdatedFile1, movedAndUpdatedFile2 });

                FileModel renamedAndMovedFile1 = new FileModel { FileOsId = 15, FileType = FileType.FILE };
                FileModel renamedAndMovedFile2 = new FileModel { FileOsId = 16, FileType = FileType.FILE };
                FileModel renamedAndMovedFolder3 = new FileModel { FileOsId = 17, FileType = FileType.FOLDER };
                FileModel renamedAndMovedFolder4 = new FileModel { FileOsId = 18, FileType = FileType.FILE };
                moved.AddRange(new[] { renamedAndMovedFile1, renamedAndMovedFile2, renamedAndMovedFolder3, renamedAndMovedFolder4 });
                renamed.AddRange(new[] { renamedAndMovedFile1, renamedAndMovedFile2, renamedAndMovedFolder3, renamedAndMovedFolder4 });

                FileModel renamedMovedAndUpdatedFile1 = new FileModel { FileOsId = 19, FileType = FileType.FILE };
                FileModel renamedMovedAndUpdatedFile2 = new FileModel { FileOsId = 20, FileType = FileType.FILE };
                moved.AddRange(new[] { renamedMovedAndUpdatedFile1, renamedMovedAndUpdatedFile2 });
                updated.AddRange(new[] { renamedMovedAndUpdatedFile1, renamedMovedAndUpdatedFile2 });
                renamed.AddRange(new[] { renamedMovedAndUpdatedFile1, renamedMovedAndUpdatedFile2 });
            }

            MixedChanges result = new MixedChanges(added, removed, updated, renamed, moved);

            return result;
        }

        private void AssetSeparatedResult(IReadOnlyList<ChangedFile> actualResult)
        {
            Assert.True(actualResult.Count(f => f.ChangeKind == FileChange.ADDED) == 2);
            Assert.Contains(actualResult, f => f.File.FileOsId == 1 && f.ChangeKind == FileChange.ADDED);
            Assert.Contains(actualResult, f => f.File.FileOsId == 2 && f.ChangeKind == FileChange.ADDED);

            Assert.True(actualResult.Count(f => f.ChangeKind == FileChange.REMOVED) == 2);
            Assert.Contains(actualResult, f => f.File.FileOsId == 3 && f.ChangeKind == FileChange.REMOVED);
            Assert.Contains(actualResult, f => f.File.FileOsId == 4 && f.ChangeKind == FileChange.REMOVED);

            Assert.True(actualResult.Count(f => f.ChangeKind == FileChange.UPDATED) == 2);
            Assert.Contains(actualResult, f => f.File.FileOsId == 5 && f.ChangeKind == FileChange.UPDATED);
            Assert.Contains(actualResult, f => f.File.FileOsId == 6 && f.ChangeKind == FileChange.UPDATED);

            Assert.True(actualResult.Count(f => f.ChangeKind == FileChange.RENAMED) == 2);
            Assert.Contains(actualResult, f => f.File.FileOsId == 7 && f.ChangeKind == FileChange.RENAMED);
            Assert.Contains(actualResult, f => f.File.FileOsId == 8 && f.ChangeKind == FileChange.RENAMED);

            Assert.True(actualResult.Count(f => f.ChangeKind == FileChange.MOVED) == 2);
            Assert.Contains(actualResult, f => f.File.FileOsId == 9 && f.ChangeKind == FileChange.MOVED);
            Assert.Contains(actualResult, f => f.File.FileOsId == 10 && f.ChangeKind == FileChange.MOVED);
        }
    }
}