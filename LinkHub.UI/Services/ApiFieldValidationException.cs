namespace LinkHub.UI.Services
{
    public sealed class ApiFieldValidationException : Exception
    {
        public string FieldName { get; }

        public ApiFieldValidationException(string fieldName, string message)
            : base(message)
        {
            FieldName = fieldName;
        }
    }
}
