using PathOptimization;
using PathOptimization.Factories;
using PathOptimization.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddScoped<PathFinderFactory>()
    .AddScoped<IDictionary<string, IWithMap<IMapValueValidator>>>(sp =>
        new Dictionary<string, IWithMap<IMapValueValidator>>
        {
            { "vessel", new VesselMapValueValidator()! },
            { "plane", new PlaneMapValueValidator()! },
        });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
