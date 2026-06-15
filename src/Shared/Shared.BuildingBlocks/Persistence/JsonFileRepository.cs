using System.Text.Json;

namespace Shared.BuildingBlocks.Persistence;

public abstract class JsonFileRepository<TEntity, TKey> where TEntity : class
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _options;
    private readonly SemaphoreSlim _lock = new(1, 1);

    protected JsonFileRepository(string filePath, JsonSerializerOptions? options = null)
    {
        _filePath = filePath;
        _options = options ?? new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    protected abstract TKey GetKey(TEntity entity);

    protected abstract TEntity WithKey(TEntity entity, TKey key);

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await ReadAllAsync(cancellationToken);
        return items.AsReadOnly();
    }

    public async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var items = await ReadAllAsync(cancellationToken);
        return items.FirstOrDefault(item => EqualityComparer<TKey>.Default.Equals(GetKey(item), id));
    }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            var items = await ReadAllInternalAsync(cancellationToken);
            items.Add(entity);
            await WriteAllInternalAsync(items, cancellationToken);
            return entity;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<TEntity?> UpdateAsync(TKey id, Func<TEntity, TEntity> update, CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            var items = await ReadAllInternalAsync(cancellationToken);
            var index = items.FindIndex(item => EqualityComparer<TKey>.Default.Equals(GetKey(item), id));
            if (index < 0)
            {
                return null;
            }

            items[index] = update(items[index]);
            await WriteAllInternalAsync(items, cancellationToken);
            return items[index];
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            var items = await ReadAllInternalAsync(cancellationToken);
            var removed = items.RemoveAll(item => EqualityComparer<TKey>.Default.Equals(GetKey(item), id));
            if (removed == 0)
            {
                return false;
            }

            await WriteAllInternalAsync(items, cancellationToken);
            return true;
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<List<TEntity>> ReadAllAsync(CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            return await ReadAllInternalAsync(cancellationToken);
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<List<TEntity>> ReadAllInternalAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_filePath))
        {
            return [];
        }

        await using var stream = File.OpenRead(_filePath);
        var items = await JsonSerializer.DeserializeAsync<List<TEntity>>(stream, _options, cancellationToken);
        return items ?? [];
    }

    private async Task WriteAllInternalAsync(List<TEntity> items, CancellationToken cancellationToken)
    {
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, items, _options, cancellationToken);
    }
}
