namespace StoragePoint.Domain.Common.Extensions
{
    public static class IntExtensions
    {
        public static int RotateLeft(this int value, int count = 16)
        {
            //uint valueUint = (uint)value;
            return (value << count) | (value >> (32 - count));
        }
    }
}