using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace ListBuilder.Model;

public record BSDGalleryInfo
{
	public string? Name { get; init; }
	public string? Description { get; init; }
	public string? BattleScribeVersion { get; init; }
	public string? DiscordUrl { get; init; }
	public string? TwitterUrl { get; init; }
	public string? FacebookUrl { get; init; }
	public string? RespositorySourceUrl { get; init; }
	public string? FeedUrl { get; init; }
	public string? GithubUrl { get; init; }
	public string? WebsiteUrl { get; init; }
	public ImmutableList<BSDRepositoryInfo> Repositories { get; init; } = ImmutableList<BSDRepositoryInfo>.Empty;
}

public record BSDRepositoryInfo
{
	public string? Name { get; init; }
	public string? Description { get; init; }
	public string? BattleScribeVersion { get; init; }
	public string? Version { get; init; }
	public string? LastUpdated { get; init; }
	public string? LastUpdateDescription { get; init; }
	public string? IndexUrl { get; init; }
	public string? RepositoryUrl { get; init; }
	public string? RepositoryGzipUrl { get; init; }
	public string? RepositoryBsrUrl { get; init; }
	public string? GithubUrl { get; init; }
	public string? BugTrackerUrl { get; init; }
	public string? ReportBugUrl { get; init; }
	public bool? Archived { get; init; }
	public ImmutableList<BSDFileInfo> RepositoryFiles { get; init; } = ImmutableList<BSDFileInfo>.Empty;
}

public record BSDFileInfo
{
	public string? Id { get; init; }
	public string? Name { get; init; }
	public string? Type { get; init; }
	public int? Revision { get; init; }
	public string? BattleScribeVersion { get; init; }
	public string? FileUrl { get; init; }
	public string? GithubUrl { get; init; }
	public string? BugTrackerUrl { get; init; }
	public string? ReportBugUrl { get; init; }
	public string? AuthorName { get; init; }
	public string? AutherContact { get; init; }
	public string? AuthorUrl { get; init; }
	public string? SourceSha256 { get; init; }
}

[JsonSerializable(typeof(ImmutableList<string>))]
internal sealed partial class BSDContext : JsonSerializerContext
{
}
