using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace CommonLibrary
{
    public class CustomJsonConfigurationSource : JsonConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new CustomJsonConfigurationProvider(this);
        }
    }
}