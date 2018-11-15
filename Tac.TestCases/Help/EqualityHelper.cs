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
        public static void ValueEqualOrThrow(this object target, object actual)
        {
            if (!PublicStateIsValueEqual(target, actual,new List<(object,object)>(), out var res))
            {
                throw new Exception(res);
            }
        }

        private static bool PublicStateIsValueEqual(this object target, object actual,IEnumerable<(object,object)> assumed, out string error)
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

            if (target is IEnumerable<object> leftEnum && actual is IEnumerable<object> rightEnum)
            {
                if (target is IList<object> && actual is IList<object>)
                {
                    var leftEnumor = leftEnum.GetEnumerator();
                    var rightEnumor = rightEnum.GetEnumerator();
                    var i = 0;

                    while (leftEnumor.MoveNext() && rightEnumor.MoveNext())
                    {
                        if (!PublicStateIsValueEqual(leftEnumor.Current, rightEnumor.Current, innerAssummed, out var err))
                        {
                            error = $"[{i}]{err}";
                            return false;
                        }
                        i++;
                    }
                }
                else {
                    var leftDict = leftEnum.ToDictionary(x => x, x => false);
                    var rightDict = rightEnum.ToDictionary(x => x, x => false);

                    if (leftDict.Count != rightDict.Count) {
                        error = $".Count: left {leftDict.Count}, right: {rightDict.Count}";
                        return false;
                    }

                    foreach (var leftKey in leftDict.Keys)
                    {
                        foreach (var rightKey in rightDict.Where(x=>x.Value).Select(x=>x.Key))
                        {
                            if (PublicStateIsValueEqual(leftKey, rightKey, innerAssummed, out var err))
                            {
                                leftDict[leftKey] = false;
                                rightDict[rightKey] = false;
                            }
                            else {
                                error = $"[{leftKey}]{err}";
                                return false;
                            }
                        }
                    }
                }
            }
            
            foreach (var propertyInfo in GetPropertryInfo())
            {
                if (propertyInfo.CanRead && propertyInfo.GetGetMethod().IsPublic && propertyInfo.GetIndexParameters().Count() == 0)
                {
                    var firstValue = propertyInfo.GetValue(target, null);
                    var secondValue = propertyInfo.GetValue(actual, null);
                    if (!ReferenceEquals(firstValue, target) && !ReferenceEquals(secondValue, actual) && !PublicStateIsValueEqual(firstValue, secondValue, innerAssummed, out var res))
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
                    if (!PublicStateIsValueEqual(firstValue, secondValue, innerAssummed, out var res))
                    {
                        error = $".{fieldInfo.Name}{res}";
                        return false;
                    }
                }
            }

            error = null;
            return true;

            PropertyInfo[] GetPropertryInfo()
            {
                if (target.GetType() != actual.GetType())
                {
                    return target.GetType().FindInterfaces((x, y) => true, new object()).Intersect(actual.GetType().FindInterfaces((x, y) => true, new object())).SelectMany(x => x.GetProperties()).ToArray();
                }
                else
                {
                    return target.GetType().GetProperties();
                }
            }

            FieldInfo[] GetFieldInfo()
            {
                if (target.GetType() != actual.GetType())
                {
                    return new FieldInfo[0];
                }
                else
                {
                    return target.GetType().GetFields();
                }
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
