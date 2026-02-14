using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using WebApp.Services.DTOs;

namespace WebApp.ModelBinders
{
    public class QuestionDtoModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // Check if the model type is QuestionDto or any of its derived types
            if (context.Metadata.ModelType == typeof(QuestionDto) ||
                typeof(QuestionDto).IsAssignableFrom(context.Metadata.ModelType))
            {
                return new BinderTypeModelBinder(typeof(QuestionDtoModelBinder));
            }

            return null;
        }
    }
}
