namespace CJTasksHelperBot.Persistence.DataSeeding;

public class EntityList<TEntity> : List<TEntity>
{
    private readonly Func<TEntity, TEntity, bool> _comparer;

    public EntityList(Func<TEntity, TEntity, bool> comparer)
    {
        _comparer = comparer;
    }

    public void AddIfNotExists(TEntity entity)
    {
        if (!Exists(x => _comparer(x, entity)))
        {
            Add(entity);
        }
    }
    
    public void AddIfNotExistsRange(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            AddIfNotExists(entity);
        }
    }
}