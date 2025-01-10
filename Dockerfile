FROM mcr.microsoft.com/dotnet/sdk:9.0@sha256:3fcf6f1e809c0553f9feb222369f58749af314af6f063f389cbd2f913b4ad556 AS build
WORKDIR /

RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build
RUN dotnet build

RUN dotnet ef migrations script --startup-project=src/Api --project=src/Data --idempotent -o init.sql

# Build runtime image
FROM postgres:17.2-alpine3.21@sha256:17143ad87797f511036cf8f50ada164aeb371f0d8068a172510549fb5d2cd65f AS datbase
WORKDIR /
COPY --from=build /init.sql /docker-entrypoint-initdb.d/init.sql
CMD [ "postgres" ]
