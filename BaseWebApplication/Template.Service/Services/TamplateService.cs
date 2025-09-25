using Template.Domain.Model;
using Template.Domain.Repository;
using Template.Domain.Services;

namespace Template.Service.Services
{
    public class TamplateService(ITamplateRepository tamplateRepository) : ITamplateService
    {
        private readonly ITamplateRepository tamplateRepository = tamplateRepository;

        public void CreateAsync(Tamplate item, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void DeleteAsync(Tamplate item, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Tamplate?> FindAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tamplate> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void UpdateAsync(Tamplate item, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
