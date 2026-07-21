using CustomerSupportPlateform.Application.ChatSessions.Features.Dtos;

namespace CustomerSupportPlateform.Application.ChatSessions.Features.Commands;

public record CreateSessionCommand(Guid? Id): IRequest<SessionDto>;

public class CreateSessionHandler(IApplicationDbContext dbContext) : IRequestHandler<CreateSessionCommand, SessionDto>
{
    public async ValueTask<SessionDto> Handle(CreateSessionCommand command, CancellationToken cancellationToken)
    {
        var session =(command.Id== null || command.Id ==Guid.Empty)
                        ? Session.CreateNew()
                        : Session.CreateNew(command.Id.Value);

        dbContext.Add(session);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new(session.Id);
    }
}
