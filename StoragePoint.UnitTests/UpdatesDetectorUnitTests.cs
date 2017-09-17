namespace StoragePoint.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using StoragePoint.Domain.Model;
    using StoragePoint.Domain.Service;
    using StoragePoint.UnitTests.Helper;

    using Xunit;

    public class UpdatesDetectorUnitTests
    {
        private readonly StorageContent referenceContent;
        private readonly List<FileModel> referenceFiles;

        private readonly StorageContent sourceContent;
        private readonly List<FileModel> sourceFiles;

        private readonly UpdatesDetector detector;

        private readonly DateTime nowDateTime = DateTime.Now;

        public UpdatesDetectorUnitTests()
        {
            this.referenceFiles = new List<FileModel>(this.CreateReferenceContent());
            this.referenceFiles.Shuffle();
            this.referenceContent = new StorageContent
            {
                Files = this.referenceFiles
            };

            this.sourceFiles = new List<FileModel>(this.CreateChangedContent());
            this.sourceFiles.Shuffle();
            this.sourceContent = new StorageContent
            {
                Files = this.sourceFiles
            };

            this.detector = new UpdatesDetector();
        }

        [Fact]
        public void UpdatesDetectorPreConditions_SomeContentIsNull_ItMustThrowNullArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() => this.detector.Detect(null, new StorageContent()));
            Assert.Throws<ArgumentNullException>(() => this.detector.Detect(new StorageContent(), null));
        }

        [Fact]
        public void UpdatesDetectorPreConditions_ContentHasNotRootFolder_ItMustThrowArgumentException()
        {
            var storageContentWithoutRoot1 = new StorageContent
            {
                Files = new[] { new FileModel { ParentFileOsId = 1, FileType = FileType.FOLDER } }
            };
            
            var storageContentWithoutRoot2 = new StorageContent
            {
                Files = new[] { new FileModel { ParentFileOsId = 0, FileType = FileType.FILE } }
            };

            Assert.Throws<ArgumentException>(() => this.detector.Detect(storageContentWithoutRoot1, this.sourceContent));
            Assert.Throws<ArgumentException>(() => this.detector.Detect(this.sourceContent, storageContentWithoutRoot1));
            Assert.Throws<ArgumentException>(() => this.detector.Detect(storageContentWithoutRoot2, this.sourceContent));
            Assert.Throws<ArgumentException>(() => this.detector.Detect(this.sourceContent, storageContentWithoutRoot2));
        }

        [Fact]
        public void UpdatesDetectorPreConditions_ContentSeveralRootFolders_ItMustThrowArgumentException()
        {
            var storageContentWithSeveralRoots = new StorageContent
            {
                Files = new[]
                {
                    new FileModel { ParentFileOsId = 0, FileType = FileType.FOLDER },
                    new FileModel { ParentFileOsId = 0, FileType = FileType.FOLDER }
                }
            };

            Assert.Throws<ArgumentException>(() => this.detector.Detect(storageContentWithSeveralRoots, this.sourceContent));
            Assert.Throws<ArgumentException>(() => this.detector.Detect(this.sourceContent, storageContentWithSeveralRoots));
        }

        [Fact]
        public void UpdatesDetectorNewFiles_CorrectArgs_ItMustAddNewFilesToResult()
        {
            RepositoryUpdates actualUpdates = this.detector.Detect(this.referenceContent, this.sourceContent);

            Assert.Equal(6, actualUpdates.Added.Count);
            Assert.True(actualUpdates.Added.Any(f => f.FileOsId == 17));
            Assert.True(actualUpdates.Added.Any(f => f.FileOsId == 18));
            Assert.True(actualUpdates.Added.Any(f => f.FileOsId == 19));
            Assert.True(actualUpdates.Added.Any(f => f.FileOsId == 20));
            Assert.True(actualUpdates.Added.Any(f => f.FileOsId == 21));
            Assert.True(actualUpdates.Added.Any(f => f.FileOsId == 22));
        }

        [Fact]
        public void UpdatesDetectorNewFiles_ReferenceIsEmpty_ItMustAddNewFilesToResult()
        {
            RepositoryUpdates actualUpdates = this.detector.Detect(new StorageContent
            {
                Files = new List<FileModel> { new FileModel { FileType = FileType.FOLDER, ParentFileOsId = FileModel.ROOT_FOLDER_PARENT_OS_ID } }
            }, this.sourceContent);
        }

        [Fact]
        public void UpdatesDetectorRemovedFiles_CorrectArgs_ItMustAddRemovedFilesToResult()
        {
            RepositoryUpdates actualUpdates = this.detector.Detect(this.referenceContent, this.sourceContent);

            Assert.Equal(4, actualUpdates.Removed.Count);
            Assert.True(actualUpdates.Removed.Any(f => f.FileOsId == 04));
            Assert.True(actualUpdates.Removed.Any(f => f.FileOsId == 14));
            Assert.True(actualUpdates.Removed.Any(f => f.FileOsId == 15));
            Assert.True(actualUpdates.Removed.Any(f => f.FileOsId == 16));
        }

        [Fact]
        public void UpdatesDetectorMovingFiles_CorrectArgs_ItMustAddMovedFilesInfoInResult()
        {
            RepositoryUpdates actualUpdates = this.detector.Detect(this.referenceContent, this.sourceContent);

            Assert.Equal(5, actualUpdates.Moved.Count);
            Assert.True(actualUpdates.Moved.Any(f => f.FileOsId == 08));
            Assert.True(actualUpdates.Moved.Any(f => f.FileOsId == 11));
            Assert.True(actualUpdates.Moved.Any(f => f.FileOsId == 12));
            Assert.True(actualUpdates.Moved.Any(f => f.FileOsId == 13));
            Assert.True(actualUpdates.Moved.Any(f => f.FileOsId == 06));
        }

        private FileModel[] CreateReferenceContent()
        {
            DateTime now = DateTime.Parse("2017-09-16 17:44:23");

            return new[]
            {
                new FileModel { FileOsId = 01, UpdateTime = now, ParentFileOsId = FileModel.ROOT_FOLDER_PARENT_OS_ID, FileType = FileType.FOLDER }, // 1
                new FileModel { FileOsId = 02, UpdateTime = now, ParentFileOsId = 01, FileType = FileType.FOLDER }, // 2
                new FileModel { FileOsId = 03, UpdateTime = now, ParentFileOsId = 01, FileType = FileType.FOLDER }, // 3
                new FileModel { FileOsId = 04, UpdateTime = now, ParentFileOsId = 01, FileType = FileType.FOLDER }, // 4
                new FileModel { FileOsId = 05, UpdateTime = now, ParentFileOsId = 01, FileType = FileType.FILE },   // 5
                new FileModel { FileOsId = 06, UpdateTime = now, ParentFileOsId = 01, FileType = FileType.FILE },   // 6

                new FileModel { FileOsId = 07, UpdateTime = now, ParentFileOsId = 02, FileType = FileType.FOLDER }, // 7
                new FileModel { FileOsId = 08, UpdateTime = now, ParentFileOsId = 02, FileType = FileType.FOLDER }, // 8
                new FileModel { FileOsId = 09, UpdateTime = now, ParentFileOsId = 02, FileType = FileType.FILE },   // 9
                new FileModel { FileOsId = 10, UpdateTime = now, ParentFileOsId = 02, FileType = FileType.FILE },   // 10

                new FileModel { FileOsId = 11, UpdateTime = now, ParentFileOsId = 03, FileType = FileType.FOLDER }, // 11
                new FileModel { FileOsId = 12, UpdateTime = now, ParentFileOsId = 11, FileType = FileType.FOLDER }, // 12
                new FileModel { FileOsId = 13, UpdateTime = now, ParentFileOsId = 12, FileType = FileType.FILE },   // 13

                new FileModel { FileOsId = 14, UpdateTime = now, ParentFileOsId = 04, FileType = FileType.FOLDER }, // 14
                new FileModel { FileOsId = 15, UpdateTime = now, ParentFileOsId = 14, FileType = FileType.FOLDER }, // 15
                new FileModel { FileOsId = 16, UpdateTime = now, ParentFileOsId = 15, FileType = FileType.FILE },   // 16
            };
        }

        private FileModel[] CreateChangedContent(FileModel[] source = null)
        {
            List<FileModel> result = source?.ToList() ?? this.referenceContent?.Files?.ToList() ?? new List<FileModel>();

            // NEW
            FileModel newFile1 = new FileModel { FileOsId = 17, ParentFileOsId = 1, FileType = FileType.FOLDER };  // 17
            FileModel newFile2 = new FileModel { FileOsId = 18, ParentFileOsId = 12, FileType = FileType.FILE };   // 18
            FileModel newFile3 = new FileModel { FileOsId = 19, ParentFileOsId = 17, FileType = FileType.FILE };   // 19
            FileModel newFile4 = new FileModel { FileOsId = 20, ParentFileOsId = 17, FileType = FileType.FOLDER }; // 20
            FileModel newFile5 = new FileModel { FileOsId = 21, ParentFileOsId = 20, FileType = FileType.FOLDER }; // 21
            FileModel newFile6 = new FileModel { FileOsId = 22, ParentFileOsId = 21, FileType = FileType.FILE };   // 22
            result.AddRange(new FileModel[] { newFile1, newFile2, newFile3, newFile4, newFile5, newFile6 });

            // REMOVED
            FileModel removedFile1 = result.Find(f => f.FileOsId == 04 && f.FileType == FileType.FOLDER);
            FileModel removedFile2 = result.Find(f => f.FileOsId == 14 && f.FileType == FileType.FOLDER);
            FileModel removedFile3 = result.Find(f => f.FileOsId == 15 && f.FileType == FileType.FOLDER);
            FileModel removedFile4 = result.Find(f => f.FileOsId == 16 && f.FileType == FileType.FILE);
            result.RemoveAll(f => new FileModel[] { removedFile1, removedFile2, removedFile3, removedFile4 }.Contains(f));

            // MOVED
            result.RemoveAll(f => new int[] { 08, 11, 12, 13, 06 }.Contains(f.FileOsId));
            FileModel movedFile1 = new FileModel { FileOsId = 08, ParentFileOsId = 11, FileType = FileType.FOLDER }; // 08 --> 11
            FileModel movedFile2 = new FileModel { FileOsId = 11, ParentFileOsId = 01, FileType = FileType.FOLDER }; // 11 --> 01
            FileModel movedFile3 = new FileModel { FileOsId = 12, ParentFileOsId = 17, FileType = FileType.FOLDER }; // 12 --> 17
            FileModel movedFile4 = new FileModel { FileOsId = 13, ParentFileOsId = 12, FileType = FileType.FILE };   // no changes
            FileModel movedFile5 = new FileModel { FileOsId = 06, ParentFileOsId = 11, FileType = FileType.FOLDER }; // 06 --> 11
            result.AddRange(new FileModel[] { movedFile1, movedFile2, movedFile3, movedFile4, movedFile5 });

            return result.ToArray();
        }
    }
}