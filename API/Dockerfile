FROM mcr.microsoft.com/dotnet/aspnet:5.0

ENV ASPNETCORE_ENVIRONMENT="Production"
ENV ConnectionStrings:DefaultConnection="Server=host.docker.internal; Database=disjockey; User Id=sa; Password=i5EQqqn2MPXOK5kRwoxJ;"

COPY bin/Release/net5.0/publish/ App/
WORKDIR /App
ENTRYPOINT ["dotnet", "API.dll"]