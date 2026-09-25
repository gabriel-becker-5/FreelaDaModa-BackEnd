using _02_Application.DTOs;
using _02_Application.DTOs.Freelancer;
using _04_Domain.Enums;

namespace _02_Application.Validation
{
    public record DtoFieldValidation(string FieldName, Type EnumType, ICollection<int> Values)
    {
        // Sobrecarga do Record, permitindo passar um 'int' ao invés de um 'ICollection<int>'
        public DtoFieldValidation(string fieldName, Type enumType, int value)
            : this(fieldName, enumType, new[] { value }) { }
    }

    public static class FreelancerFieldValidator
    {
        public static bool IsIdValid<T>(int id) where T : struct, Enum
        {
            return IsIdValid<T>(new[] { id });
        }

        public static bool IsIdValid<T>(IEnumerable<int> ids) where T : struct, Enum
        {
            return ids.All(id => Enum.IsDefined(typeof(T), id));
        }

        public static ICollection<EnumValidationError> ValidateDtoFields(ICollection<DtoFieldValidation> fields)
        {
            var errors = new List<EnumValidationError>();

            foreach (var field in fields)
            {
                foreach (var value in field.Values)
                {
                    if (!Enum.IsDefined(field.EnumType, value))
                    {
                        errors.Add(new EnumValidationError
                        {
                            EnumType = field.EnumType.Name,
                            FieldName = field.FieldName,
                            Value = value
                        });
                    }
                }
            }

            return errors;
        }

        public static ICollection<EnumValidationError> IsFreelancerFieldsValid(CreateFreelancerDto dto)
        {
            List<DtoFieldValidation> fields = new()
            {
                new("AvailableTimeId", typeof(AvailableTime), dto.AvailableTimeId),
                new("ExperienceYearsId", typeof(ExperienceYears), dto.ExperienceYearsId),
                new("SpecialtyIds", typeof(Specialty), dto.SpecialtyIds),
                new("OwnMachineIds", typeof(OwnMachine), dto.OwnMachineIds)
            };

            return ValidateDtoFields(fields);
        }
    }
}