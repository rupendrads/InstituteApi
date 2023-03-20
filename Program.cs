using Microsoft.EntityFrameworkCore;
using InstituteApi.Models;
using System.Text.Json.Serialization;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy(name:MyAllowSpecificOrigins, 
        builder =>
    {
        builder.WithOrigins("http://localhost",
            "http://localhost:4200",
            "https://localhost:7230",
            "http://localhost:90")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});
builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
;
builder.Services.AddDbContext<InstituteContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("InstituteDB")));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod());

app.UseAuthorization();

app.MapControllers();

app.Run();
