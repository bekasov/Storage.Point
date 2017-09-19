namespace StoragePoint.UnitTests
{
    using System.Collections.Generic;

    using FakeItEasy;

    using StoragePoint.Contracts.Domain.FileStorage.Model;
    using StoragePoint.Contracts.Infrastructure.Service;
    using StoragePoint.Domain.Service;

    using Xunit;

    public class UpdatesMergerUnitTests
    {
        private readonly IPathBuilder pathBuilderFake;

        private readonly UpdatesMerger merger;

        public UpdatesMergerUnitTests()
        {
            this.pathBuilderFake = A.Fake<IPathBuilder>();

            this.merger = new UpdatesMerger(this.pathBuilderFake);
        }

        [Fact]
        public void UpdatesMergerGetPaths_CorrectArgs_ItMustCallPathBuilder()
        {
            var updatesRepo1 = new RepositoryUpdates
            {
                Added = new FileModel[0],
                Changed = new FileModel[0],
                Moved = new FileModel[0],
                Removed = new FileModel[0],
                Renamed = new FileModel[0]
            };

            var updatesRepo2 = new RepositoryUpdates
            {
                Added = new FileModel[0],
                Changed = new FileModel[0],
                Moved = new FileModel[0],
                Removed = new FileModel[0],
                Renamed = new FileModel[0]
            };

            var changes = new List<RepositoryUpdates> { updatesRepo1, updatesRepo2 };

            this.merger.Merge(changes);

            A.CallTo(() => this.pathBuilderFake.GetPaths(updatesRepo1.Added)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => this.pathBuilderFake.GetPaths(updatesRepo1.Changed)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => this.pathBuilderFake.GetPaths(updatesRepo1.Moved)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => this.pathBuilderFake.GetPaths(updatesRepo1.Removed)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => this.pathBuilderFake.GetPaths(updatesRepo1.Renamed)).MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => this.pathBuilderFake.GetPaths(updatesRepo2.Added)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => this.pathBuilderFake.GetPaths(updatesRepo2.Changed)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => this.pathBuilderFake.GetPaths(updatesRepo2.Moved)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => this.pathBuilderFake.GetPaths(updatesRepo2.Removed)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => this.pathBuilderFake.GetPaths(updatesRepo2.Renamed)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}