using Microsoft.AspNetCore.Mvc.ModelBinding;
using WebApp.Services.DTOs;

namespace WebApp.ModelBinders
{
    public class QuestionDtoModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            // Get the QuestionType value from the form
            var questionTypeValue = bindingContext.ValueProvider.GetValue("QuestionType");
            if (questionTypeValue == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            var questionType = questionTypeValue.FirstValue;
            if (string.IsNullOrEmpty(questionType))
            {
                return Task.CompletedTask;
            }

            // Create the appropriate concrete type based on QuestionType
            QuestionDto? model = questionType switch
            {
                "SingleLineString" => new SingleLineStringQuestionDto(),
                "MultiLineText" => new MultiLineTextQuestionDto(),
                "PositiveInteger" => new PositiveIntegerQuestionDto(),
                "Checkbox" => new CheckboxQuestionDto(),
                "Boolean" => new BooleanQuestionDto(),
                _ => null
            };

            if (model == null)
            {
                bindingContext.ModelState.AddModelError(
                    bindingContext.ModelName,
                    $"Unknown question type: {questionType}");
                return Task.CompletedTask;
            }

            // Bind common properties
            var idValue = bindingContext.ValueProvider.GetValue("Id");
            if (idValue != ValueProviderResult.None && Guid.TryParse(idValue.FirstValue, out var id))
            {
                model.Id = id;
            }

            var titleValue = bindingContext.ValueProvider.GetValue("Title");
            if (titleValue != ValueProviderResult.None)
            {
                model.Title = titleValue.FirstValue ?? string.Empty;
            }

            var descriptionValue = bindingContext.ValueProvider.GetValue("Description");
            if (descriptionValue != ValueProviderResult.None)
            {
                model.Description = descriptionValue.FirstValue;
            }

            // Set the QuestionType property
            model.QuestionType = questionType;

            // Bind Options for CheckboxQuestion
            if (model is CheckboxQuestionDto checkboxQuestion)
            {
                var options = new List<string>();
                var optionsValue = bindingContext.ValueProvider.GetValue("Options");
                
                if (optionsValue != ValueProviderResult.None)
                {
                    // Handle both indexed (Options[0], Options[1], ...) and single value
                    foreach (var option in optionsValue.Values)
                    {
                        if (!string.IsNullOrWhiteSpace(option))
                        {
                            options.Add(option);
                        }
                    }
                }

                if (options.Any())
                {
                    checkboxQuestion.Options = options;
                }
            }

            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }
    }
}
