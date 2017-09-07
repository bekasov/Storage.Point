namespace StoragePoint.UnitTests
{
    using System;
    using System.Linq;

    using FakeItEasy;
    using StoragePoint.Domain;
    using Xunit;

    public class SyncServiceUnitTests
    {
        private readonly IFileRepository sourceRepoFake;
        private readonly IFileRepository destinationRepoFake;
        private readonly IFileRepository referenceRepoFake;
        private readonly IDifferencesMerger mergerFake;

        private readonly SyncService syncService;

        public SyncServiceUnitTests()
        {
            this.sourceRepoFake = A.Fake<IFileRepository>();
            this.destinationRepoFake = A.Fake<IFileRepository>();
            this.referenceRepoFake = A.Fake<IFileRepository>();
            this.mergerFake = A.Fake<IDifferencesMerger>();

            this.syncService = new SyncService(this.mergerFake);
        } 

        [Fact]
        public void SyncService_EmptyDestinationFolder_ItMustCheckIsDestinationEmpty()
        {
            this.syncService.Sync(this.sourceRepoFake, this.destinationRepoFake, this.referenceRepoFake);

            A.CallTo(() => this.destinationRepoFake.IsRootEmpty).MustHaveHappened();
        }

        [Fact]
        public void SyncService_EmptyDestinationFolder_ItMustCallCopyAllContentAndNoReferenceNullException()
        {
            const IFileRepository NULL_REFERENCE_REPO = null;
            A.CallTo(() => this.destinationRepoFake.IsRootEmpty).Returns(true);
            
            bool exceptionWasThrow = false;
            try
            {
                this.syncService.Sync(this.sourceRepoFake, this.destinationRepoFake, NULL_REFERENCE_REPO);
            }
            catch (Exception)
            {
                exceptionWasThrow = true;
            }
            
            A.CallTo(() => this.destinationRepoFake.GetAllFrom(this.sourceRepoFake)).MustHaveHappened();
            Assert.False(exceptionWasThrow);
        }

        [Fact]
        public void SyncService_DestinationFolderExistsAndNullReference_ItMustThrowReferenceNullException()
        {
            const IFileRepository NULL_REFERENCE_REPO = null;
            A.CallTo(() => this.destinationRepoFake.IsRootEmpty).Returns(false);

            Assert.Throws<ArgumentNullException>(() => 
                this.syncService.Sync(this.sourceRepoFake, this.destinationRepoFake, NULL_REFERENCE_REPO));
        }

        [Fact]
        public void SyncService_DestinationFolderExists_ItMustCallDetectUpdatesForSourceAndDestination()
        {
            A.CallTo(() => this.destinationRepoFake.IsRootEmpty).Returns(false);

            this.syncService.Sync(this.sourceRepoFake, this.destinationRepoFake, this.referenceRepoFake);

            A.CallTo(() => this.referenceRepoFake.DetectUpdates(A<IFileRepository>.That.IsSameAs(this.sourceRepoFake)))
                .MustHaveHappened();
            A.CallTo(() => this.referenceRepoFake.DetectUpdates(A<IFileRepository>.That.IsSameAs(this.destinationRepoFake)))
                .MustHaveHappened();
        }

        [Fact]
        public void SyncService_DestinationFolderExists_ItMustCallDifferencesMerger()
        {
            A.CallTo(() => this.destinationRepoFake.IsRootEmpty).Returns(false);
            RepositoryUpdates fromSource = new RepositoryUpdates();
            A.CallTo(() => this.referenceRepoFake.DetectUpdates(this.sourceRepoFake)).Returns(fromSource);
            RepositoryUpdates fromDestination = new RepositoryUpdates();
            A.CallTo(() => this.referenceRepoFake.DetectUpdates(this.destinationRepoFake)).Returns(fromDestination);

            this.syncService.Sync(this.sourceRepoFake, this.destinationRepoFake, this.referenceRepoFake);

            A.CallTo(() => this.mergerFake.Merge(A<RepositoryUpdates[]>.That.Matches(m => 
                    m.Length == 2
                    && m.Contains(fromSource)
                    && m.Contains(fromDestination))))
                .MustHaveHappened();
        }
    }
}