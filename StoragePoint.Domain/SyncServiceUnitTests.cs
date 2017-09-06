using FakeItEasy;
using StoragePoint.Domain;
using Xunit;

namespace StoragePoint.UnitTests
{
    public class SyncServiceUnitTests
    {
        private readonly IFileRepository sourceRepoFake;
        private readonly IFileRepository destinationRepoFake;

        public SyncServiceUnitTests()
        {
            this.sourceRepoFake = A.Fake<IFileRepository>();
            this.destinationRepoFake = A.Fake<IFileRepository>();
        }

        [Fact]
        public void SyncService_EmptyDestinationFolder_ItMustCopyAllContent()
        {
            SyncService syncService = new SyncService();

            A.CallTo(() => this.destinationRepoFake.IsRootEmpty).Returns(true);

            syncService.Sync(this.sourceRepoFake, this.destinationRepoFake);

            A.CallTo(() => this.)

        }
    }
}