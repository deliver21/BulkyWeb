using BulkyWeb.Models;

namespace BulkyWeb.Repository
{
    public interface ICompanyRepository: IRepository<Company>
    {
        void Update(Company company);
    }
}
