using FakeItEasy;
using StoragePoint.Domain;
using Xunit;

namespace StoragePoint.UnitTests
{
    public class SyncServiceUnitTests
    {
        private readonly IFileRepository sourceRepoFake;
        private readonly IFileRepository destinationRepoFake;
        private readonly SyncService syncService;

        public SyncServiceUnitTests()
        {
            this.sourceRepoFake = A.Fake<IFileRepository>();
            this.destinationRepoFake = A.Fake<IFileRepository>();

            this.syncService = new SyncService();
        }

        [Fact]
        public void SyncService_EmptyDestinationFolder_ItMustCheckDestinationIfEmpty()
        {
            this.syncService.Sync(this.sourceRepoFake, this.destinationRepoFake);

            A.CallTo(() => this.destinationRepoFake.IsRootEmpty).MustHaveHappened();
        }

        [Fact]
        public void SyncService_EmptyDestinationFolder_ItMustCopyAllContent()
        {
            A.CallTo(() => this.destinationRepoFake.IsRootEmpty).Returns(true);

            this.syncService.Sync(this.sourceRepoFake, this.destinationRepoFake);
            
            A.CallTo(() => this.destinationRepoFake.GetAllFrom(this.sourceRepoFake)).MustHaveHappened();
        }
    }
}