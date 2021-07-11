using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;

namespace Tac.Backend.Emit
{

    public class Assembly : IAssembly<object>
    {
        public Assembly(NameKey key, IInterfaceType scope, object backing)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.Backing = backing ?? throw new ArgumentNullException(nameof(backing));
        }

        //public IReadOnlyList<IAssembly<EmitAssemblyBacking>> References => new List<IAssembly<EmitAssemblyBacking>>();

        public NameKey Key { get; }
        public IInterfaceType Scope { get; }
        public object Backing { get; }
    }

    //public class ExternalAssemblyBuilder
    //{
    //    private readonly IKey key;
    //    private readonly List<IsStatic> members = new List<IsStatic>();

    //    public ExternalAssemblyBuilder(IKey key)
    //    {
    //        this.key = key ?? throw new ArgumentNullException(nameof(key));
    //    }

    //    public ExternalAssemblyBuilder AddMethod<TIn, TOut>(IKey key, Func<TIn, TOut> func, IMethodType type)
    //    {
    //        members.Add(new IsStatic(MemberDefinition.CreateAndBuild(key, type, Access.ReadOnly), false));
    //        return this;
    //    }

    //    // on the backend it is just an object
    //    // and on the backend I just need to take that object and assign it to a memeber
    //    // I just analyze the object and create a scope

    //    // just an object with a lot of annotations?
    //    // every member needs it's tac side name and it's tac side type, how do I capture that?

    //    // maybe don't create an object
    //    // just defince some structure 
    //    // and then have a method that takes a TacCompilation and assignes value reflectively

    //    // but... what if I'm trying to have a type in a dependency
    //    // I think tac can refernece external types
    //    // as long as the member definitions align

    //    // so... back to the whole dependency is a instance
    //    // and I just manually create the metadata to support it
        
    //    public class EmitAssemblyBacking
    //    {
           
    //    }

    //    public IAssembly<EmitAssemblyBacking> Build()
    //    {
    //        var scope = new Scope();

    //        scope.Build(members);

    //        return new Assembly(
    //            key,
    //            scope,
    //            new EmitAssemblyBacking()
    //            );
    //    }


    //}
}
