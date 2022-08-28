using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

public class DontForceGcCollectionsConfig : ManualConfig
{
    public DontForceGcCollectionsConfig()
    {
        AddJob(Job.Default
            .WithGcMode(new GcMode()
            {
                Force = false // tell BenchmarkDotNet not to force GC collections after every iteration
            }));
    }
}