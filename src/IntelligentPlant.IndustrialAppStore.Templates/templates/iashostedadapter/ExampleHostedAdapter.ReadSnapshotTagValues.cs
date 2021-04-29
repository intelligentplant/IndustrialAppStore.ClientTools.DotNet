using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using DataCore.Adapter;
using DataCore.Adapter.RealTimeData;

namespace ExampleHostedAdapter {

    // This file implements the IReadSnapshotTagValues adapter feature, which allows tags to be
    // polled for their current value.
    //
    // See https://github.com/intelligentplant/AppStoreConnect.Adapters for more details.

    partial class ExampleHostedAdapter : IReadSnapshotTagValues {
        public async IAsyncEnumerable<TagValueQueryResult> ReadSnapshotTagValues(
            IAdapterCallContext context, 
            ReadSnapshotTagValuesRequest request, 
            [EnumeratorCancellation]
            CancellationToken cancellationToken
        ) {
            ValidateInvocation(context, request);

            await Task.CompletedTask.ConfigureAwait(false);

            var now = DateTime.UtcNow;

            Random GetRng(string tagId) {
                return new Random((tagId.GetHashCode() + now.GetHashCode()).GetHashCode());
            }

            using (var ctSource = CreateCancellationTokenSource(cancellationToken)) {
                foreach (var tag in request.Tags) {
                    if (ctSource.IsCancellationRequested) {
                        break;
                    }

                    if (!TryGetTagByIdOrName(tag, out var tagDef)) {
                        continue;
                    }

                    var rnd = GetRng(tagDef.Id);

                    var value = new TagValueBuilder().WithUtcSampleTime(now).WithValue(rnd.NextDouble() * 100).Build();
                    yield return new TagValueQueryResult(tagDef.Id, tagDef.Name, value);
                }
            }
        }
    }
}
