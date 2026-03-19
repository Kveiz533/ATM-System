using BankApi.Application;
using BankApi.Infrastructure.Persistence;
using BankApi.Presentation.Http;
using FluentMigrator.Runner;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInfrastructurePersistence(builder.Configuration)
    .AddApplication()
    .AddPresentationHttp();

builder.Services.AddSwaggerGen().AddEndpointsApiExplorer();

WebApplication app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    IMigrationRunner runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    runner.MigrateUp();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UsePresentationHttp();

await app.RunAsync();