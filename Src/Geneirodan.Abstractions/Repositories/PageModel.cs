using JetBrains.Annotations;

namespace Geneirodan.Abstractions.Repositories;

/// <summary>
/// Represents a single page of a paginated result set.
/// Used by query handlers and repositories to return sliced data together with page index, size, and total count
/// so that clients can build paged UI or follow next/previous links.
/// </summary>
/// <typeparam name="T">The type of items contained in the paginated result.</typeparam>
[PublicAPI]
public record PageModel<T>
{
    /// <summary>
    /// The collection of items for the current page (at most <see cref="PageSize"/> items).
    /// </summary>
    public required IEnumerable<T> Items { get; init; }

    /// <summary>
    /// The current page number (1-based). Used together with <see cref="PageSize"/> to compute skip/take for the next page.
    /// </summary>
    public required int Page { get; init; }

    /// <summary>
    /// The number of items per page. Determines the maximum size of <see cref="Items"/> for this page.
    /// </summary>
    public required int PageSize { get; init; }

    /// <summary>
    /// The total number of items across all pages. Used to compute <see cref="TotalPages"/> and to show "X of Y" in the UI.
    /// </summary>
    public required long TotalCount { get; init; }

    /// <summary>
    /// Gets a value indicating whether there is a next page of items after the current page.
    /// </summary>
    public bool HasNextPage => Page * PageSize < TotalCount;

    /// <summary>
    /// Gets a value indicating whether there is a previous page (i.e. current page is greater than 1).
    /// </summary>
    public bool HasPrevPage => Page > 1;

    /// <summary>
    /// Gets the total number of pages based on <see cref="TotalCount"/> and <see cref="PageSize"/> (ceiling division).
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}