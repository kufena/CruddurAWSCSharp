using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(options => { })
    .AddJwtBearer(authenticationScheme: "token", configureOptions: options => 
    {
        options.Authority = "https://login.aws.com/";
        options.Audience = "api";
        options.MapInboundClaims = false;
    });
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy(name: "ApiCaller", configurePolicy: policy =>
        {
            policy.RequireClaim("scope", new string[] { "api" });
        });
    });

    //.AddOpenIdConnect("oidc", options => 
    //{
    //    options.Authority = "https://awsblahbalhbalh";
    //    // confidential client using code flow + PKCE
    //    options.ClientId = "";
    //    options.ClientSecret = "";
    //    options.ResponseType = "code";
    //    options.ResponseMode = "query";
    //    // store tokens in session
    //    options.SaveTokens = true;
    //    options.Scope.Add("api");
    //    options.Scope.Add("offline_access");
    //});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseAuthentication();
    app.UseAuthorization();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
