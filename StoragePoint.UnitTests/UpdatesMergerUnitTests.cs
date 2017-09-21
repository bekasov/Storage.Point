namespace StoragePoint.UnitTests
{
    using System;
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

//        [Fact]
//        public void UpdatesMergerGetPaths_CorrectArgs_ItMustCallPathBuilder()
//        {
//            var updatesRepo1 = this.CreateEmptyUpdates();
//            var updatesRepo2 = this.CreateEmptyUpdates();
//            var changes = new List<StorageUpdates> { updatesRepo1, updatesRepo2 };
//
//            this.merger.Merge(changes);
//
//            string s1 = "Doc1.docx", s2 = "doc1.docx", s3 = "", s4 = "1", s5 = "11";
//
//            int i1 = s1.GetHashCode(StringComparison.CurrentCulture); // -2035784446
//            int i2 = s2.GetHashCode(StringComparison.CurrentCulture); // -1789490680
//            int i3 = s3.GetHashCode(StringComparison.CurrentCulture); // 0
//            int i4 = s4.GetHashCode(StringComparison.CurrentCulture); // -822360972
//            int i5 = s5.GetHashCode(StringComparison.CurrentCulture); // 1954045428
//
//            A.CallTo(() => this.pathBuilderFake.GetPaths(updatesRepo1.Added)).MustHaveHappened(Repeated.Exactly.Once);
//            A.CallTo(() => this.pathBuilderFake.GetPaths(updatesRepo1.Changed)).MustHaveHappened(Repeated.Exactly.Once);
//            A.CallTo(() => this.pathBuilderFake.GetPaths(updatesRepo1.Moved)).MustHaveHappened(Repeated.Exactly.Once);
//            A.CallTo(() => this.pathBuilderFake.GetPaths(updatesRepo1.Removed)).MustHaveHappened(Repeated.Exactly.Once);
//            A.CallTo(() => this.pathBuilderFake.GetPaths(updatesRepo1.Renamed)).MustHaveHappened(Repeated.Exactly.Once);
//
//            A.CallTo(() => this.pathBuilderFake.GetPaths(updatesRepo2.Added)).MustHaveHappened(Repeated.Exactly.Once);
//            A.CallTo(() => this.pathBuilderFake.GetPaths(updatesRepo2.Changed)).MustHaveHappened(Repeated.Exactly.Once);
//            A.CallTo(() => this.pathBuilderFake.GetPaths(updatesRepo2.Moved)).MustHaveHappened(Repeated.Exactly.Once);
//            A.CallTo(() => this.pathBuilderFake.GetPaths(updatesRepo2.Removed)).MustHaveHappened(Repeated.Exactly.Once);
//            A.CallTo(() => this.pathBuilderFake.GetPaths(updatesRepo2.Renamed)).MustHaveHappened(Repeated.Exactly.Once);
//        }

        private StorageUpdates CreateEmptyUpdates()
        {
            return new StorageUpdates(0, new FileModel[0], new FileModel[0], new FileModel[0], new FileModel[0], new FileModel[0]);
        }
    }
}