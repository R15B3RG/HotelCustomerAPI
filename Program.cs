using HotelCustomerAPI;
using HotelCustomerAPI.HotelDbContext;
using HotelCustomerAPI.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDb"));
builder.Services.AddSingleton<IMongoDbContext, MongoDbContext>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddSingleton<RabbitMqService>();
builder.Services.AddHostedService<RabbitMqConsumerService>();

var app = builder.Build();

app.MapGet("/customers", async (ICustomerRepository repo, int start, int count) =>
{
    return await repo.GetManyAsync(start, count);
});

app.Run("http://0.0.0.0:6050");
