using PathOptimization;
using PathOptimization.Factories;
using PathOptimization.Registry;
using PathOptimization.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddScoped<IPathFinderFactory, PathFinderFactory>()
    .AddScoped<IWithMap<IPathValidator>, PathValidator>()
    .AddScoped<VesselMapValidator>()
    .AddScoped<PlaneMapValidator>()
    .AddScoped<IDictionary<Vehicles, IWithMap<IMapValueValidator>>>(sp =>
        new Dictionary<Vehicles, IWithMap<IMapValueValidator>>
        {
            { Vehicles.Vessel, sp.GetService<VesselMapValidator>()! },
            { Vehicles.Plane, sp.GetService<PlaneMapValidator>()! },
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
