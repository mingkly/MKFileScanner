using Android.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
#nullable enable
namespace MKFileScanner.Platforms.Android
{
    internal static class ExpressionParser
    {
        public static bool TryConvert(Expression<Func<string, bool>> expression,out string? selection,out List<string>? selectionArgs)
        {
            var sb = new StringBuilder();
            var args = new List<string>();
            try
            {
                if (expression.Body.CanReduce)
                {
                    var exp = expression.Body.Reduce();
                    ParseExpression(sb, exp, args);
                }
                else
                {
                    ParseExpression(sb, expression.Body, args);
                }
                selection=sb.ToString();
                selectionArgs=args.ToList();
                return true;
            }
            catch
            {
                selection = null;
                selectionArgs=null;
                return false;
            }
            
        }
        static void ParseExpression(StringBuilder sb, Expression expression, List<string> args, bool positive = true)
        {
            Console.WriteLine($"parse expression {expression}," +
                $"\n{expression.GetType()},{expression.Type},{expression.NodeType}");
            if (expression is MethodCallExpression method)
            {
                ParseMethodExpression(sb, args, method, positive);
            }
            else if (expression is BinaryExpression binaryExpression)
            {
                ParseBinaryExpression(sb, args, binaryExpression, positive);
            }
            else if (expression is UnaryExpression unaryExpression)
            {
                ParseUnaryExpression(sb, args, unaryExpression, positive);
            }
            else
            {
                throw new NotSupportedException($"not support this expression: {expression}");
            }
        }
        static T? ParseArgumentExpression<T>(Expression expression)
        {
            Console.WriteLine($"{expression},{expression.GetType()},{expression.Type},{expression.NodeType}");
            if (expression is ConstantExpression constantExpression)
            {
                if (constantExpression.Value != null)
                {
                    return (T)constantExpression.Value;
                }
                return default;
            }
            else if (expression.Type == typeof(T) && expression is not ParameterExpression)
            {
                return Expression.Lambda<Func<T>>(expression).Compile().Invoke();
            }
            return default;
        }
        static void ParseUnaryExpression(StringBuilder sb, List<string> args, UnaryExpression unaryExpression, bool positive = true)
        {
            positive = !positive;
            ParseExpression(sb, unaryExpression.Operand, args, positive);
        }

        static void ParseBinaryExpression(StringBuilder sb, List<string> args, BinaryExpression binary, bool positive = true)
        {
            sb.Append(" ( ");
            switch (binary.NodeType)
            {
                case ExpressionType.AndAlso:
                case ExpressionType.And:

                    ParseExpression(sb, binary.Left, args, positive);
                    sb.Append(positive ? " and " : " or ");
                    ParseExpression(sb, binary.Right, args, positive);
                    break;
                case ExpressionType.OrElse:
                case ExpressionType.Or:
                    ParseExpression(sb, binary.Left, args, positive);
                    sb.Append(positive ? " or " : " and ");
                    ParseExpression(sb, binary.Right, args, positive);
                    break;
                case ExpressionType.Equal:
                    var value = ParseArgumentExpression<string>(binary.Left) ?? ParseArgumentExpression<string>(binary.Right);
                    if (value != null)
                    {
                        sb.Append($"{MediaStore.IMediaColumns.DisplayName} {(positive ? "=" : "!=")} ? ");
                        args.Add(value);
                    }
                    break;
                case ExpressionType.NotEqual:
                    var value2 = ParseArgumentExpression<string>(binary.Left) ?? ParseArgumentExpression<string>(binary.Right);
                    if (value2 != null)
                    {
                        sb.Append($"{MediaStore.IMediaColumns.DisplayName} {(positive ? "!=" : "=")} ? ");
                        args.Add(value2);
                    }
                    break;
                default:
                    throw new NotSupportedException($"not support this expression: {binary}");
            }
            sb.Append(" ) ");
        }

        static void ParseMethodExpression(StringBuilder sb, List<string> args, MethodCallExpression method, bool positive = true)
        {
            if (method.Method.DeclaringType == typeof(string) && method.Method.Name == "StartsWith")
            {
                sb.Append($"{MediaStore.IMediaColumns.DisplayName} {(positive ? "" : "not")} like ?");
                var value = ParseArgumentExpression<string>(method.Arguments[0]);
                args.Add($"{value}%");
            }
            else if (method.Method.DeclaringType == typeof(string) && method.Method.Name == "EndsWith")
            {
                sb.Append($"{MediaStore.IMediaColumns.DisplayName} {(positive ? "" : "not")} like ?");
                var value = ParseArgumentExpression<string>(method.Arguments[0]);
                args.Add($"%{value}");
            }
            else if (method.Method.DeclaringType == typeof(string) && method.Method.Name == "Contains")
            {
                sb.Append($"{MediaStore.IMediaColumns.DisplayName} {(positive ? "" : "not")} like ?");
                var value = ParseArgumentExpression<string>(method.Arguments[0]);
                args.Add($"%{value}%");
            }
            else
            {
                throw new NotSupportedException($"not support this expression: {method}");
            }
        }
    }
}
