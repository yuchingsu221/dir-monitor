using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace CommonLibrary
{
    public static class JsonConfigurationExtensions
    {
        public static IConfigurationBuilder CustomAddJsonFile(this IConfigurationBuilder builder, string path, bool optional, bool reloadOnChange)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("File path must be a non-empty string.");
            }

            var source = new CustomJsonConfigurationSource
            {
                FileProvider = null,
                Path = path,
                Optional = optional,
                ReloadOnChange = reloadOnChange
            };

            source.ResolveFileProvider();
            builder.Add(source);
            return builder;
        }

        public static IConfigurationBuilder CustomAddJsonStream(this IConfigurationBuilder builder, Stream contentStream, bool optional, bool reloadOnChange)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            // 如果是空的就不讀了
            if (contentStream == null || contentStream.Length == 0)
            {
                return builder;
            }

            var source = new CustomJsonStreamConfigurationSource
            {
                Stream = contentStream
            };

            builder.Add(source);
            return builder;
        }
    }
}