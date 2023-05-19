namespace Ndumiso_Assessment_2023_05_17.Interfaces
{
    public interface IGenericService<T> where T : class
    {
        List<T> GetAll();

        T GetById(int id);

        void Add(T entity);

        void Update(T entity);

        void Delete(int id);
    }
}
