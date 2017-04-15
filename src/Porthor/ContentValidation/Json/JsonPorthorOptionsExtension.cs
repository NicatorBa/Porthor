namespace Porthor.ContentValidation.Json
{
    public static class JsonPorthorOptionsExtension
    {
        public static PorthorOptions AddJson(this PorthorOptions options)
        {
            options.AddContentValidatorFactory(MediaType.Json, new JsonContentValidatorFactory());
            return options;
        }
    }
}
