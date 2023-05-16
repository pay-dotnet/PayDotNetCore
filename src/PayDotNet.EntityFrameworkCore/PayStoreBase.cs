using Microsoft.EntityFrameworkCore;

namespace PayDotNet.EntityFrameworkCore;

public class PayStoreBase<TModel, TContext> : IModelStore<TModel>
    where TModel : class
    where TContext : DbContext
{
    public PayStoreBase(TContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        Context = context;
    }

    public virtual TContext Context { get; }

    protected DbSet<TModel> Entities => Context.Set<TModel>();

    public async Task CreateAsync(TModel model)
    {
        await Entities.AddAsync(model);
        await Context.SaveChangesAsync();
    }

    public Task DeleteAllAsync(ICollection<TModel> models)
    {
        Entities.RemoveRange(models);
        return Context.SaveChangesAsync();
    }

    public Task DeleteAsync(TModel model)
    {
        Entities.Remove(model);
        return Context.SaveChangesAsync();
    }

    public Task UpdateAllAsync(ICollection<TModel> models)
    {
        Entities.UpdateRange(models);
        return Context.SaveChangesAsync();
    }

    public Task UpdateAsync(TModel model)
    {
        Entities.Update(model);
        return Context.SaveChangesAsync();
    }
}