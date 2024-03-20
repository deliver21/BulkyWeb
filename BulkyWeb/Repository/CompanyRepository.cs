using BulkyWeb.Data;
using BulkyWeb.Models;

namespace BulkyWeb.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private ApplicationDbContext _db;
        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            this._db = db;
        }
        public void Update(Company company)
        {
            _db.Companies.Update(company);
        }
    }
}
