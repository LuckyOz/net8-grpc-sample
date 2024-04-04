
var builder = WebApplication.CreateBuilder(args);

//Config Controller
builder.Services.AddControllers();

//Config Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//Run Swagger
app.UseSwagger();
app.UseSwaggerUI();

//Run Https
app.UseHttpsRedirection();

//Run Auth
app.UseAuthorization();

//Run Controller
app.MapControllers();

app.Run();
