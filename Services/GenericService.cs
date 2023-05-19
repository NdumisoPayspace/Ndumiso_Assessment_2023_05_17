namespace Ndumiso_Assessment_2023_05_17.Services
{
    using Microsoft.EntityFrameworkCore;

    using Ndumiso_Assessment_2023_05_17.Data;
    using Ndumiso_Assessment_2023_05_17.Interfaces;

    public class GenericService<T> : IGenericService<T> where T : class
    {
        private readonly ApplicationDbContext context;

        public GenericService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public List<T> GetAll()
        {
            return context.Set<T>().ToList();
        }

        public T GetById(int id)
        {
            return context.Set<T>().Find(id);
        }
        public void Add(T entity)
        {
            context.Set<T>().Add(entity);
            context.SaveChanges();
        }
        public void Update(T entity)
        {
            context.Set<T>().Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            T? entity = context.Set<T>().Find(id);
            context.Set<T>().Remove(entity!);
            context.SaveChanges();
        }
    }
}
