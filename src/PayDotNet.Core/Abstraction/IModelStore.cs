namespace PayDotNet.Core.Abstraction;

public interface IModelStore<TModel>
{
    Task CreateAsync(TModel model);

    Task UpdateAsync(TModel model);

    Task UpdateAsync(ICollection<TModel> models);
}
