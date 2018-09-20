using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Tac.Tests.Help
{
    public static class EqualityHelper
    {
        public static void ValueEqualOrThrow(this object left, object right) {
            if (!PublicStateIsValueEqual(left, right, out var res)) {
                throw new Exception(res);
            }
        }

        private static bool PublicStateIsValueEqual(this object left, object right, out string error)
        {
            if (left == null && right == null)
            {
                error = null;
                return true;
            }

            if (left == null)
            {
                error = $" {nameof(left)} is null, {nameof(right)} is {right}";
                return false;
            }

            if (right == null)
            {
                error = $" {nameof(left)} is {left}, {nameof(right)} is null";
                return false;
            }

            if (right.GetType() != left.GetType())
            {
                error = $" {nameof(left)} is of type {left.GetType()}, {nameof(right)} is of type {right.GetType()}";
                return false;
            }

            var type = right.GetType();

            if (type.IsPrimitive)
            {
                if (!left.Equals(right))
                {
                    error = $" {nameof(left)} is {left}, {nameof(right)} is {right}";
                    return false;
                }
            }
            
            if (left is IEnumerable<object> leftEnum && right is IEnumerable<object> rightEnum) {
                var leftEnumor = leftEnum.GetEnumerator();
                var rightEnumor = rightEnum.GetEnumerator();
                var i = 0;

                while (leftEnumor.MoveNext() && rightEnumor.MoveNext()) {
                    if (!PublicStateIsValueEqual(leftEnumor.Current, rightEnumor.Current,out var err)){
                        error = $"[{i}]{err}";
                        return false;
                    }
                }
            }
            
            foreach (var propertyInfo in type.GetProperties())
            {
                if (propertyInfo.CanRead && propertyInfo.GetGetMethod().IsPublic && propertyInfo.GetIndexParameters().Count() == 0)
                {
                    var firstValue = propertyInfo.GetValue(left, null);
                    var secondValue = propertyInfo.GetValue(right, null);
                    if (!ReferenceEquals(firstValue,left) && !ReferenceEquals(secondValue, right) && !PublicStateIsValueEqual(firstValue, secondValue, out var res))
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
                    var firstValue = fieldInfo.GetValue(left);
                    var secondValue = fieldInfo.GetValue(right);
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
