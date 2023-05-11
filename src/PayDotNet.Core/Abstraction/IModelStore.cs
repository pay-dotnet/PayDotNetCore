namespace PayDotNet.Core.Abstraction;

public interface IModelStore<TModel>
{
    Task CreateAsync(TModel model);

    Task UpdateAsync(TModel model);

    Task UpdateAllAsync(ICollection<TModel> models);

    Task DeleteAsync(TModel model);

    Task DeleteAllAsync(ICollection<TModel> models);
}