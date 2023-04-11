using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using CruddurSQL;
using System.Reflection.Metadata.Ecma335;
using Amazon.Runtime;
using Amazon;
using Microsoft.AspNetCore.DataProtection;
using IdentityModel.Client;
using Microsoft.Extensions.Options;
using Amazon.CognitoIdentityProvider;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

Console.Out.WriteLine("Hello There!");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Use AWS Systems Manager Parameter Store for values required, rather than appsettings.json.
//builder.Configuration.GetValue<string>("Environment");
string environment = builder.Environment.EnvironmentName.ToLower();
string aws_environment = $"/{environment}/cruddur/";
builder.Configuration.AddSystemsManager(aws_environment, 
    new Amazon.Extensions.NETCore.Setup.AWSOptions { Region = RegionEndpoint.EUWest2 });

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Audience = "api";
            options.Authority = "https://cognito-idp.your-region.amazonaws.com/your-user-pool-id";
            options.RequireHttpsMetadata = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "https://cognito-idp.your-region.amazonaws.com/your-user-pool-id",
                ValidAudience = "your-audience",
                IssuerSigningKeyResolver = (s, securityToken, identifier, parameters) =>
                {
                    var provider = new AmazonCognitoIdentityProviderClient()
                    {
                        Config = new AmazonServiceClient(
                    };
                    var result = provider.GetSigningKeyAsync(identifier).GetAwaiter().GetResult();
                    return new[] { result.Key };
                }
            };
        });

// Authority for authentication of tokens.  AWS Cognito I hope.
//builder.Services.AddAuthentication(options => { })
//    .AddJwtBearer(authenticationScheme: "token", configureOptions: options => 
//    {
//        options.Authority = "https://login.aws.com/";
//        options.Audience = "api";
//        options.MapInboundClaims = false;
//    });
//    builder.Services.AddAuthorization(options =>
//    {
//        options.AddPolicy(name: "ApiCaller", configurePolicy: policy =>
//        {
//            policy.RequireClaim("scope", new string[] { "api" });
//        });
//    });

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

builder.Services.AddScoped<UserDbContext>(x => { return new UserDbContext(""); });
builder.Services.AddScoped<ActivitiesDbContext>(x => { return new ActivitiesDbContext(""); });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var configuration = app.Configuration;
var connection_string = configuration.GetValue<string>($"postgres_connection_string");

//string connection_string = app.Configuration.GetValue<string>("postgres_connection_string");
Console.Out.WriteLine($"Postgres connection string = {connection_string}");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
