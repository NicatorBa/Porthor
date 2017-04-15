using System.Threading.Tasks;

namespace Porthor.ContentValidation
{
    public abstract class ContentValidatorFactory
    {
        public abstract Task<IContentValidator> CreateContentValidatorAsync(string template);
    }
}
