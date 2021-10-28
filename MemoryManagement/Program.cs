
#region Pinning through GC

DoSomethingWithPinnedMemory();

unsafe static void DoSomethingWithPinnedMemory()
{
    // create on LOH - Large Object Heap
    var pinnedArray = GC.AllocateArray<byte>(128, pinned: true);
    fixed (byte* ptr = pinnedArray)
    {
        // This is no-op pinning as it does not influence the GC compaction
    }
}

#endregion

#region Through MemoryPool

using (var rental = MemoryPool<byte>.Shared.Rent(1024))
{
    DoSomething(rental.Memory);
}

static void DoSomething(Memory<byte> scratchBuffer)
{
    var pinnedHandle = scratchBuffer.Pin();
}

#endregion

#region StackAllocation

Span<byte> stackBuffer = stackalloc byte[1024];

#endregion