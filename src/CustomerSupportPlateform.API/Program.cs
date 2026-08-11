using Carter;
using CustomerSupportPlateform.API.Extensions;
using CustomerSupportPlateform.API.Middlewares;
using CustomerSupportPlateform.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDependencies(builder.Configuration);
builder.Services.AddProblemDetails();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ChatWidget", policy =>
    {
        policy.AllowAnyOrigin()
            .WithMethods("POST")
            .WithHeaders("Content-Type");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseDeveloperExceptionPage();


    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseExceptionHandler();

app.UseCors("ChatWidget");

app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("api/identity")
        .WithTags("Identity")
        .MapIdentityApi<ApplicationUser>();

app.MapCarter();


app.Run();


