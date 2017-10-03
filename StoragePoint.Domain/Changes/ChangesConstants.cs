namespace StoragePoint.Domain.Changes
{
    using System.Collections.Generic;

    using StoragePoint.Contracts.Domain.Changes.Model;

    public enum DetectedChange
    {
        ADDED,
        REMOVED,
        UPDATED,
        RENAMED,
        MOVED
    }

    public class ChangesConstants
    {
        public static readonly IDictionary<IList<DetectedChange>, FileChange> MIXED_CHANGES_MAP 
            = new Dictionary<IList<DetectedChange>, FileChange>
        {
            { new List<DetectedChange> { DetectedChange.UPDATED },
                    FileChange.UPDATED },
            { new List<DetectedChange> { DetectedChange.RENAMED },
                    FileChange.RENAMED },
            { new List<DetectedChange> { DetectedChange.MOVED },
                    FileChange.MOVED },
            { new List<DetectedChange> { DetectedChange.RENAMED, DetectedChange.UPDATED },
                    FileChange.RENAMED_UPDATED },
            { new List<DetectedChange> { DetectedChange.MOVED, DetectedChange.UPDATED },
                    FileChange.MOVED_UPDATED },
            { new List<DetectedChange> { DetectedChange.RENAMED, DetectedChange.MOVED },
                    FileChange.RENAMED_MOVED },
            { new List<DetectedChange> { DetectedChange.RENAMED, DetectedChange.MOVED, DetectedChange.UPDATED },
                    FileChange.RENAMED_MOVED_UPDATED }
        };
    }
}