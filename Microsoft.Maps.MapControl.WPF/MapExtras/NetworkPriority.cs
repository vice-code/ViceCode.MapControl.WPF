namespace Microsoft.Maps.MapExtras
{
    internal enum NetworkPriority
    {
        Idle = -2147483648, // 0x80000000
        Low = -1000, // 0xFFFFFC18
        Normal = 0,
        High = 1000, // 0x000003E8
    }
}
