using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using Tac.Model;
using Tac.Model.Instantiated;
using Tac.Backend.Emit.SyntaxModel.Run_Time_Objects;

namespace Tac.Backend.Emit.SyntaxModel
{
    internal class InterpetedModuleDefinition : IAssembledOperation
    {
        public void Init(IInterpetedScopeTemplate scope, IEnumerable<IAssembledOperation> staticInitialization, InterpetedEntryPointDefinition interpetedEntry)
        {
            ScopeTemplate = scope ?? throw new ArgumentNullException(nameof(scope));
            StaticInitialization = staticInitialization ?? throw new ArgumentNullException(nameof(staticInitialization));
            InterpetedEntry = interpetedEntry ?? throw new ArgumentNullException(nameof(interpetedEntry));
        }


        private InterpetedEntryPointDefinition? interpetedEntry;
        public InterpetedEntryPointDefinition InterpetedEntry { get => interpetedEntry ?? throw new NullReferenceException(nameof(interpetedEntry)); private set => interpetedEntry = value ?? throw new NullReferenceException(nameof(value)); }


        private IInterpetedScopeTemplate? scopeTemplate;
        public IInterpetedScopeTemplate ScopeTemplate { get => scopeTemplate ?? throw new NullReferenceException(nameof(scopeTemplate)); private set => scopeTemplate = value ?? throw new NullReferenceException(nameof(value)); }


        private IEnumerable<IAssembledOperation>? staticInitialization;
        public IEnumerable<IAssembledOperation> StaticInitialization { get => staticInitialization ?? throw new NullReferenceException(nameof(staticInitialization)); private set => staticInitialization = value ?? throw new NullReferenceException(nameof(value)); }


        public IInterpetedResult<IInterpetedMember> Assemble(AssemblyContext interpetedContext)
        {
            var scope = ScopeTemplate.Create();

            var context = interpetedContext.Child(scope);

            foreach (var line in StaticInitialization)
            {
                line.Assemble(context);
            }

            return InterpetedResult.Create(TypeManager.Member(scope.Convert(TransformerExtensions.NewConversionContext()), scope));
        }
        
        //public IInterpetedScope GetDefault()
        //{
        //    return TypeManager.EmptyStaticScope();
        //}
    }
}