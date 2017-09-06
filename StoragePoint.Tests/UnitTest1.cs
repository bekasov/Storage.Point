using System;
using StoragePoint.Domain;
using Xunit;

namespace StoragePoint.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void SyncService_EmptyDestinationFolder_ItMustCopyAllContent()
        {
            SyncService syncService = new SyncService();
        }
    }
}