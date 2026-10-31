using Shekar.Application.Abstractions;
using Shekar.Domain.Entities;
using Shekar.Domain.Enums;

namespace Shekar.Mock.Repositories;

public sealed class MockCarListingRepository : ICarListingRepository
{
    private readonly MockDataStore _store;
    public MockCarListingRepository(MockDataStore store) => _store = store;

    public Task<CarListing?> GetByExternalIdAsync(ListingSource source, string externalId, CancellationToken ct = default) =>
        Task.FromResult(_store.CarListings.FirstOrDefault(l => l.Source == source && l.ExternalId == externalId));

    public Task<bool> ExistsActiveByContentHashAsync(string contentHash, CancellationToken ct = default) =>
        Task.FromResult(_store.CarListings.Any(l => l.State == ListingState.Active && l.ContentHash == contentHash));

    public Task<IReadOnlyList<CarListing>> ListActiveAsync(ListingSource source, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<CarListing>>(
            _store.CarListings.Where(l => l.Source == source && l.State == ListingState.Active).ToList());

    public Task<CarListing> AddAsync(CarListing listing, CancellationToken ct = default)
    {
        lock (_store.Gate)
        {
            listing.Id = _store.CarListings.Select(l => l.Id).DefaultIfEmpty(0).Max() + 1;
            _store.CarListings.Add(listing);
        }
        return Task.FromResult(listing);
    }

    public Task UpdateAsync(CarListing listing, CancellationToken ct = default) => Task.CompletedTask;
}

public sealed class MockCrawlRunRepository : ICrawlRunRepository
{
    private readonly MockDataStore _store;
    public MockCrawlRunRepository(MockDataStore store) => _store = store;

    public Task<CrawlRun> AddAsync(CrawlRun run, CancellationToken ct = default)
    {
        lock (_store.Gate)
        {
            run.Id = _store.CrawlRuns.Select(r => r.Id).DefaultIfEmpty(0).Max() + 1;
            _store.CrawlRuns.Add(run);
        }
        return Task.FromResult(run);
    }

    public Task UpdateAsync(CrawlRun run, CancellationToken ct = default) => Task.CompletedTask;
}
