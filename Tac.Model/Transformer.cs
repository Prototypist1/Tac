using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Model
{

    public interface IBuildIntention<out T>
    {
        Action Build { get; }
        T Tobuild { get; }
    }

    public interface IConvertable<out T>
    {
        IBuildIntention<T> GetBuildIntention(IConversionContext context);
    }

    public interface IConversionContext {
        bool TryGetValue(object key, out object value);
        void AddOrThrow(object key, object value);
    }

    public class BuildIntention<T> : IBuildIntention<T> where T : class
    {
        public T Tobuild { get; }
        public Action Build { get; }

        public BuildIntention(T tobuild, Action build)
        {
            this.Tobuild = tobuild ?? throw new ArgumentNullException(nameof(tobuild)); ;
            this.Build = build ?? throw new ArgumentNullException(nameof(build));
        }
    }
}
