using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary
{
    public class CustomJsonStreamConfigurationSource : JsonStreamConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new CustomJsonStreamConfigurationProvider(this);
        }
    }
}
