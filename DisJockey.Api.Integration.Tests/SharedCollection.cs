namespace DisJockey.Api.Integration.Tests;

[CollectionDefinition(CollectionName, DisableParallelization = true)]
public class SharedCollection
{
    public const string CollectionName = "Shared collection";
}
