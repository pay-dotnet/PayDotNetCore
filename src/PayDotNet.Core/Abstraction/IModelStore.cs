namespace PayDotNet.Core.Abstraction;

public interface IModelStore<TModel>
{
    Task CreateAsync(TModel model);

    Task UpdateAsync(string id, TModel model);

}
