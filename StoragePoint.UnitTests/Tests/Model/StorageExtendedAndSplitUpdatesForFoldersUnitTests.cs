namespace StoragePoint.UnitTests.Tests.Model
{
    using System.Collections.Generic;

    using StoragePoint.Contracts.Domain.Exceptions;
    using StoragePoint.Contracts.Domain.FileStorage.Model;
    using StoragePoint.Domain.Model;

    using Xunit;

    public class StorageExtendedAndSplitUpdatesForFoldersUnitTests
    {
        private ExtendedUpdates model;

        [Fact]
        public void ExtendedUpdates_UpdatedFolderInUpdates_ItMustThrowWrongTypeOfUpdatesException()
        {
            StorageUpdates updatesWithChangedFolder = new StorageUpdates(
                0,
                new FileModel[0],
                new FileModel[0],
                new FileModel[] { new FileModel { FileType = FileType.FILE }, new FileModel { FileType = FileType.FOLDER } },
                new FileModel[0],
                new FileModel[0]);

            Assert.Throws<WrongTypeOfUpdates>(() => this.model = new ExtendedUpdates(updatesWithChangedFolder));
        }

        [Fact]
        public void ExtendedUpdatesForFilesWithoutMix_CorrectParams_ItMustCreateCorrectExtendedUpdatesModel()
        {
            this.model = new ExtendedUpdates(this.CreateUpdates());

            Assert.True(this.model.Added.Count == 2);
            Assert.Contains(this.model.Added, af => af.FileOsId == 1);
            Assert.Contains(this.model.Added, af => af.FileOsId == 2);

            Assert.True(this.model.Removed.Count == 2);
            Assert.Contains(this.model.Removed, af => af.FileOsId == 3);
            Assert.Contains(this.model.Removed, af => af.FileOsId == 4);

            Assert.True(this.model.Updated.Count == 2);
            Assert.Contains(this.model.Updated, af => af.FileOsId == 5);
            Assert.Contains(this.model.Updated, af => af.FileOsId == 6);

            Assert.True(this.model.Renamed.Count == 2);
            Assert.Contains(this.model.Renamed, af => af.FileOsId == 7);
            Assert.Contains(this.model.Renamed, af => af.FileOsId == 8);

            Assert.True(this.model.Moved.Count == 2);
            Assert.Contains(this.model.Moved, af => af.FileOsId == 9);
            Assert.Contains(this.model.Moved, af => af.FileOsId == 10);

            Assert.True(this.model.RenamedAndUpdated.Count == 0);
            Assert.True(this.model.MovedAndUpdated.Count == 0);
            Assert.True(this.model.RenamedAndMoved.Count == 0);
            Assert.True(this.model.RenamedAndMovedAndUpdated.Count == 0);
        }

        [Fact]
        public void ExtendedUpdatesForFilesWithMix_CorrectParams_ItMustCreateCorrectExtendedUpdatesModel()
        {
            this.model = new ExtendedUpdates(this.CreateUpdates(true));

            Assert.True(this.model.Added.Count == 2);
            Assert.Contains(this.model.Added, af => af.FileOsId == 1);
            Assert.Contains(this.model.Added, af => af.FileOsId == 2);

            Assert.True(this.model.Removed.Count == 2);
            Assert.Contains(this.model.Removed, af => af.FileOsId == 3);
            Assert.Contains(this.model.Removed, af => af.FileOsId == 4);

            Assert.True(this.model.Updated.Count == 2);
            Assert.Contains(this.model.Updated, af => af.FileOsId == 5);
            Assert.Contains(this.model.Updated, af => af.FileOsId == 6);

            Assert.True(this.model.Renamed.Count == 2);
            Assert.Contains(this.model.Renamed, af => af.FileOsId == 7);
            Assert.Contains(this.model.Renamed, af => af.FileOsId == 8);

            Assert.True(this.model.Moved.Count == 2);
            Assert.Contains(this.model.Moved, af => af.FileOsId == 9);
            Assert.Contains(this.model.Moved, af => af.FileOsId == 10);

            Assert.True(this.model.RenamedAndUpdated.Count == 2);
            Assert.Contains(this.model.RenamedAndUpdated, af => af.FileOsId == 11);
            Assert.Contains(this.model.RenamedAndUpdated, af => af.FileOsId == 12);

            Assert.True(this.model.MovedAndUpdated.Count == 2);
            Assert.Contains(this.model.MovedAndUpdated, af => af.FileOsId == 13);
            Assert.Contains(this.model.MovedAndUpdated, af => af.FileOsId == 14);

            Assert.True(this.model.RenamedAndMoved.Count == 4);
            Assert.Contains(this.model.RenamedAndMoved, af => af.FileOsId == 15);
            Assert.Contains(this.model.RenamedAndMoved, af => af.FileOsId == 16);
            Assert.Contains(this.model.RenamedAndMoved, af => af.FileOsId == 17);
            Assert.Contains(this.model.RenamedAndMoved, af => af.FileOsId == 18);

            Assert.True(this.model.RenamedAndMovedAndUpdated.Count == 2);
            Assert.Contains(this.model.RenamedAndMovedAndUpdated, af => af.FileOsId == 19);
            Assert.Contains(this.model.RenamedAndMovedAndUpdated, af => af.FileOsId == 20);
        }

        private StorageUpdates CreateUpdates(bool addMixedChanges = false)
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

            StorageUpdates result = new StorageUpdates(0, added, removed, updated, renamed, moved);

            return result;
        }
    }
}