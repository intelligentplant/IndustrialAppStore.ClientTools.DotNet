using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using DataCore.Adapter;
using DataCore.Adapter.Common;
using DataCore.Adapter.Tags;

namespace ExampleHostedAdapter {

    // This file implements the ITagSearch adapter feature, meaning that our adapter allows
    // callers to discover available tags (measurements) that can be read. In our example we use a
    // fixed set of tags created at startup time, but your implementation might e.g. query a
    // database to get a list of available measurements.
    //
    // See https://github.com/intelligentplant/AppStoreConnect.Adapters for more details.

    partial class ExampleHostedAdapter : ITagSearch {

        private static readonly AdapterProperty s_tagCreatedAtPropertyDefinition = new AdapterProperty("UtcCreatedAt", DateTime.MinValue, "The UTC creation time for the tag");

        private static readonly AdapterProperty[] s_tagPropertyDefinitions = {
            s_tagCreatedAtPropertyDefinition
        };

        private readonly ConcurrentDictionary<string, TagDefinition> _tags = new ConcurrentDictionary<string, TagDefinition>(StringComparer.OrdinalIgnoreCase) { 
            ["test"] = new TagDefinitionBuilder("test", "Test Tag")
                .WithDescription("An example tag that can be polled for snapshot (current) values.")
                .WithDataType(VariantType.Double)
                .WithSupportsReadSnapshotValues()
                .WithProperty(s_tagCreatedAtPropertyDefinition.Name, DateTime.UtcNow)
                .Build()
        };


        private bool TryGetTagByIdOrName(string tagIdOrName, out TagDefinition tag) {
            if (_tags.TryGetValue(tagIdOrName, out tag)) {
                return true;
            }

            tag = _tags.Values.FirstOrDefault(x => string.Equals(x.Name, tagIdOrName, StringComparison.OrdinalIgnoreCase));
            return tag != null;
        }


        public async IAsyncEnumerable<AdapterProperty> GetTagProperties(
            IAdapterCallContext context, 
            GetTagPropertiesRequest request, 
            [EnumeratorCancellation]
            CancellationToken cancellationToken
        ) {
            ValidateInvocation(context, request);

            await Task.CompletedTask.ConfigureAwait(false);

            using (var ctSource = CreateCancellationTokenSource(cancellationToken)) {
                foreach (var prop in s_tagPropertyDefinitions.OrderBy(x => x.Name).SelectPage(request)) {
                    if (ctSource.IsCancellationRequested) {
                        break;
                    }
                    yield return prop;
                }
            }
        }


        public async IAsyncEnumerable<TagDefinition> GetTags(
            IAdapterCallContext context, 
            GetTagsRequest request, 
            [EnumeratorCancellation]
            CancellationToken cancellationToken
        ) {
            ValidateInvocation(context, request);

            await Task.CompletedTask.ConfigureAwait(false);

            using (var ctSource = CreateCancellationTokenSource(cancellationToken)) {
                foreach (var tag in request.Tags) {
                    if (ctSource.IsCancellationRequested) {
                        break;
                    }

                    if (!TryGetTagByIdOrName(tag, out var tagDef)) {
                        continue;
                    }

                    yield return tagDef;
                }
            }
        }


        public async IAsyncEnumerable<TagDefinition> FindTags(
            IAdapterCallContext context, 
            FindTagsRequest request, 
            [EnumeratorCancellation]
            CancellationToken cancellationToken
        ) {
            ValidateInvocation(context, request);

            await Task.CompletedTask.ConfigureAwait(false);

            using (var ctSource = CreateCancellationTokenSource(cancellationToken)) {
                foreach (var tagDef in _tags.Values.ApplyFilter(request)) {
                    if (ctSource.IsCancellationRequested) {
                        break;
                    }

                    yield return tagDef;
                }
            }
        }

        
    }
}
