namespace StoragePoint.UnitTests.Tests.Service
{
    using System.Collections.Generic;

    using FakeItEasy;

    using StoragePoint.Contracts.Domain.Changes.Model;
    using StoragePoint.Contracts.Domain.Changes.Service;
    using StoragePoint.Contracts.Domain.Exceptions;
    using StoragePoint.Contracts.Domain.FileStorage;
    using StoragePoint.Domain.Service;

    using Xunit;

    public class SyncServiceUnitTests
    {
        private readonly IFileStorage repo1Fake;
        private readonly IFileStorage repo2Fake;
        private readonly IFileStorage repo3Fake;
        private readonly IFileStorage repo4Fake;
        private readonly IFileStorage[] repos;

        private readonly IFileReferenceStorage referenceRepoFake;

        private readonly IMixedChangesSplitter splitterFake;

        private readonly SyncService syncService;

        public SyncServiceUnitTests()
        {
            this.repo1Fake = A.Fake<IFileStorage>();
            A.CallTo<bool>(() => this.repo1Fake.IsInitialized).Returns(false);
            this.repo2Fake = A.Fake<IFileStorage>();
            A.CallTo<bool>(() => this.repo2Fake.IsInitialized).Returns(false);
            this.repo3Fake = A.Fake<IFileStorage>();
            A.CallTo<bool>(() => this.repo3Fake.IsInitialized).Returns(false);
            this.repo4Fake = A.Fake<IFileStorage>();
            A.CallTo<bool>(() => this.repo4Fake.IsInitialized).Returns(false);

            this.repos = new IFileStorage[] { this.repo1Fake, this.repo2Fake, this.repo3Fake, this.repo4Fake };

            this.referenceRepoFake = A.Fake<IFileReferenceStorage>();
            A.CallTo<bool>(() => this.referenceRepoFake.IsInitialized).Returns(true);

            this.splitterFake = A.Fake<IMixedChangesSplitter>();

            this.syncService = new SyncService(this.splitterFake);
        }

        [Fact]
        public void SyncServiceEdges_ReferenceNotInitialized_ItMustThrowReferenceNotInitException()
        {
            A.CallTo<bool>(() => this.referenceRepoFake.IsInitialized).Returns(false);

            Assert.Throws<RepositoryNotInitialized>(() => this.syncService.Sync(this.repos, this.referenceRepoFake));
        }

        [Fact]
        public void SyncServiceEdges_ThereIsAtLeastEmptyRepo_ItMustThrowAllReposMustBeInitException()
        {
            A.CallTo<bool>(() => this.repo2Fake.IsInitialized).Returns(true);

            Assert.Throws<AllRepositoriesMustBeInitialized>(
                () => this.syncService.Sync(this.repos, this.referenceRepoFake));
        }

        [Fact]
        public void SyncServiceDetectUpdates_AllReposAreCorrect_ItMustCallDetectUpdatesForAllRepos()
        {
            this.syncService.Sync(this.repos, this.referenceRepoFake);

            A.CallTo<MixedChanges>(() => this.referenceRepoFake.DetectUpdates(this.repo1Fake))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo<MixedChanges>(() => this.referenceRepoFake.DetectUpdates(this.repo2Fake))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo<MixedChanges>(() => this.referenceRepoFake.DetectUpdates(this.repo3Fake))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo<MixedChanges>(() => this.referenceRepoFake.DetectUpdates(this.repo4Fake))
                .MustHaveHappened(Repeated.Exactly.Once);
            Assert.Equal(4, this.repos.Length);
        }

        [Fact]
        public void SyncServiceDetectUpdates_AllReposAreCorrect_ItMustCallMergeUpdatesForAllUpdates()
        {
            IReadOnlyList<MixedChanges> updates = new MixedChanges[]
            {
                this.CreateEmptyUpdates(), this.CreateEmptyUpdates(),
                this.CreateEmptyUpdates(), this.CreateEmptyUpdates()
            };
            A.CallTo<MixedChanges>(() => this.referenceRepoFake.DetectUpdates(this.repo1Fake)).Returns(updates[0]);
            A.CallTo<MixedChanges>(() => this.referenceRepoFake.DetectUpdates(this.repo2Fake)).Returns(updates[1]);
            A.CallTo<MixedChanges>(() => this.referenceRepoFake.DetectUpdates(this.repo3Fake)).Returns(updates[2]);
            A.CallTo<MixedChanges>(() => this.referenceRepoFake.DetectUpdates(this.repo4Fake)).Returns(updates[3]);


            this.syncService.Sync(this.repos, this.referenceRepoFake);

            A.CallTo<IReadOnlyList<ChangedFile>>(() => this.splitterFake.Split(A<MixedChanges>.That.IsSameAs(updates[0])))
                .MustHaveHappened(Repeated.Exactly.Once);
            Assert.Equal(4, this.repos.Length);
        }

        [Fact]
        public void SyncServiceSynchronization_AllReposAreCorrect_ItMustCallUpdateWithMergedUpdatesForAllRepos()
        {
            IReadOnlyList<ChangedFile> mergedChanges = new List<ChangedFile>();
            A.CallTo<IReadOnlyList<ChangedFile>>(() => this.splitterFake.Split(A<MixedChanges>.Ignored)).Returns(mergedChanges);

            this.syncService.Sync(this.repos, this.referenceRepoFake);

            A.CallTo(() => this.repo1Fake.Update(mergedChanges)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => this.repo2Fake.Update(mergedChanges)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => this.repo3Fake.Update(mergedChanges)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => this.repo4Fake.Update(mergedChanges)).MustHaveHappened(Repeated.Exactly.Once);
            Assert.Equal(4, this.repos.Length);
        }

        [Fact]
        public void SyncServiceSynchronization_AllReposAreCorrect_ItMustCallUpdateWithMergedUpdatesForReferenceRepos()
        {
            IReadOnlyList<ChangedFile> mergedChanges = new List<ChangedFile>();
            A.CallTo<IReadOnlyList<ChangedFile>>(() => this.splitterFake.Split(A<MixedChanges>.Ignored)).Returns(mergedChanges);

            this.syncService.Sync(this.repos, this.referenceRepoFake);

            A.CallTo(() => this.referenceRepoFake.Update(mergedChanges)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void SyncServiceInitReposEdges_ReferenceNotInitialized_ItMustThrowReferenceNotInitException()
        {
            A.CallTo<bool>(() => this.referenceRepoFake.IsInitialized).Returns(false);

            Assert.Throws<RepositoryNotInitialized>(() => this.syncService.InitRepositories(this.repos, this.referenceRepoFake));
        }

        [Fact]
        public void SyncServiceInitRepos_FillEmptyRepos_ItMustCallCopyAllForAllUninitRepos()
        {
            A.CallTo<bool>(() => this.repo1Fake.IsInitialized).Returns(false);
            A.CallTo<bool>(() => this.repo2Fake.IsInitialized).Returns(true);
            A.CallTo<bool>(() => this.repo3Fake.IsInitialized).Returns(true);
            A.CallTo<bool>(() => this.repo4Fake.IsInitialized).Returns(false);

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

            A.CallTo<bool>(() => this.referenceRepoFake.IsInitialized).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void SyncServiceInitReference_AllKindsOfReferences_ItMustCallCopyAllOnlyForUninitialized(
                bool isInitialized, bool repeatedOnce)
        {
            A.CallTo<bool>(() => this.referenceRepoFake.IsInitialized).Returns(isInitialized);

            this.syncService.InitReference(this.referenceRepoFake, this.repo1Fake);

            A.CallTo(() => this.referenceRepoFake.CopyAll(this.repo1Fake))
                .MustHaveHappened(repeatedOnce ? Repeated.Exactly.Once : Repeated.Never);
        }

        private MixedChanges CreateEmptyUpdates()
        {
            return new MixedChanges(null, null, null, null, null);
        }
    }
}