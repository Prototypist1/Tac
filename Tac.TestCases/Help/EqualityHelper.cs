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
        {
            if (!PublicStateIsValueEqual(target, actual,new List<(object,object)>(), new List<Type>() { typeof(T) }, out var res))
            {
                throw new Exception(res);
            }
        }

        private static bool PublicStateIsValueEqual(this object target, object actual,IEnumerable<(object,object)> assumed, IEnumerable<Type> types,  out string error)
        {
            if (target == null && actual == null)
            {
                error = null;
                return true;
            }

            if (target == null)
            {
                error = $" {nameof(target)} is null, {nameof(actual)} is {actual}";
                return false;
            }

            if (actual == null)
            {
                error = $" {nameof(target)} is {target}, {nameof(actual)} is null";
                return false;
            }

            if (!types.Any()) {
                types = GetTypes();
            }

            if (!types.Any()) {
                error = $" {nameof(target)} and {target} are different types";
                return false;
            }

            if (assumed.Any(x => (x.Item1.Equals(target) && x.Item2.Equals(actual)) ||
                (x.Item2.Equals(target) && x.Item1.Equals(actual))))
            {
                error = null;
                return true;
            }
            
            if (target.GetType().IsPrimitive)
            {
                if (!target.Equals(actual))
                {
                    error = $" {nameof(target)} is {target}, {nameof(actual)} is {actual}";
                    return false;
                }
                error = null;
                return true;
            }

            if (target is string targetString)
            {
                if (!(actual is string actualString) || targetString != actualString)
                {
                    error = $" {nameof(target)} is {target}, {nameof(actual)} is {actual}";
                    return false;
                }
                error = null;
                return true;
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

                    var leftEnumor =(System.Collections.IEnumerator) method.Invoke(target, new object[] { }); 
                    var rightEnumor = (System.Collections.IEnumerator)method.Invoke(actual, new object[] { });
                    var i = 0;

                    while (leftEnumor.MoveNext() && rightEnumor.MoveNext())
                    {
                        if (!PublicStateIsValueEqual(leftEnumor.Current, rightEnumor.Current, innerAssummed, new[] { list.GetGenericArguments().Single() }, out var err))
                        {
                            error = $"[{i}]{err}";
                            return false;
                        }
                        i++;
                    }
                }


                foreach (var enumer in enums)
                {
                    var method = enumer.GetMethod("GetEnumerator");

                    var leftEnum = (System.Collections.IEnumerator)method.Invoke(target, new object[] { });
                    var rightEnum = (System.Collections.IEnumerator)method.Invoke(actual, new object[] { });

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
                        error = $".Count: left {leftDict.Count}, right: {rightDict.Count}";
                        return false;
                    }

                    foreach (var leftKey in leftDict.Keys)
                    {
                        foreach (var rightKey in rightDict.Where(x => x.Value).Select(x => x.Key))
                        {
                            if (PublicStateIsValueEqual(leftKey, rightKey, innerAssummed, new[] { enumer.GetGenericArguments().Single() }, out var err))
                            {
                                leftDict[leftKey] = false;
                                rightDict[rightKey] = false;
                            }
                            else
                            {
                                error = $"[{leftKey}]{err}";
                                return false;
                            }
                        }
                    }
                }
            }
        
            
            types = types.Where(x => !new[] { "IEnumerable`1", "IList`1" }.Contains(x.Name)).ToArray();

            foreach (var propertyInfo in GetPropertryInfo())
            {
                if (propertyInfo.CanRead && propertyInfo.GetGetMethod().IsPublic && propertyInfo.GetIndexParameters().Count() == 0)
                {
                    var firstValue = propertyInfo.GetValue(target, null);
                    var secondValue = propertyInfo.GetValue(actual, null);
                    if (!ReferenceEquals(firstValue, target) && !ReferenceEquals(secondValue, actual) && !PublicStateIsValueEqual(firstValue, secondValue, innerAssummed, new[] { propertyInfo.PropertyType }, out var res))
                    {
                        error = $".{propertyInfo.Name}{res}";
                        return false;
                    }
                }
            }

            foreach (var fieldInfo in GetFieldInfo())
            {
                if (fieldInfo.IsPublic)
                {
                    var firstValue = fieldInfo.GetValue(target);
                    var secondValue = fieldInfo.GetValue(actual);
                    if (!PublicStateIsValueEqual(firstValue, secondValue, innerAssummed, new[] { fieldInfo.FieldType }, out var res))
                    {
                        error = $".{fieldInfo.Name}{res}";
                        return false;
                    }
                }
            }

            error = null;
            return true;
            
            IEnumerable<Type> GetTypes() {
                if (target.GetType() == actual.GetType())
                {
                    return new List<Type> { target.GetType() };
                }
                return target.GetType().FindInterfaces((x, y) => true, new object()).Intersect(actual.GetType().FindInterfaces((x, y) => true, new object()));
            }

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
