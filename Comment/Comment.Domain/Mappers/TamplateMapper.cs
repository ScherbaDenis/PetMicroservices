using Comment.Domain.DTOs;

namespace Comment.Domain.Mappers
{
    public static class TamplateMapper
    {
        public static TamplateDto ToDto(this Models.Tamplate e)
        {
            return e == null ? null! : new TamplateDto { Id = e.Id, Title = e.Title };
        }

        public static Models.Tamplate ToEntity(this TamplateDto d)
        {
            return d == null ? null! : new Models.Tamplate { Id = d.Id, Title = d.Title };
        }
    }
}
