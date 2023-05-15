namespace PayDotNet.Stores;

public interface IModelStore<TModel>
{
    Task CreateAsync(TModel model);

    Task DeleteAllAsync(ICollection<TModel> models);

    Task DeleteAsync(TModel model);

    Task UpdateAllAsync(ICollection<TModel> models);

    Task UpdateAsync(TModel model);
}