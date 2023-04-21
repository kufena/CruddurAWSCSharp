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

var _user_pool_id = System.Environment.GetEnvironmentVariable("AWS_COGNITO_USER_POOL_ID");
var _client_id = System.Environment.GetEnvironmentVariable("AWS_COGNITO_USER_POOL_CLIENT_ID");
var _default_region = System.Environment.GetEnvironmentVariable("AWS_DEFAULT_REGION");

if (_user_pool_id == null || _client_id == null || _default_region == null)
{
    Console.WriteLine($"Lack of Environment variables: UserPoolId = {_user_pool_id}, ClientId = {_client_id}, DefaultRegion = {_default_region}");
    throw new Exception("Null values for one of the essential three environment variables.");
}

Console.Out.WriteLine("Hello There!");
Console.Out.WriteLine($"UserPoolId = {_user_pool_id}, ClientId = {_client_id}, DefaultRegion = {_default_region}");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Use AWS Systems Manager Parameter Store for values required, rather than appsettings.json.
//builder.Configuration.GetValue<string>("Environment");
string environment = builder.Environment.EnvironmentName.ToLower();
string aws_environment = $"/{environment}/cruddur/";

string cognito_authority = $"https://cognito-idp.{_default_region}.amazonaws.com/{_user_pool_id}";
builder.Configuration.AddSystemsManager(aws_environment, 
    new Amazon.Extensions.NETCore.Setup.AWSOptions { Region = RegionEndpoint.EUWest2 });

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Audience = _client_id; // this is the client id.
            options.Authority = cognito_authority;
            options.RequireHttpsMetadata = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = cognito_authority,
                ValidAudience = _client_id,
                //IssuerSigningKeyResolver = (s, securityToken, identifier, parameters) =>
                //{
                //    var provider = new AmazonCognitoIdentityProviderClient()
                //    {
                //        Config = new ClientOptions()
                //        {
                //             ClientId = ""
                //        }
                //    };
                //    var result = provider.GetSigningKeyAsync(identifier).GetAwaiter().GetResult();
                //    return new[] { result.Key };
                //}
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

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyAllowedOrigins",
        policy =>
        {
            policy.WithOrigins("*") // note the port is included 
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

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

//app.UseHttpsRedirection();

app.UseCors("MyAllowedOrigins");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
