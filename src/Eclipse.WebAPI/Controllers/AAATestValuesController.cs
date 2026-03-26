using Eclipse.Core.Stores.Cosmos;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Eclipse.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AAATestValuesController : ControllerBase
{
    private readonly CosmosClient _cosmosClient;

    private readonly IOptions<EclipseCoreStoresCosmosOptions> _options;

    public AAATestValuesController(CosmosClient cosmosClient, IOptions<EclipseCoreStoresCosmosOptions> options)
    {
        _cosmosClient = cosmosClient;
        _options = options;
    }

    [HttpGet("pipelines")]
    public async Task<IActionResult> GetPipelines(CancellationToken cancellationToken)
    {
        var container = _cosmosClient.GetContainer(_options.Value.Database, _options.Value.Container);
        var query = new QueryDefinition("SELECT * FROM c WHERE c.Discriminator = 'PipelineInfo'");
        using var iterator = container.GetItemQueryIterator<dynamic>(query);
        List<dynamic> pipelines = [];
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(cancellationToken);
            pipelines.AddRange(response.Resource);
        }
        return Ok(pipelines);
    }

    [HttpGet("messages")]
    public async Task<IActionResult> GetMessages(CancellationToken cancellationToken)
    {
        var container = _cosmosClient.GetContainer(_options.Value.Database, _options.Value.Container);
        var query = new QueryDefinition("SELECT * FROM c WHERE c.Discriminator = 'MessageInfo'");
        using var iterator = container.GetItemQueryIterator<dynamic>(query);
        List<dynamic> messages = [];
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(cancellationToken);
            messages.AddRange(response.Resource);
        }
        return Ok(messages);
    }
}
