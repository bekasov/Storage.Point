namespace StoragePoint.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FakeItEasy;

    using StoragePoint.Domain.Model;
    using StoragePoint.Domain.Repository;
    using StoragePoint.Domain.Service;

    using Xunit;

    public class UpdatesDetectorUnitTests
    {
        private readonly StorageContent referenceContent;
        private readonly StorageContent correctContent;

        private readonly UpdatesDetector detector;

        public UpdatesDetectorUnitTests()
        {
            this.correctContent = new StorageContent
            {
                Files = this.GetCorrectTestContent()
            };

            this.referenceContent = new StorageContent
            {
                Files = this.GetReferenceContent()
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

            Assert.Throws<ArgumentException>(() => this.detector.Detect(storageContentWithoutRoot1, this.correctContent));
            Assert.Throws<ArgumentException>(() => this.detector.Detect(this.correctContent, storageContentWithoutRoot1));
            Assert.Throws<ArgumentException>(() => this.detector.Detect(storageContentWithoutRoot2, this.correctContent));
            Assert.Throws<ArgumentException>(() => this.detector.Detect(this.correctContent, storageContentWithoutRoot2));
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

            Assert.Throws<ArgumentException>(() => this.detector.Detect(storageContentWithSeveralRoots, this.correctContent));
            Assert.Throws<ArgumentException>(() => this.detector.Detect(this.correctContent, storageContentWithSeveralRoots));
        }

        [Fact]
        public void DetectNewFiles_NewFileInSource_ItMustAddNewFileInfoInResult()
        {
            FileModel newFile1 = new FileModel { FileOsId = int.MaxValue };
            FileModel newFile2 = new FileModel { FileOsId = int.MaxValue - 1 };
            var storageContentWithNewFile = new StorageContent
            {
                Files = new List<FileModel>(this.GetCorrectTestContent()) { newFile1, newFile2 }
            };

            RepositoryUpdates actualUpdates = this.detector.Detect(this.correctContent, storageContentWithNewFile);

            Assert.Equal(2, actualUpdates.Added.Count);
            Assert.True(actualUpdates.Added.Contains(newFile1));
            Assert.True(actualUpdates.Added.Contains(newFile2));
        }

        private FileModel[] GetReferenceContent()
        {
            return new[]
            {
                new FileModel { FileOsId = 1, ParentFileOsId = 0, FileType = FileType.FOLDER },
            };
        }

        private FileModel[] GetCorrectTestContent()
        {
            return new[]
            {
                new FileModel { FileOsId = 1, ParentFileOsId = 0, FileType = FileType.FOLDER },
                new FileModel { FileOsId = 2, ParentFileOsId = 1, FileType = FileType.FILE },
                new FileModel { FileOsId = 3, ParentFileOsId = 1, FileType = FileType.FILE },
            };
        }
    }
}