using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Model.Instantiated
{

    public static class TransformerExtensions
    {

        public static IConversionContext NewConversionContext() {
            return new ConversionContext();
        }

        // TODO thread safe?
        private class ConversionContext : IConversionContext
        {
            internal readonly Dictionary<object, object> backing = new Dictionary<object, object>();

            public void AddOrThrow(object key, object value)
            {
                backing.Add(key, value);
            }

            public bool TryGetValue(object key, out object value)
            {
                return backing.TryGetValue(key, out value);
            }
        }

        public static T Convert<T>(this IConvertable<T> self, IConversionContext context)
            where T : class
        {
            if (context.TryGetValue(self, out var res))
            {
                return res.Cast<T>();
            }
            else
            {
                var buildIntention = self.GetBuildIntention(context);
                context.AddOrThrow(self, buildIntention.Tobuild);
                buildIntention.Build();
                return buildIntention.Tobuild;
            }
        }
    }
}
