using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;
using static Tac.Frontend.TransformerExtensions;

namespace Tac.Frontend
{
    public class BuildIntention<T> : IBuildIntention<T> where T:class
    {
        public T Tobuild { get; }
        public Action Build { get; }

        public BuildIntention(T tobuild, Action build)
        {
            this.Tobuild = tobuild ?? throw new ArgumentNullException(nameof(tobuild)); ;
            this.Build = build ?? throw new ArgumentNullException(nameof(build));
        }
    }

    public interface IBuildIntention<out T>
    {
        Action Build { get; }
        T Tobuild { get; }
    }

    internal interface IConvertable<out T>
    {
        IBuildIntention<T> GetBuildIntention(ConversionContext context);
    }

    public static class TransformerExtensions {

        public class ConversionContext {
            internal readonly Dictionary<object, object> backing = new Dictionary<object, object>();
        }

        internal static T Convert<T>(this IConvertable<T> self, ConversionContext context)
            where T:class
        {
            if (context.backing.TryGetValue(self, out var res))
            {
                return res.Cast<T>();
            }
            else
            {
                var buildIntention = self.GetBuildIntention(context);
                context.backing.Add(self, buildIntention.Tobuild);
                buildIntention.Build();
                return buildIntention.Tobuild;
            }
        }
        
    }
}
