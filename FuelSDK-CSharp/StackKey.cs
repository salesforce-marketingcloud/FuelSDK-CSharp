using System;
using System.Collections.Concurrent;

namespace FuelSDK
{
    public class StackKey
    {
        ConcurrentDictionary<long, string> values;

        private static readonly Lazy<StackKey> lazy =
            new Lazy<StackKey>(() => new StackKey());

        public static StackKey Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private StackKey()
        {
            values = new ConcurrentDictionary<long, string>();
        }

        public string Get(long enterpriseId, ETClient client)
        {
            return values.GetOrAdd(enterpriseId, (eId) => {
                var restAuth = client.FetchRestAuth();
                var userInfo = new UserInfo(restAuth)
                {
                    AuthStub = client
                };
                return ((UserInfo)userInfo.Get().Results[0]).StackKey;
            });
        }
    }
}
