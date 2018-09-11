using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac._2_Parser
{



    // finds all the assign operations with static targets
    public static class Walkers
    {

        public static IEnumerable<ICodeElement> ExtractTopLevelTypeDefinitions(this IEnumerable<ICodeElement> lines, out IReadOnlyList<TypeDefinition> typeDefinitions)
        {
            var nextLines = new List<ICodeElement>();
            var typeDefinitionsList = new List<TypeDefinition>();

            foreach (var line in lines)
            {
                if (line is TypeDefinition typeDefinition)
                {
                    typeDefinitionsList.Add(typeDefinition);
                }
                else {
                    nextLines.Add(line);
                }
            }
            typeDefinitions = typeDefinitionsList;
            return nextLines;
        }

        public static IEnumerable<ICodeElement> ExtractMemberDefinitions(this IEnumerable<ICodeElement> lines,  out IReadOnlyList<MemberDefinition> memberDefinitions)
        {
            var nextLines = new List<ICodeElement>();
            var memberDefinitionList = new List<MemberDefinition>();

            foreach (var line in lines)
            {
                if (line is AssignOperation assignOperation && assignOperation.right  is MemberDefinition memberDefinition )
                {
                    memberDefinitionList.Add(memberDefinition);
                }
                else
                {
                    nextLines.Add(line);
                }
            }
            memberDefinitions = memberDefinitionList;
            return nextLines;
        }

        public static IEnumerable<ICodeElement> ExtractTopLevelAssignOperations(this IEnumerable<ICodeElement> lines, out IReadOnlyList<AssignOperation> assignOperations) {
            var nextLines = new List<ICodeElement>();
            var assignOperationList = new List<AssignOperation>();

            foreach (var line in lines)
            {
                if (line is AssignOperation assignOperation)
                {
                    assignOperationList.Add(assignOperation);
                }
                else
                {
                    nextLines.Add(line);
                }
            }
            assignOperations = assignOperationList;
            return nextLines;
        }


        public static IEnumerable<ICodeElement> ExtractMemberReferances(this IEnumerable<ICodeElement> lines, out IReadOnlyList<(ICodeElement,MemberReferance)> memberReferances)
        {
            var nextLines = new List<ICodeElement>();
            var memberReferanceList = new List<(ICodeElement,MemberReferance)>();

            foreach (var line in lines)
            {
                if (line is AssignOperation assignOperation && assignOperation.right is MemberReferance memberDefinition)
                {
                    memberReferanceList.Add((assignOperation.left, memberDefinition));
                }
                else
                {
                    nextLines.Add(line);
                }
            }
            memberReferances = memberReferanceList;
            return nextLines;
        }


        // this one is a little different

        public static MemberDefinition[] DeepMemberDefinitions(this IEnumerable<ICodeElement> lines) {
            var list = new List<MemberDefinition>();
            foreach (var line in lines)
            {
                list.AddRange(DeepMemberDefinitions(line));
            }
            return list.ToArray();

            MemberDefinition[] DeepMemberDefinitions(ICodeElement line) {
                if (line is MemberDefinition memberDefinition) {
                    return memberDefinition.ToArray();
                }

                if (line is IOperation operation) {
                    return operation.Operands.SelectMany(DeepMemberDefinitions).ToArray();
                }

                return new MemberDefinition[0];
            }
        }


        public static MemberReferance[] DeepMemberReferances(this IEnumerable<ICodeElement> lines)
        {
            var list = new List<MemberReferance>();
            foreach (var line in lines)
            {
                list.AddRange(DeepMemberReferances(line));
            }
            return list.ToArray();

            MemberReferance[] DeepMemberReferances(ICodeElement line)
            {
                if (line is MemberReferance memberReferance)
                {
                    return memberReferance.ToArray();
                }

                if (line is IOperation operation)
                {
                    return operation.Operands.SelectMany(DeepMemberReferances).ToArray();
                }

                return new MemberReferance[0];
            }
        }

    }
}
