namespace StoragePoint.UnitTests
{
    using System.Collections.Generic;

    using FakeItEasy;

    using StoragePoint.Contracts.Domain.FileStorage;
    using StoragePoint.Contracts.Domain.FileStorage.Model;
    using StoragePoint.Contracts.Domain.Service;
    using StoragePoint.Domain.Exceptions;
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

        private readonly IUpdatesMerger mergerFake;

        private readonly SyncService syncService;

        public SyncServiceUnitTests()
        {
            this.repo1Fake = A.Fake<IFileRepository>();
            A.CallTo(() => this.repo1Fake.IsInitialized).Returns(false);
            this.repo2Fake = A.Fake<IFileRepository>();
            A.CallTo(() => this.repo2Fake.IsInitialized).Returns(false);
            this.repo3Fake = A.Fake<IFileRepository>();
            A.CallTo(() => this.repo3Fake.IsInitialized).Returns(false);
            this.repo4Fake = A.Fake<IFileRepository>();
            A.CallTo(() => this.repo4Fake.IsInitialized).Returns(false);

            this.repos = new IFileRepository[] { this.repo1Fake, this.repo2Fake, this.repo3Fake, this.repo4Fake };

            this.referenceRepoFake = A.Fake<IFileReferenceRepository>();
            A.CallTo(() => this.referenceRepoFake.IsInitialized).Returns(true);

            this.mergerFake = A.Fake<IUpdatesMerger>();

            this.syncService = new SyncService(this.mergerFake);
        }

        [Fact]
        public void SyncServiceEdges_ReferenceNotInitialized_ItMustThrowReferenceNotInitException()
        {
            A.CallTo(() => this.referenceRepoFake.IsInitialized).Returns(false);

            Assert.Throws<RepositoryNotInitialized>(() => this.syncService.Sync(this.repos, this.referenceRepoFake));
        }

        [Fact]
        public void SyncServiceEdges_ThereIsAtLeastEmptyRepo_ItMustThrowAllReposMustBeInitException()
        {
            A.CallTo(() => this.repo2Fake.IsInitialized).Returns(true);

            Assert.Throws<AllRepositoriesMustBeInitialized>(
                () => this.syncService.Sync(this.repos, this.referenceRepoFake));
        }

        [Fact]
        public void SyncServiceDetectUpdates_AllReposAreCorrect_ItMustCallDetectUpdatesForAllRepos()
        {
            this.syncService.Sync(this.repos, this.referenceRepoFake);

            A.CallTo(() => this.referenceRepoFake.DetectUpdates(this.repo1Fake))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => this.referenceRepoFake.DetectUpdates(this.repo2Fake))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => this.referenceRepoFake.DetectUpdates(this.repo3Fake))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => this.referenceRepoFake.DetectUpdates(this.repo4Fake))
                .MustHaveHappened(Repeated.Exactly.Once);
            Assert.Equal(4, this.repos.Length);
        }

        [Fact]
        public void SyncServiceDetectUpdates_AllReposAreCorrect_ItMustCallMergeUpdatesForAllUpdates()
        {
            IReadOnlyList<RepositoryUpdates> updates = new RepositoryUpdates[]
            {
                new RepositoryUpdates(), new RepositoryUpdates(),
                new RepositoryUpdates(), new RepositoryUpdates()
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
        public void SyncServiceSynchronization_AllReposAreCorrect_ItMustCallUpdateWithMergedUpdatesForAllRepos()
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

        [Fact]
        public void SyncServiceSynchronization_AllReposAreCorrect_ItMustCallUpdateWithMergedUpdatesForReferenceRepos()
        {
            RepositoryUpdates mergedUpdates = new RepositoryUpdates();
            A.CallTo(() => this.mergerFake.Merge(A<IReadOnlyList<RepositoryUpdates>>.Ignored)).Returns(mergedUpdates);

            this.syncService.Sync(this.repos, this.referenceRepoFake);

            A.CallTo(() => this.referenceRepoFake.Update(mergedUpdates)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void SyncServiceInitReposEdges_ReferenceNotInitialized_ItMustThrowReferenceNotInitException()
        {
            A.CallTo(() => this.referenceRepoFake.IsInitialized).Returns(false);

            Assert.Throws<RepositoryNotInitialized>(() => this.syncService.InitRepositories(this.repos, this.referenceRepoFake));
        }

        [Fact]
        public void SyncServiceInitRepos_FillEmptyRepos_ItMustCallCopyAllForAllUninitRepos()
        {
            A.CallTo(() => this.repo1Fake.IsInitialized).Returns(false);
            A.CallTo(() => this.repo2Fake.IsInitialized).Returns(true);
            A.CallTo(() => this.repo3Fake.IsInitialized).Returns(true);
            A.CallTo(() => this.repo4Fake.IsInitialized).Returns(false);

            this.syncService.InitRepositories(this.repos, this.referenceRepoFake);

            A.CallTo(() => this.repo1Fake.CopyAll(this.referenceRepoFake)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => this.repo2Fake.CopyAll(this.referenceRepoFake)).MustNotHaveHappened();
            A.CallTo(() => this.repo3Fake.CopyAll(this.referenceRepoFake)).MustNotHaveHappened();
            A.CallTo(() => this.repo4Fake.CopyAll(this.referenceRepoFake)).MustHaveHappened(Repeated.Exactly.Once);
            Assert.Equal(4, this.repos.Length);
        }

        [Fact]
        public void SyncServiceInitReferenceEdges_InitReference_ItMustCheckThatReferenceIsInitialized()
        {
            this.syncService.InitReference(this.referenceRepoFake, this.repo1Fake);

            A.CallTo(() => this.referenceRepoFake.IsInitialized).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void SyncServiceInitReference_AllKindsOfReferences_ItMustCallCopyAllOnlyForUninitialized(
                bool isInitialized, bool repeatedOnce)
        {
            A.CallTo(() => this.referenceRepoFake.IsInitialized).Returns(isInitialized);

            this.syncService.InitReference(this.referenceRepoFake, this.repo1Fake);

            A.CallTo(() => this.referenceRepoFake.CopyAll(this.repo1Fake))
                .MustHaveHappened(repeatedOnce ? Repeated.Exactly.Once : Repeated.Never);
        }
    }
}