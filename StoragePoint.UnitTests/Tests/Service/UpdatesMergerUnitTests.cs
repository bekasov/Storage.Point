namespace StoragePoint.UnitTests.Tests.Service
{
    using System.Collections.Generic;

    using FakeItEasy;

    using StoragePoint.Contracts.Domain.Changes.Model;
    using StoragePoint.Contracts.Domain.FileStorage.Model;
    using StoragePoint.Domain.Service;
    using StoragePoint.Domain.Service.Helper;

    using Xunit;

    public class UpdatesMergerUnitTests
    {
        private readonly IUpdatedFilesJoiner filesJoiner;
        
        private readonly MixedChangesMerger merger;

        public UpdatesMergerUnitTests()
        {
            this.filesJoiner = A.Fake<IUpdatedFilesJoiner>();

            this.merger = new MixedChangesMerger
            {
                FilesJoiner = this.filesJoiner
            };
        }

        [Fact]
        public void UpdatesMerger_CorrectArgs_ItMustCallUpdatedFilesJoiner()
        {
            FileModel addedFile1 = new FileModel();
            FileModel addedFile2 = new FileModel();
            FileModel removedFile1 = new FileModel();
            FileModel removedFile2 = new FileModel();
            FileModel updatedFile1 = new FileModel();
            FileModel updatedFile2 = new FileModel();
            FileModel renamedFile1 = new FileModel();
            FileModel renamedFile2 = new FileModel();
            FileModel movedFile1 = new FileModel();
            FileModel movedFile2 = new FileModel();

            MixedChanges changesRepo1 = new MixedChanges(
                0, 
                new FileModel[] { addedFile1 },
                new FileModel[] { removedFile1 },
                new FileModel[] { updatedFile1 },
                new FileModel[] { renamedFile1 },
                new FileModel[] { movedFile1 });

            MixedChanges changesRepo2 = new MixedChanges(
                0,
                new FileModel[] { addedFile2 },
                new FileModel[] { removedFile2 },
                new FileModel[] { updatedFile2 },
                new FileModel[] { renamedFile2 },
                new FileModel[] { movedFile2 });
            
            var changes = new List<MixedChanges> { changesRepo1, changesRepo2 };

            this.merger.Merge(changes);

            A.CallTo(() => this.filesJoiner.JoinTheSame(
                A<IReadOnlyList<FileModel>>.That.IsSameSequenceAs(new FileModel[] { addedFile1, addedFile2 })))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => this.filesJoiner.JoinTheSame(
                A<IReadOnlyList<FileModel>>.That.IsSameSequenceAs(new FileModel[] { removedFile1, removedFile2 })))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => this.filesJoiner.JoinTheSame(
                A<IReadOnlyList<FileModel>>.That.IsSameSequenceAs(new FileModel[] { updatedFile1, updatedFile2 })))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => this.filesJoiner.JoinTheSame(
                A<IReadOnlyList<FileModel>>.That.IsSameSequenceAs(new FileModel[] { renamedFile1, renamedFile2 })))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => this.filesJoiner.JoinTheSame(
                A<IReadOnlyList<FileModel>>.That.IsSameSequenceAs(new FileModel[] { movedFile1, movedFile2 })))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}