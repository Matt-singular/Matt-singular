namespace Services;

using Octokit;
using Microsoft.Extensions.Configuration;
using Services.Helpers;

public class GitHubRestService(IConfiguration configuration) : IGitHubRestService
{
  public GitHubClient CreateGitHubApiClient()
  {
    // Configure the client credentials
    var token = configuration.GetConfigValue(ConfigHelpers.Constants.AccessTokenKey);
    var credentials = new Credentials(token);

    // Configure the client headers
    var applicationHeader = new ProductHeaderValue(Constants.ApplicationName);

    // Create and return the GitHub Octokit API client
    var client = new GitHubClient(applicationHeader)
    {
      Credentials = credentials
    };

    return client;
  }
  
  public async Task<Octokit.Organization> GetGitHubOrganisationDetailsAsync()
  {
    // Fetch the current user's organisations
    var client = CreateGitHubApiClient();
    var organisations = await client.Organization.GetAllForCurrent().ConfigureAwait(false);

    // Currently limited to one GitHub organisation, but this logic could be extended to cater for multiple if needed.
    var organisationDetails = organisations?.FirstOrDefault() ?? throw new ArgumentNullException("couldn't find organisation");
    return organisationDetails;
  }

  public async Task GetGitHubUserStatistics()
  {
    // Get statistics for the user's repositories
    var client = CreateGitHubApiClient();
    var userRepositories = await client.Repository.GetAllForCurrent().ConfigureAwait(false);
    var userRepoDetails = await Task.WhenAll(userRepositories
      .Select(repo => GetRepositoryStatistics(repo, client))
      .ToList())
      .ConfigureAwait(false);
    var totalUserStatistics = userRepoDetails.Sum(userRepoStats => userRepoStats.commits);

    // Get the Organisation repositories
    var organisation = await GetGitHubOrganisationDetailsAsync().ConfigureAwait(false);
    var organisationRepositories = await client.Repository.GetAllForOrg(organisation.Name).ConfigureAwait(false);

    // Filter to the configured user repositories (currently doing this manually as the REST API doesn't seem to cater for this filtering)
    var userOrgRepositories = configuration.GetConfigValue(ConfigHelpers.Constants.UserOrganisationRepositories)?.Split(',')?.ToList() ?? throw new ArgumentNullException("user's organisation repositories have not been configured");
    var userOrganisationRepos = organisationRepositories.Where(orgRepo => userOrgRepositories.Any(userOrgRepo => orgRepo.Name.Contains(userOrgRepo))).ToList();

    // Get the user's organisation repo details
    var userOrganisationRepoDetails = await Task.WhenAll(userOrganisationRepos
      .Select(repo => GetRepositoryStatistics(repo, client))
      .ToList())
      .ConfigureAwait(false);
    var totalOrganisationStatistics = userOrganisationRepoDetails.Sum(orgRepoStats => orgRepoStats.commits);

    // Totals (still very rough numbers and doesn't include all repositories contributed to)
    var totalCommitsMade = totalUserStatistics + totalOrganisationStatistics;
  }

  public static async Task<(string? repoName, int commits)> GetRepositoryStatistics(Repository repository, GitHubClient client)
  {
    // User and repository names
    var username = "Matt-singular";
    var repoName = repository.Name;

    // Get the user's organisation statistics
    var request = new CommitRequest
    {
      Since = DateTime.UtcNow.AddHours(2).AddYears(-1),
      Until = DateTime.UtcNow.AddHours(2),
      Author = username
    };
    var repoStatistics = await client.Repository.Commit.GetAll(repository.Id, request).ConfigureAwait(false);
    var commits = repoStatistics?.Count ?? 0;
    
    return (repoName, commits);
  }

  public async Task GetGitHubDiffForRange()
  {
    var client = CreateGitHubApiClient();
    var organisation = await GetGitHubOrganisationDetailsAsync().ConfigureAwait(false);

    // POC
    var repoName = string.Empty; // todo - add the repo name here
    var startRef = string.Empty; // todo - add the start branch/tag here
    var endRef = string.Empty; // todo - add the end branch/tag here
    var response = await client.Repository.Commit.Compare(owner: organisation.Name, repoName, @base: startRef, head: endRef).ConfigureAwait(false);
    var diff = response.Commits; // contains the list of diffs here
  }

  /// <summary>
  /// Localised GitHubRestService constants
  /// </summary>
  public static class Constants
  {
    public const string ApplicationName = "GitHubStats";
  }
}

/// <summary>
/// Houses the various GitHub OctoKit API services abstracted into useful
/// </summary>
public interface IGitHubRestService
{
  /// <summary>
  /// Creates an instance of the GitHub OctoKit Api Client 
  /// using a PAT for authentication and an application-identifying header
  /// </summary>
  /// <returns>An instance of the GitHub OctoKit Api Client</returns>
  public GitHubClient CreateGitHubApiClient();
  public Task GetGitHubUserStatistics(); // TODO in progress still - method signature to change
  public Task GetGitHubDiffForRange(); // Testing this to see if it is viable for pulling repo diffs
}