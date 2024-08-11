using ErrorOr;
using MediatR;

namespace DisJockey.Application.Features.Tracks.Commands.BlacklistTrack;

public record BlacklistTrackCommand(int Id) : IRequest<ErrorOr<Success>>;
