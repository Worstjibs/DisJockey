using Microsoft.Extensions.DependencyInjection;

namespace DisJockey.Application.Contracts;

public interface IMediator
{
    Task<TResponse> SendAsync<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken = default)
            where TRequest : IRequest<TResponse>;
}

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> SendAsync<TRequest, TResponse>(
        TRequest request, 
        CancellationToken cancellationToken = default)
            where TRequest : IRequest<TResponse>
    {
        var service = _serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();

        return await service.HandleAsync(request, cancellationToken);
    }
}
