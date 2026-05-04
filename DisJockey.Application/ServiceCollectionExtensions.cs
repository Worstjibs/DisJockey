using DisJockey.Application.Contracts;
using DisJockey.Application.Features.Members.Queries.AllMembers;
using DisJockey.Application.Features.Members.Queries.GetMember;
using DisJockey.Application.Profiles;
using DisJockey.Shared.DTOs.Member;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DisJockey.Application;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplicationServicesV2()
        {
            services.AddAutoMapper(configuration =>
            {
                configuration.AddProfile<AutoMapperProfiles>();
            });

            services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.Load("DisJockey.Application")));

            services.RegisterHandlers();

            return services;
        }

        private IServiceCollection RegisterHandlers()
        {
            services.AddScoped<IMediator, Mediator>();

            services.AddScoped<IRequestHandler<AllMembersQuery, IEnumerable<MemberListDto>>, AllMembersHandler>();
            services.AddScoped<IRequestHandler<GetMemberQuery, MemberDetailDto?>, GetMemberHandler>();

            return services;
        }
    }
}
