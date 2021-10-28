using System.Buffers;

const int minimumLength = 1024;

//Using a shared *threadsafe* pool,
//default max array length => 2^20 (1024*1024 = 1 048 576)
var samePool = ArrayPool<byte>.Shared;

//Option 2: *threadsafe* as well
//var samePool = ArrayPool<byte>.Create(maxArrayLength: 1024, maxArraysPerBucket: 16);

byte[] buffer = samePool.Rent(minimumLength);

try
{
    Use(buffer);
}
finally
{
    samePool.Return(buffer);
    // don't use the reference to the buffer after returning it!
}

void Use(byte[] buffer)
{
    // do something useful
    Console.WriteLine($"Array size {buffer.Length}");
}