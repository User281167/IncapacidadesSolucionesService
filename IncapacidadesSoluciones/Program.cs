using IncapacidadesSoluciones.Services;
using Supabase;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers().AddNewtonsoftJson();

// clients
//builder.Services.AddScoped<SupabaseClient>(_ => new SupabaseClient());
builder.Services.AddScoped<Supabase.Client>(_ =>
    new Supabase.Client(
                Environment.GetEnvironmentVariable("SUPABASE_URL"),
                Environment.GetEnvironmentVariable("SUPABASE_KEY"),
                new SupabaseOptions
                {
                    AutoRefreshToken = true,
                    AutoConnectRealtime = true,
                }
                )
);

// services
builder.Services.AddScoped<UserService>();

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
