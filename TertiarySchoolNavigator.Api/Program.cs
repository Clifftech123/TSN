using FluentValidation;
using TertiarySchoolNavigator.Api.Extensions;
using TertiarySchoolNavigator.Api.Interface;
using TertiarySchoolNavigator.Api.Middleware;
using TertiarySchoolNavigator.Api.Service;
using TertiarySchoolNavigator.Api.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.ConfigureCors();
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureIdentity();
builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.AddScoped<IAuthenticationManager, AuthenticationService>();
builder.Services.AddScoped<ISchoolService, SchoolService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<SchoolCreateRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<SchoolUpdateRequestValidator>();

// Adding of Exceptions handle 
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();
app.UseExceptionHandler();

app.MapControllers();

app.Run();
