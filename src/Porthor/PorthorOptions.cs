namespace Porthor
{
    public class PorthorOptions
    {
        public bool EnableQueryParameters { get; set; }

        public SecurityOptions SecurityOptions { get; private set; } = new SecurityOptions();

        public ContentValidationOptions ContentValidationOptions { get; private set; } = new ContentValidationOptions();
    }
}
