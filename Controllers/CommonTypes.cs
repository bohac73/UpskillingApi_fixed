namespace UpskillingApi.Controllers;

public record LinkDto(string Rel, string Href, string Method);

public record PagedResult<T>(IEnumerable<T> Items, int PageNumber, int PageSize, int TotalCount, int TotalPages, IEnumerable<LinkDto> Links);