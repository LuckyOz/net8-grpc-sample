
using System.Reflection;
using AppsServer.Configs;
using AppsServer.Services;
using AppsServer.Models.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//Config Env
builder.Services.Configure<AppsConfig>(builder.Configuration);
AppsConfig? appConfig = builder.Configuration.Get<AppsConfig>();

//Config DB Postgres
builder.Services.AddDbContext<DataDbContext>(options => {
    options.UseNpgsql(appConfig!.PostgreSqlConnectionString!);
});

//Config Automapper
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

//Config Grpc
builder.Services.AddGrpc();

var app = builder.Build();

//Run Migration
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataDbContext>();
    db.Database.Migrate();
}

//Run Grpc Services
app.MapGrpcService<GreeterService>();
app.MapGrpcService<ProductService>();

//Run Rest Services
app.MapGet("/", () => "Grpc Server Start !!");

app.Run();
