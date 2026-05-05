namespace DisJockey.Application.Contracts;

public interface IRequest;

public interface IRequest<T>;

public interface IRequestHandler<TRequest> 
    where TRequest : IRequest
{
    Task HandleAsync(
        TRequest request, 
        CancellationToken cancellationToken = default);
}

public interface IRequestHandler<in TRequest, TResponse> 
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> HandleAsync(
        TRequest request, 
        CancellationToken cancellationToken = default);
}
