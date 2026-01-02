using Comment.Domain.DTOs;

namespace Comment.Domain.Mappers
{
    public static class TemplateMapper
    {
        public static TemplateDto ToDto(this Models.Template e)
        {
            return e == null ? null! : new TemplateDto { Id = e.Id, Title = e.Title };
        }

        public static Models.Template ToEntity(this TemplateDto d)
        {
            return d == null ? null! : new Models.Template { Id = d.Id, Title = d.Title };
        }
    }
}
