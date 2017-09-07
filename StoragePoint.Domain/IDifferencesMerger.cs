namespace StoragePoint.Domain
{
    public interface IDifferencesMerger
    {
        RepositoryUpdates Merge(RepositoryUpdates[] differences);
    }
}