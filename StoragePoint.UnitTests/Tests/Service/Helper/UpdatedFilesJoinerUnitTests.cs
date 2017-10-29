namespace StoragePoint.UnitTests.Tests.Service.Helper
{
    using System;
    using System.Collections.Generic;

    using StoragePoint.Contracts.Domain.FileStorage.Model;
    using StoragePoint.Domain.Service.Helper;

    using Xunit;

    public class UpdatedFilesJoinerUnitTests
    {
        private readonly UpdatedFilesJoiner joiner = new UpdatedFilesJoiner();

        [Fact]
        public void UpdatedFilesJoiner_NoSameFiles_ItMustReturnTheSameCollection()
        {
            IReadOnlyList<FileModel> expectedCollection = new List<FileModel>
            {
                new FileModel { FileStorageId = 1 },
                new FileModel { FileStorageId = 2 },
            };

            IReadOnlyList<FileModel> actualCollection = this.joiner.JoinTheSame(expectedCollection);
            
            Assert.Equal(expectedCollection, actualCollection);
        }

        [Fact]
        public void UpdatedFilesJoiner_SeveralSameFiles_ItMustReturnJoinedCollection()
        {
            FileModel file1 = new FileModel { FileStorageId = 1 };
            FileModel file1_2 = new FileModel { FileStorageId = 1 };
            FileModel file2 = new FileModel { FileStorageId = 2 };
            FileModel file2_2 = new FileModel { FileStorageId = 2 };
            FileModel file3 = new FileModel { FileStorageId = 3 };
            
            IReadOnlyList<FileModel> collection = new List<FileModel> { file1, file2, file1_2, file3, file2_2 };

            IReadOnlyList<FileModel> expectedCollection = new List<FileModel> { file1, file2, file3 };

            IReadOnlyList<FileModel> actualCollection = this.joiner.JoinTheSame(collection);

            Assert.Equal(expectedCollection, actualCollection);
        }

        [Fact]
        public void UpdatedFilesJoiner_SeveralSameFilesWithDifferentDates_ItMustReturnJoinedCollectionWithNewestFiles()
        {
            FileModel file1 = new FileModel { FileStorageId = 1, UpdateTime = DateTime.Now };
            FileModel file1_2 = new FileModel { FileStorageId = 1, UpdateTime = DateTime.Now.AddHours(1) };
            FileModel file2 = new FileModel { FileStorageId = 2, UpdateTime = DateTime.Now.AddMinutes(1) };
            FileModel file2_2 = new FileModel { FileStorageId = 2, UpdateTime = DateTime.Now };
            FileModel file3 = new FileModel { FileStorageId = 3 };

            IReadOnlyList<FileModel> collection = new List<FileModel> { file1, file2, file1_2, file3, file2_2 };

            IReadOnlyList<FileModel> expectedCollection = new List<FileModel> { file2, file1_2, file3 };

            IReadOnlyList<FileModel> actualCollection = this.joiner.JoinTheSame(collection);

            Assert.Equal(expectedCollection, actualCollection);
        }
    }
}