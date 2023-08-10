FROM ubuntu:latest AS base
ENV TZ=Etc/Utc
ENV DEBIAN_FRONTEND=noninteractive
RUN apt update
RUN apt install -y libcurl4
RUN apt install -y aspnetcore-runtime-6.0
RUN apt install -y tzdata
WORKDIR /app
ENV ASPNETCORE_URLS=http://0.0.0.0:80
ENV FFXIV_VENUES_API__HttpsOnly=False
EXPOSE 80

FROM ubuntu:latest AS build
COPY . /src
RUN apt update
RUN apt install -y libcurl4
RUN apt install -y dotnet-sdk-6.0
RUN dotnet publish /src/FFXIVVenues.Api.csproj -c Release -o /src/build

FROM base AS final
WORKDIR /app
COPY --from=build /src/build .
ENTRYPOINT ["dotnet", "FFXIVVenues.Api.dll"]
