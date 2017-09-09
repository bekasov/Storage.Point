namespace StoragePoint.UnitTests
{
    using System.Collections.Generic;

    using FakeItEasy;
    using StoragePoint.Domain;
    using StoragePoint.Domain.Exceptions;
    using StoragePoint.Domain.Model;
    using StoragePoint.Domain.Repository;
    using StoragePoint.Domain.Service;

    using Xunit;

    public class SyncServiceUnitTests
    {
        private readonly IFileRepository repo1Fake;
        private readonly IFileRepository repo2Fake;
        private readonly IFileRepository repo3Fake;
        private readonly IFileRepository repo4Fake;
        private readonly IFileRepository[] repos;
        private readonly IFileReferenceRepository referenceRepoFake;
        private readonly IDifferencesMerger mergerFake;

        private readonly SyncService syncService;

        public SyncServiceUnitTests()
        {
            this.repo1Fake = A.Fake<IFileRepository>();
            A.CallTo(() => this.repo1Fake.IsEmpty).Returns(false);
            this.repo2Fake = A.Fake<IFileRepository>();
            A.CallTo(() => this.repo2Fake.IsEmpty).Returns(false);
            this.repo3Fake = A.Fake<IFileRepository>();
            A.CallTo(() => this.repo3Fake.IsEmpty).Returns(false);
            this.repo4Fake = A.Fake<IFileRepository>();
            A.CallTo(() => this.repo4Fake.IsEmpty).Returns(false);

            this.repos = new IFileRepository[] { this.repo1Fake, this.repo2Fake, this.repo3Fake, this.repo4Fake };

            this.referenceRepoFake = A.Fake<IFileReferenceRepository>();
            A.CallTo(() => this.referenceRepoFake.IsInitialized).Returns(true);

            this.mergerFake = A.Fake<IDifferencesMerger>();

            this.syncService = new SyncService(this.mergerFake);
        }

        [Fact]
        public void SyncServiceEdges_ReferenceNotInitialized_ItMustThrowReferenceNotInitException()
        {
            A.CallTo(() => this.referenceRepoFake.IsInitialized).Returns(false);

            Assert.Throws<ReferenceNotInitialized>(() => this.syncService.Sync(this.repos, this.referenceRepoFake));
        }

        [Fact]
        public void SyncServiceEdges_ThereIsAtLeastEmptyRepo_ItMustThrowAllReposMustBeInitException()
        {
            A.CallTo(() => this.repo2Fake.IsEmpty).Returns(true);

            Assert.Throws<AllRepositoriesMustBeInitialized>(() => this.syncService.Sync(this.repos, this.referenceRepoFake));
        }

        [Fact]
        public void SyncServiceDetectUpdates_AllReposAreCorrect_ItMustCallDetectUpdatesForAllRepos()
        {
            this.syncService.Sync(this.repos, this.referenceRepoFake);

            A.CallTo(() => this.referenceRepoFake.DetectUpdates(this.repo1Fake)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => this.referenceRepoFake.DetectUpdates(this.repo2Fake)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => this.referenceRepoFake.DetectUpdates(this.repo3Fake)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => this.referenceRepoFake.DetectUpdates(this.repo4Fake)).MustHaveHappened(Repeated.Exactly.Once);
            Assert.Equal(4, this.repos.Length);
        }

        [Fact]
        public void SyncServiceDetectUpdates_AllReposAreCorrect_ItMustCallMergeUpdatesForAllUpdates()
        {
            IReadOnlyList<RepositoryUpdates> updates = new RepositoryUpdates[]
            {
                new RepositoryUpdates(),
                new RepositoryUpdates(),
                new RepositoryUpdates(),
                new RepositoryUpdates()
            };
            A.CallTo(() => this.referenceRepoFake.DetectUpdates(this.repo1Fake)).Returns(updates[0]);
            A.CallTo(() => this.referenceRepoFake.DetectUpdates(this.repo2Fake)).Returns(updates[1]);
            A.CallTo(() => this.referenceRepoFake.DetectUpdates(this.repo3Fake)).Returns(updates[2]);
            A.CallTo(() => this.referenceRepoFake.DetectUpdates(this.repo4Fake)).Returns(updates[3]);


            this.syncService.Sync(this.repos, this.referenceRepoFake);

            A.CallTo(() => this.mergerFake.Merge(A<IReadOnlyList<RepositoryUpdates>>.That.IsSameSequenceAs(updates)))
                .MustHaveHappened(Repeated.Exactly.Once);
            Assert.Equal(4, this.repos.Length);
        }

        [Fact]
        public void SyncServiceDetectUpdates_AllReposAreCorrect_ItMustCallUpdateWithMergedUpdatesForAllRepos()
        {
            RepositoryUpdates mergedUpdates = new RepositoryUpdates();
            A.CallTo(() => this.mergerFake.Merge(A<IReadOnlyList<RepositoryUpdates>>.Ignored)).Returns(mergedUpdates);

            this.syncService.Sync(this.repos, this.referenceRepoFake);

            A.CallTo(() => this.repo1Fake.Update(mergedUpdates)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => this.repo2Fake.Update(mergedUpdates)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => this.repo3Fake.Update(mergedUpdates)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => this.repo4Fake.Update(mergedUpdates)).MustHaveHappened(Repeated.Exactly.Once);
            Assert.Equal(4, this.repos.Length);
        }
    }
}