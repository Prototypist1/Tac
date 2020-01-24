using Prototypist.Toolbox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Tac.Tests.Help
{
    // TODO this does not handle circular stuff very well
    public static class EqualityHelper
    {
        public static void ValueEqualOrThrow<T>(this T target, T actual)
            where T:notnull
        {
            var sharedTypes = target.GetSharedTypes(actual);

            if (!sharedTypes.Any()) {
                sharedTypes = new List<Type>() { typeof(T) };
            }

            if (PublicStateIsValueNotEqual(target, actual,new List<(object,object)>(), sharedTypes) is IIsDefinately<string> value)
            {
                throw new Exception(value.Value);
            }
        }

        private static IEnumerable<Type> GetSharedTypes(this object target, object actual)
        {
            if (target == null || actual == null) {
                return new List<Type> { };
            }

            if (target.GetType() == actual.GetType())
            {
                return new List<Type> { target.GetType() };
            }
            return target.GetType().FindInterfaces((x, y) => true, new object()).Intersect(actual.GetType().FindInterfaces((x, y) => true, new object()));
        }

        /// <summary>
        /// returns Possibly.IsNot when Equal
        /// and
        /// Possibly.Is when not equal
        /// <returns></returns>
        private static IIsPossibly<string> PublicStateIsValueNotEqual(this object target, object actual,IEnumerable<(object,object)> assumed, IEnumerable<Type> types)
        {
            if (target == null && actual == null)
            {
                return Possibly.IsNot<string>(); ;
            }

            if (target == null)
            {
                return  Possibly.Is($" {nameof(target)} is null, {nameof(actual)} is {actual}");
            }

            if (actual == null)
            {
                return Possibly.Is($" {nameof(target)} is {target}, {nameof(actual)} is null");
            }
            
            if (!types.Any()) {
                return Possibly.Is($" {nameof(target)} and {target} are different types");
            }

            if (assumed.Any(x => (x.Item1.Equals(target) && x.Item2.Equals(actual)) ||
                (x.Item2.Equals(target) && x.Item1.Equals(actual))))
            {
                return Possibly.IsNot<string>();
            }
            
            if (target.GetType().IsPrimitive)
            {
                if (!target.Equals(actual))
                {
                    return Possibly.Is($" {nameof(target)} is {target}, {nameof(actual)} is {actual}");
                }
                return Possibly.IsNot<string>();
            }

            if (target is string targetString)
            {
                if (!(actual is string actualString) || targetString != actualString)
                {
                    return Possibly.Is($" {nameof(target)} is {target}, {nameof(actual)} is {actual}");
                }
                return Possibly.IsNot<string>();
            }

            var innerAssummed = new Overlay(assumed, new List<(object, object)> { (target, actual) });

            foreach (var type in types.Where(x => typeof(IEnumerable).IsAssignableFrom(x))) {
                var lists = type.FindInterfaces((x, y) => true, new object()).Where(x => x.Name == "IList`1").ToArray();
                var listType = lists.SelectMany(x => x.GetGenericArguments());
                var enums = type.FindInterfaces((x, y) => true, new object()).Where(x => "IEnumerable`1"==x.Name && !listType.Contains(x.GetGenericArguments().Single()) ).Union(
                    type.FindInterfaces((x, y) => true, new object()).Where(x => "IEnumerable"  == x.Name)).ToArray();

                foreach (var list in lists)
                {
                    var method = list.GetMethod("GetEnumerator");

                    var leftEnumor =(System.Collections.IEnumerator) method.Invoke(target, Array.Empty<object>()); 
                    var rightEnumor = (System.Collections.IEnumerator)method.Invoke(actual, Array.Empty<object>());
                    var i = 0;

                    while (leftEnumor.MoveNext() && rightEnumor.MoveNext())
                    {
                        if (PublicStateIsValueNotEqual(leftEnumor.Current, rightEnumor.Current, innerAssummed, new[] { list.GetGenericArguments().Single() }) is IIsDefinately<string> value)
                        {
                            return Possibly.Is($"[{i}]{value.Value}");
                        }
                        i++;
                    }
                }


                foreach (var enumer in enums)
                {
                    var method = enumer.GetMethod("GetEnumerator");

                    var leftEnum = (System.Collections.IEnumerator)method.Invoke(target, Array.Empty<object>());
                    var rightEnum = (System.Collections.IEnumerator)method.Invoke(actual, Array.Empty<object>());

                    var leftDict = new Dictionary<object,bool>();
                    while (leftEnum.MoveNext())
                    {
                        leftDict[leftEnum.Current] = false;
                    }
                    var rightDict = new Dictionary<object, bool>();
                    while (rightEnum.MoveNext())
                    {
                        rightDict[rightEnum.Current] = false;
                    }

                    if (leftDict.Count != rightDict.Count)
                    {
                        return Possibly.Is($".Count: left {leftDict.Count}, right: {rightDict.Count}");
                    }

                    foreach (var leftKey in leftDict.Keys)
                    {
                        foreach (var rightKey in rightDict.Where(x => x.Value).Select(x => x.Key))
                        {
                            if (PublicStateIsValueNotEqual(leftKey, rightKey, innerAssummed, new[] { enumer.GetGenericArguments().Single() }) is IIsDefinately<string> value)
                            {
                                return Possibly.Is($"[{leftKey}]{value.Value}");
                            }
                            else
                            {
                                leftDict[leftKey] = false;
                                rightDict[rightKey] = false;
                            }
                        }
                    }
                }
            }
        
            
            types = types.Where(x => !new[] { "IEnumerable`1", "IList`1" }.Contains(x.Name)).ToArray();

            foreach (var propertyInfo in GetPropertryInfo())
            {
                if (propertyInfo.CanRead && propertyInfo.GetGetMethod().IsPublic && propertyInfo.GetIndexParameters().Any())
                {
                    var firstValue = propertyInfo.GetValue(target, null);
                    var secondValue = propertyInfo.GetValue(actual, null);
                    if (ReferenceEquals(firstValue, target) && !ReferenceEquals(secondValue, actual) && PublicStateIsValueNotEqual(firstValue, secondValue, innerAssummed, new[] { propertyInfo.PropertyType }) is IIsDefinately<string> value)
                    {
                        return Possibly.Is($".{propertyInfo.Name}{value.Value}");
                    }
                }
            }

            foreach (var fieldInfo in GetFieldInfo())
            {
                if (fieldInfo.IsPublic)
                {
                    var firstValue = fieldInfo.GetValue(target);
                    var secondValue = fieldInfo.GetValue(actual);
                    if (PublicStateIsValueNotEqual(firstValue, secondValue, innerAssummed, new[] { fieldInfo.FieldType }) is IIsDefinately<string> value)
                    {
                        return Possibly.Is($".{fieldInfo.Name}{value.Value}");
                    }
                }
            }

            return Possibly.IsNot<string>();

            PropertyInfo[] GetPropertryInfo()
            {
                return types.SelectMany(x => x.GetProperties()).ToArray();
            }

            FieldInfo[] GetFieldInfo()
            {
                return types.SelectMany(x => x.GetFields()).ToArray();
            }
        }

        private class Overlay : IEnumerable<(object, object)> {
            private readonly IEnumerable<(object, object)> backing;
            private readonly IEnumerable<(object, object)> overlay;

            public Overlay(IEnumerable<(object, object)> backing, IEnumerable<(object, object)> overlay)
            {
                this.backing = backing ?? throw new ArgumentNullException(nameof(backing));
                this.overlay = overlay ?? throw new ArgumentNullException(nameof(overlay));
            }

            public IEnumerator<(object, object)> GetEnumerator()
            {
                foreach (var item in overlay)
                {
                    yield return item;
                }
                foreach (var item in backing)
                {
                    yield return item;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
