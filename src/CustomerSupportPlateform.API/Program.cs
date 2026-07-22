using Carter;
using CustomerSupportPlateform.API.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDependencies(builder.Configuration); 

//builder.Services.AddAntiforgery();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.Title = "Customer Support Plateform API";
    });
}
//app.UseAntiforgery();
app.UseHttpsRedirection();

app.MapCarter();


app.Run();


