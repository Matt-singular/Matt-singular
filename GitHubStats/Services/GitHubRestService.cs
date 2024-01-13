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

  public async Task GetGitHubUserStatistics() // todo - this can be optimised, but serves as a poc
  {
    // Get all of the repositories the user has contributed to (note what gets returned by this call will drastically change based on the PAT's access rights)
    var client = CreateGitHubApiClient();
    var repositoryList = await client.Repository.GetAllForCurrent().ConfigureAwait(false);

    // Get some user-specific details (as determined by the configured PAT)
    var user = await client.User.Current().ConfigureAwait(false);
    var organisation = await GetGitHubOrganisationDetailsAsync().ConfigureAwait(false);

    // Filter the repository list down to a specified few
    var groupedRepositoryList = repositoryList.GroupBy(repo => repo.Owner.Login).ToList();
    var userOrgRepositories = configuration.GetConfigValue(ConfigHelpers.Constants.UserOrganisationRepositories)?.Split(',')?.ToList() ?? throw new ArgumentNullException("user's organisation repositories have not been configured");
    var filteredRepositoryList = groupedRepositoryList
      .SelectMany(group => FilterToUserSpecifiedRepositories(group, organisation.Login, userOrgRepositories))
      .ToList();

    // Fetch the statistics for the filtered repositories
    var startDate = DateTime.UtcNow.AddHours(2).AddYears(-1);
    var endDate = DateTime.UtcNow.AddHours(2);
    var repositoryStatistics = await Task.WhenAll(filteredRepositoryList
      .Select(repo => GetRepositoryStatistics(repo, client, user.Login, startDate, endDate))
      .ToList())
      .ConfigureAwait(false);

    var totalUserCommits = repositoryStatistics.Sum(userRepoStats => userRepoStats.commits);
  }

  public List<Repository?> FilterToUserSpecifiedRepositories(IGrouping<string, Repository?> group, string organisation, List<string> userSpecifiedOrganisationRepos)
  {
    // Non-organisation repository
    if (group.Key != organisation)
    {
      // Short-circuit and return the unfilitred repositories
      return [.. group];
    }

    // For ogranisation repositories only, filter to a specific list
    var filteredRepositories = group
      .Where(orgRepo => userSpecifiedOrganisationRepos.Any(userSpecifiedOrgRepo => orgRepo.Name.Contains(userSpecifiedOrgRepo)))
      .ToList();
    return filteredRepositories;
  }

  public static async Task<(string? repoName, int commits)> GetRepositoryStatistics(Repository repository, GitHubClient client, string username,
    DateTime? startDate = null, DateTime? endDate = null)
  {
    // Get the user's repoository statistics
    var request = new CommitRequest
    {
      Since = startDate,//DateTime.UtcNow.AddHours(2).AddYears(-1),
      Until = endDate,//
      Author = username
    };

    // TODO - see if we can pull other statistics for the repository (comments, issues, prs)
    var repoCommitStats = await client.Repository.Commit.GetAll(repository.Id, request).ConfigureAwait(false);
    var commits = repoCommitStats?.Count ?? 0;

    return (repository.Name, commits);
  }

  public async Task GetGitHubDiffForRange()
  {
    var client = CreateGitHubApiClient();
    var organisation = await GetGitHubOrganisationDetailsAsync().ConfigureAwait(false);

    // POC
    var repoName = string.Empty; // todo - add the repo name here
    var startRef = string.Empty; // todo - add the start branch/tag here
    var endRef = string.Empty; // todo - add the end branch/tag here
    var response = await client.Repository.Commit.Compare(owner: organisation.Login, repoName, @base: startRef, head: endRef).ConfigureAwait(false);
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