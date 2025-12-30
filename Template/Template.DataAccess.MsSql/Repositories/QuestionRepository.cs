namespace Template.DataAccess.MsSql.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Template.Domain.Model;
    using Template.Domain.Repository;

    public class QuestionRepository : IQuestionRepository
    {
        private readonly TemplateDbContext _context;

        public QuestionRepository(TemplateDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(Question entity, CancellationToken cancellationToken = default)
        {
            await _context.Questions.AddAsync(entity, cancellationToken);
        }

        public async Task UpdateAsync(Question entity, CancellationToken cancellationToken = default)
        {
            _context.Questions.Update(entity);
        }

        public async Task DeleteAsync(Question entity, CancellationToken cancellationToken = default)
        {
            _context.Questions.Remove(entity);
        }

        public IEnumerable<Question> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return _context.Questions.ToList();
        }

        public async Task<Question?> FindAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Questions.FindAsync(new object?[] { id }, cancellationToken: cancellationToken);
        }

        public IEnumerable<Question> Find(Func<Question, bool> predicate)
        {
            return _context.Questions.Where(predicate);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
