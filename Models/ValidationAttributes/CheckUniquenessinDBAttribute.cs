using Spider_EMT.Configuration.IService;
using System.ComponentModel.DataAnnotations;

namespace Spider_EMT.Models.ValidationAttributes
{
    public class CheckUniquenessinDBAttribute : ValidationAttribute
    {
        private readonly string _field;
        public CheckUniquenessinDBAttribute(string field)
        {
            _field = field;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var uniquenessCheckService = (IUniquenessCheckService)validationContext.GetService(typeof(IUniquenessCheckService));
            if (_field != null)
            {
                var result = uniquenessCheckService.IsUniqueAsync(_field, value as string).Result;
                if(!result)
                {
                    return new ValidationResult($"'{value as string}' is already taken. Please provide another {_field}.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
