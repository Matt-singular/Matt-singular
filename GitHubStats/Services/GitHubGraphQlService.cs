namespace Services;

using System;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.Extensions.Configuration;
using Services.Helpers;

public class GitHubGraphQlService(IConfiguration configuration) : IGitHubGraphQlService
{
  public async Task GetCommitStatisticsForUser() // this function contains some very basic POC work, not yet ready.
  {
    var accessToken = configuration.GetConfigValue(ConfigHelpers.Constants.AccessTokenKey);
    var targetUsername = "Matt-singular";

    // GitHub GraphQL API endpoint
    var apiUrl = new Uri("https://api.github.com/graphql");

    // Create GraphQL client
    using var graphQLClient = new GraphQLHttpClient(apiUrl, new NewtonsoftJsonSerializer());

    // Set the authorization header with the access token
    graphQLClient.HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

    // GraphQL query to retrieve repositories contributed by a specific user
    var query = new GraphQLRequest
    {
      // TODO: need to update this query, this one is not correct
      Query = @"
        query {
            user(login: """ + targetUsername + @""") {
                contributions(first: 100) {
                    edges {
                        node {
                            repository {
                                name
                                owner {
                                    login
                                }
                            }
                        }
                    }
                }
            }
        }"
    };

    // Execute the query
    var graphQlResponse = await graphQLClient.SendQueryAsync<UserContributionsResponse>(query);
    var graphQlResponseData = graphQlResponse.Data;

    // Display the results
    var contributions = graphQlResponseData.Data.User.Contributions.Edges;

    Console.WriteLine($"Repositories contributed to by {targetUsername}:");
    foreach (var edge in contributions)
    {
      var repository = edge.Node.Repository;
      Console.WriteLine($"- {repository.Owner.Login}/{repository.Name}");
    }
  }


  /// <summary>
  /// Localised GitHubGraphQlService constants
  /// </summary>
  public static class Constants
  {
  }
}

public interface IGitHubGraphQlService
{
  public Task GetCommitStatisticsForUser();
}

// Define classes to represent the response structure
public class UserContributionsResponse
{
  public UserData Data { get; set; }
}

public class UserData
{
  public User User { get; set; }
}

public class User
{
  public Contributions Contributions { get; set; }
}

public class Contributions
{
  public ContributionEdge[] Edges { get; set; }
}

public class ContributionEdge
{
  public ContributionNode Node { get; set; }
}

public class ContributionNode
{
  public RepositoryInfo Repository { get; set; }
}

public class RepositoryInfo
{
  public string Name { get; set; }
  public Owner Owner { get; set; }
}

public class Owner
{
  public string Login { get; set; }
}