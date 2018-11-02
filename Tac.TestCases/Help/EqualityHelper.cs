using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Tac.Tests.Help
{
    // TODO this does not handle circular stuff very well

    public static class EqualityHelper
    {
        public static void ValueEqualOrThrow(this object target, object actual) {
            if (!PublicStateIsValueEqual(target, actual, out var res)) {
                throw new Exception(res);
            }
        }

        private static bool PublicStateIsValueEqual(this object target, object actual, out string error)
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

            //if (target.GetType().IsAssignableFrom())
            //{
            //    error = $" {nameof(target)} is of type {target.GetType()}, {nameof(actual)} is of type {actual.GetType()}";
            //    return false;
            //}

            var type = target.GetType();

            if (type.IsPrimitive)
            {
                if (!target.Equals(actual))
                {
                    error = $" {nameof(target)} is {target}, {nameof(actual)} is {actual}";
                    return false;
                }
            }
            
            if (target is IEnumerable<object> leftEnum && actual is IEnumerable<object> rightEnum) {
                var leftEnumor = leftEnum.GetEnumerator();
                var rightEnumor = rightEnum.GetEnumerator();
                var i = 0;

                while (leftEnumor.MoveNext() && rightEnumor.MoveNext()) {
                    if (!PublicStateIsValueEqual(leftEnumor.Current, rightEnumor.Current,out var err)){
                        error = $"[{i}]{err}";
                        return false;
                    }
                    i++;
                }
            }
            
            foreach (var propertyInfo in type.GetProperties())
            {
                if (propertyInfo.CanRead && propertyInfo.GetGetMethod().IsPublic && propertyInfo.GetIndexParameters().Count() == 0)
                {
                    var firstValue = propertyInfo.GetValue(target, null);
                    var secondValue = propertyInfo.GetValue(actual, null);
                    if (!ReferenceEquals(firstValue,target) && !ReferenceEquals(secondValue, actual) && !PublicStateIsValueEqual(firstValue, secondValue, out var res))
                    {
                        error = $".{propertyInfo.Name}{res}";
                        return false;
                    }
                }
            }

            foreach (var fieldInfo in type.GetFields(BindingFlags.Public))
            {
                if (fieldInfo.IsPublic)
                {
                    var firstValue = fieldInfo.GetValue(target);
                    var secondValue = fieldInfo.GetValue(actual);
                    if (!PublicStateIsValueEqual(firstValue, secondValue, out var res))
                    {
                        error = $".{fieldInfo.Name}{res}";
                        return false;
                    }
                }
            }

            error = null;
            return true;

        }
    }
}
