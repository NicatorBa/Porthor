namespace Porthor
{
    public class PorthorOptions
    {
        public bool QueryStringValidationEnabled { get; set; }

        public SecurityOptions Security { get; private set; } = new SecurityOptions();

        public ContentOptions Content { get; private set; } = new ContentOptions();
    }
}
