using System.Net;
using Amazon.Lambda.Core;
using Amazon.Extensions.Configuration.SystemsManager;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using CruddurSQL;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SignupLambda;

public class Functions
{

    AmazonSimpleSystemsManagementClient ssm_client;

    /// <summary>
    /// Default constructor that Lambda will invoke.
    /// </summary>
    public Functions()
    {
        ssm_client = new AmazonSimpleSystemsManagementClient();

        //var configurations = new ConfigurationBuilder()
        //                .AddSystemsManager("/cruddur/")
        //                .AddAppConfigUsingLambdaExtension("AppConfigApplicationId", "AppConfigEnvironmentId", "AppConfigConfigurationProfileId")
        //                .Build();
        //Connection_String = configurations["postgres-connection-string"];
    }


    /// <summary>
    /// A lambda to respond to the sign up - post confirmation - stage in cognito.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Json object?</returns>
    //public async Task<JsonElement> FunctionHandler(JsonElement input, ILambdaContext context)
    public JsonElement FunctionHandler(JsonElement input, ILambdaContext context)
    {
        string? ConnectionHost = Environment.GetEnvironmentVariable("ConnectionHost");
        string? ConnectionDB = Environment.GetEnvironmentVariable("ConnectionDB");
        string? ConnectionUsername = Environment.GetEnvironmentVariable("ConnectionUsername");
        string? ConnectionPassword = Environment.GetEnvironmentVariable("ConnectionPassword");

        string Connection_String = $"Host={ConnectionHost};Database={ConnectionDB};Username={ConnectionUsername};Password={ConnectionPassword}";    
        
        context.Logger.LogInformation("let's at least see this!");

        var request = input.GetProperty("request");
        var userAttributes = request.GetProperty("userAttributes");
        string? email = userAttributes.GetProperty("email").GetString();
        string? user_cognito_id = userAttributes.GetProperty("sub").GetString();
        string? display_name = userAttributes.GetProperty("name").GetString();
        string? handle = userAttributes.GetProperty("preferred_username").GetString();

        context.Logger.LogInformation("We've got something here!");
        context.Logger.LogInformation(userAttributes.ToString());
        context.Logger.LogInformation($"{email} {display_name} {handle} {user_cognito_id}");
        if (Connection_String is null) {
            throw new Exception("no connection string found.");
        }

        context.Logger.LogInformation($"{Connection_String}");

        using (UserDbContext db = new UserDbContext(Connection_String)) {
            UserModel um = new UserModel() {
                email = email, 
                cognito_user_id = user_cognito_id, 
                display_name = display_name, 
                handle = handle
            };

            if (db.users != null) {
                var check = db.users.Where(x => x.cognito_user_id == user_cognito_id);
                if (check.Count() == 0) {
                    db.users.Add(um);

                    var model = db.users;
                    var entityType = model.EntityType;
                    var tableNameAnnotation = entityType.GetAnnotation("Relational:TableName");
                    if (tableNameAnnotation != null) {
                        string tableName = "";
                        if (tableNameAnnotation.Value != null) {
                            var tableRep = tableNameAnnotation.Value;
                            tableName = tableRep.ToString();
                        }
                        context.Logger.LogInformation($"Table name is {tableName}");
                    }

                    db.SaveChanges();
                    Console.WriteLine($"New uuid is {um.uuid}");
                }
                else {
                    context.Logger.LogInformation($"We have already {check.Count()} entries for that cognito user id.");
                }
            }
        }
        context.Logger.LogInformation("Leaving now - bye!");
        return input;
    }
}