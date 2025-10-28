using Microsoft.EntityFrameworkCore;
using ApplShopAPI.Model;
using ApplShopAPI.Services;
using System.Text.Json;
using System.Text.Json.Serialization;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure QuestPDF license to suppress console notice and comply with usage.
QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// SMTP options and app services
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddSingleton<ReceiptService>();
builder.Services.AddSingleton<EmailService>();

builder.Services.AddDbContext<AppleStoreContext>(options =>
    options.UseMySql(
        "server=localhost;user=root;password=230606;database=apple_store;",
        new MySqlServerVersion(new Version(8, 0, 21))
    ));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();