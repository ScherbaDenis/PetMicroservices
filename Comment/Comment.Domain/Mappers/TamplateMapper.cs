using Comment.Domain.DTOs;

namespace Comment.Domain.Mappers
{
    public static class TamplateMapper
    {
        public static TamplateDto ToDto(this Models.Template e)
        {
            return e == null ? null! : new TamplateDto { Id = e.Id, Title = e.Title };
        }

        public static Models.Template ToEntity(this TamplateDto d)
        {
            return d == null ? null! : new Models.Template { Id = d.Id, Title = d.Title };
        }
    }
}
