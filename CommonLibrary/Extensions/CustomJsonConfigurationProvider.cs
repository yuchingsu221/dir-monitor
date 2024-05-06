using Microsoft.Extensions.Configuration.Json;
using System.IO;
using CommonLibrary.Crypto;

namespace CommonLibrary
{
    public class CustomJsonConfigurationProvider : JsonConfigurationProvider
    {
        public CustomJsonConfigurationProvider(CustomJsonConfigurationSource source) : base(source)
        {
        }

        public override void Load(Stream stream)
        {
            base.Load(stream);

            var keys = new string[this.Data.Keys.Count];
            this.Data.Keys.CopyTo(keys, 0);
            
            foreach (var key in keys) {
                if (key.StartsWith("RelationDB:"))
                {
                    if (!string.IsNullOrWhiteSpace(Data[key]))
                    {
                        Data[key] = AESHelper.Decrypt256(Data[key], "Lp2s5v8y/B?E(H+MbQeThVmYq3t6w9z$", "gVkXp2s5v8y/B?D(G");
                    }

                }
            }
        }

    }
}