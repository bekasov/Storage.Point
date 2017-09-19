namespace StoragePoint.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using StoragePoint.Contracts.Domain.FileStorage.Model;
    using StoragePoint.Domain.Service;
    using StoragePoint.UnitTests.Helper;

    using Xunit;

    public class UpdatesDetectorUnitTests
    {
        private readonly StorageContent referenceContent;
        private readonly StorageContent sourceContent;

        private readonly UpdatesDetector detector;

        private readonly DateTime now;

        public UpdatesDetectorUnitTests()
        {
            List<FileModel> referenceFiles = new List<FileModel>(this.CreateReferenceContent());
            referenceFiles.Shuffle();
            this.referenceContent = new StorageContent(0, referenceFiles);

            List<FileModel> sourceFiles = new List<FileModel>(this.CreateChangedContent());
            sourceFiles.Shuffle();
            this.sourceContent = new StorageContent(0, sourceFiles);

            this.now = DateTime.Parse("2017-09-16 17:44:23");

            this.detector = new UpdatesDetector();
        }

        [Fact]
        public void UpdatesDetectorPreConditions_SomeContentIsNull_ItMustThrowNullArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() => this.detector.Detect(null, new StorageContent(0, null)));
            Assert.Throws<ArgumentNullException>(() => this.detector.Detect(new StorageContent(0, null), null));
        }

        [Fact]
        public void UpdatesDetectorNewFiles_CorrectArgs_ItMustAddNewFilesToResult()
        {
            StorageUpdates actualUpdates = this.detector.Detect(this.referenceContent, this.sourceContent);

            Assert.Equal(6, actualUpdates.Added.Count);
            Assert.True(actualUpdates.Added.Any(f => f.FileOsId == 17));
            Assert.True(actualUpdates.Added.Any(f => f.FileOsId == 18));
            Assert.True(actualUpdates.Added.Any(f => f.FileOsId == 19));
            Assert.True(actualUpdates.Added.Any(f => f.FileOsId == 20));
            Assert.True(actualUpdates.Added.Any(f => f.FileOsId == 21));
            Assert.True(actualUpdates.Added.Any(f => f.FileOsId == 22));
        }

        [Fact]
        public void UpdatesDetectorNewFiles_EmptyReference_ItMustAddAllNewFilesToResult()
        {
            StorageUpdates actualUpdates = this.detector.Detect(
                new StorageContent(0, new FileModel[0]), this.sourceContent);

            Assert.Equal(this.sourceContent.Files.Count, actualUpdates.Added.Count);
            Assert.Contains(actualUpdates.Added, f => this.sourceContent.Files.Contains(f));
        }

        [Fact]
        public void UpdatesDetectorNewFiles_EmptyReferenceAndSource_ItMustNotAddAnyNewFilesToResult()
        {
            StorageUpdates actualUpdates = this.detector.Detect(
                new StorageContent(0, new FileModel[0]),
                new StorageContent(0, new FileModel[0]));

            Assert.Equal(0, actualUpdates.Added.Count);
        }

        [Fact]
        public void UpdatesDetectorRemovedFiles_CorrectArgs_ItMustAddRemovedFilesToResult()
        {
            StorageUpdates actualUpdates = this.detector.Detect(this.referenceContent, this.sourceContent);

            Assert.Equal(4, actualUpdates.Removed.Count);
            Assert.True(actualUpdates.Removed.Any(f => f.FileOsId == 04));
            Assert.True(actualUpdates.Removed.Any(f => f.FileOsId == 14));
            Assert.True(actualUpdates.Removed.Any(f => f.FileOsId == 15));
            Assert.True(actualUpdates.Removed.Any(f => f.FileOsId == 16));
        }

        [Fact]
        public void UpdatesDetectorRemovedFiles_EmptySource_ItMustAddAllFilesToResult()
        {
            StorageUpdates actualUpdates = this.detector.Detect(
                this.referenceContent,
                new StorageContent(0, new FileModel[0]));

            Assert.Equal(this.referenceContent.Files.Count, actualUpdates.Removed.Count);
            Assert.Contains(actualUpdates.Removed, f => this.referenceContent.Files.Contains(f));
        }

        [Fact]
        public void UpdatesDetectorMovingFiles_CorrectArgs_ItMustAddMovedFilesInfoInResult()
        {
            StorageUpdates actualUpdates = this.detector.Detect(this.referenceContent, this.sourceContent);

            Assert.Equal(5, actualUpdates.Moved.Count);
            Assert.True(actualUpdates.Moved.Any(f => f.FileOsId == 08));
            Assert.True(actualUpdates.Moved.Any(f => f.FileOsId == 11));
            Assert.True(actualUpdates.Moved.Any(f => f.FileOsId == 12));
            Assert.True(actualUpdates.Moved.Any(f => f.FileOsId == 13));
            Assert.True(actualUpdates.Moved.Any(f => f.FileOsId == 06));
        }

        [Fact]
        public void UpdatesDetectorRenamingFiles_CorrectArgs_ItMustAddRenamedFilesInfoInResult()
        {
            StorageUpdates actualUpdates = this.detector.Detect(this.referenceContent, this.sourceContent);

            Assert.Equal(4, actualUpdates.Renamed.Count);
            Assert.True(actualUpdates.Renamed.Any(f => f.FileOsId == 03));
            Assert.True(actualUpdates.Renamed.Any(f => f.FileOsId == 09));
            Assert.True(actualUpdates.Renamed.Any(f => f.FileOsId == 11));
            Assert.True(actualUpdates.Renamed.Any(f => f.FileOsId == 13));
        }

        [Fact]
        public void UpdatesDetectorChangingFiles_CorrectArgs_ItMustAddUpdatedFilesInfoInResult()
        {
            StorageUpdates actualUpdates = this.detector.Detect(this.referenceContent, this.sourceContent);

            Assert.Equal(3, actualUpdates.Changed.Count);
            Assert.True(actualUpdates.Changed.Any(f => f.FileOsId == 13));
            Assert.True(actualUpdates.Changed.Any(f => f.FileOsId == 09));
            Assert.True(actualUpdates.Changed.Any(f => f.FileOsId == 10));
        }

        private FileModel[] CreateReferenceContent()
        {
            return new[]
            {
                new FileModel { FileOsId = 01, UpdateTime = this.now, ParentFileOsId = 00, FileType = FileType.FILE }, // 1
                new FileModel { FileOsId = 02, UpdateTime = this.now, ParentFileOsId = 00, FileType = FileType.FOLDER }, // 2
                new FileModel { FileOsId = 03, UpdateTime = this.now, ParentFileOsId = 00, FileType = FileType.FOLDER, Name = "FD3" }, // 3
                new FileModel { FileOsId = 04, UpdateTime = this.now, ParentFileOsId = 00, FileType = FileType.FOLDER }, // 4
                new FileModel { FileOsId = 05, UpdateTime = this.now, ParentFileOsId = 00, FileType = FileType.FILE },   // 5
                new FileModel { FileOsId = 06, UpdateTime = this.now, ParentFileOsId = 00, FileType = FileType.FILE },   // 6

                new FileModel { FileOsId = 07, UpdateTime = this.now, ParentFileOsId = 02, FileType = FileType.FOLDER }, // 7
                new FileModel { FileOsId = 08, UpdateTime = this.now, ParentFileOsId = 02, FileType = FileType.FOLDER }, // 8
                new FileModel { FileOsId = 09, UpdateTime = this.now, ParentFileOsId = 02, FileType = FileType.FILE, Name = "F9" }, // 9
                new FileModel { FileOsId = 10, UpdateTime = this.now, ParentFileOsId = 02, FileType = FileType.FILE },   // 10

                new FileModel { FileOsId = 11, UpdateTime = this.now, ParentFileOsId = 03, FileType = FileType.FOLDER, Name = "FD11" }, // 11
                new FileModel { FileOsId = 12, UpdateTime = this.now, ParentFileOsId = 11, FileType = FileType.FOLDER }, // 12
                new FileModel { FileOsId = 13, UpdateTime = this.now, ParentFileOsId = 12, FileType = FileType.FILE, Name = "F13" },   // 13

                new FileModel { FileOsId = 14, UpdateTime = this.now, ParentFileOsId = 04, FileType = FileType.FOLDER }, // 14
                new FileModel { FileOsId = 15, UpdateTime = this.now, ParentFileOsId = 14, FileType = FileType.FOLDER }, // 15
                new FileModel { FileOsId = 16, UpdateTime = this.now, ParentFileOsId = 15, FileType = FileType.FILE },   // 16
            };
        }

        private FileModel[] CreateChangedContent(FileModel[] source = null)
        {
            List<FileModel> result = source?.ToList() ?? this.referenceContent?.Files?.ToList() ?? new List<FileModel>();

            // NEW
            var newFile1 = new FileModel { FileOsId = 17, ParentFileOsId = 00, FileType = FileType.FOLDER };  // 17
            var newFile2 = new FileModel { FileOsId = 18, ParentFileOsId = 12, FileType = FileType.FILE };   // 18
            var newFile3 = new FileModel { FileOsId = 19, ParentFileOsId = 17, FileType = FileType.FILE };   // 19
            var newFile4 = new FileModel { FileOsId = 20, ParentFileOsId = 17, FileType = FileType.FOLDER }; // 20
            var newFile5 = new FileModel { FileOsId = 21, ParentFileOsId = 20, FileType = FileType.FOLDER }; // 21
            var newFile6 = new FileModel { FileOsId = 22, ParentFileOsId = 21, FileType = FileType.FILE };   // 22
            result.AddRange(new FileModel[] { newFile1, newFile2, newFile3, newFile4, newFile5, newFile6 });

            // REMOVED
            var removedFile1 = result.Find(f => f.FileOsId == 04 && f.FileType == FileType.FOLDER);
            var removedFile2 = result.Find(f => f.FileOsId == 14 && f.FileType == FileType.FOLDER);
            var removedFile3 = result.Find(f => f.FileOsId == 15 && f.FileType == FileType.FOLDER);
            var removedFile4 = result.Find(f => f.FileOsId == 16 && f.FileType == FileType.FILE);
            result.RemoveAll(f => new FileModel[] { removedFile1, removedFile2, removedFile3, removedFile4 }.Contains(f));

            // MOVED
            result.RemoveAll(f => new int[] { 08, 11, 12, 13, 06 }.Contains(f.FileOsId));
            var movedFile1 = new FileModel { FileOsId = 08, ParentFileOsId = 11, FileType = FileType.FOLDER, UpdateTime = this.now }; // 08 --> 11
            var movedFile2 = new FileModel { FileOsId = 11, ParentFileOsId = 00, FileType = FileType.FOLDER, UpdateTime = this.now }; // 11 --> 00
            var movedFile3 = new FileModel { FileOsId = 12, ParentFileOsId = 17, FileType = FileType.FOLDER, UpdateTime = this.now }; // 12 --> 17
            var movedFile4 = new FileModel { FileOsId = 13, ParentFileOsId = 12, FileType = FileType.FILE, UpdateTime = this.now };   // no changes
            var movedFile5 = new FileModel { FileOsId = 06, ParentFileOsId = 11, FileType = FileType.FOLDER, UpdateTime = this.now }; // 06 --> 11
            result.AddRange(new FileModel[] { movedFile1, movedFile2, movedFile3, movedFile4, movedFile5 });

            // RENAMED
            result.RemoveAll(f => new int[] { 03, 09 }.Contains(f.FileOsId));
            var renamedFile1 = new FileModel { FileOsId = 03, Name = "fd 3",  ParentFileOsId = 00, FileType = FileType.FOLDER, UpdateTime = this.now }; // 3
            movedFile2.Name = "FD-11"; // 11
            movedFile4.Name = "F_13"; // 13
            var renamedFile4 = new FileModel { FileOsId = 09, Name = "F 9",  ParentFileOsId = 02, FileType = FileType.FILE, UpdateTime = this.now };   // 16
            result.AddRange(new FileModel[] { renamedFile1, renamedFile4 });

            // CHANGED
            movedFile4.UpdateTime = movedFile4.UpdateTime.AddMinutes(2);
            renamedFile4.UpdateTime = renamedFile4.UpdateTime.AddMilliseconds(2);
            result.RemoveAll(f => new int[] { 10 }.Contains(f.FileOsId));
            var changedFile1 = new FileModel { FileOsId = 10, UpdateTime = this.now.AddHours(1), ParentFileOsId = 02, FileType = FileType.FILE };  // 10
            result.AddRange(new FileModel[] { changedFile1 });

            return result.ToArray();
        }
    }
}