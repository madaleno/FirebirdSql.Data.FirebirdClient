﻿/*
 *    The contents of this file are subject to the Initial
 *    Developer's Public License Version 1.0 (the "License");
 *    you may not use this file except in compliance with the
 *    License. You may obtain a copy of the License at
 *    https://github.com/FirebirdSQL/NETProvider/blob/master/license.txt.
 *
 *    Software distributed under the License is distributed on
 *    an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either
 *    express or implied. See the License for the specific
 *    language governing rights and limitations under the License.
 *
 *    All Rights Reserved.
 */

//$Authors = Jiri Cincura (jiri@cincura.net)

using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace FirebirdSql.EntityFrameworkCore.Firebird.Query.ExpressionTranslators.Internal
{
	public class FbContainsOptimizedTranslator : IMethodCallTranslator
	{
		static readonly MethodInfo MethodInfo = typeof(string).GetRuntimeMethod(nameof(string.Contains), new[] { typeof(string) });

		public virtual Expression Translate(MethodCallExpression methodCallExpression)
		{
			if (!methodCallExpression.Method.Equals(MethodInfo))
				return null;

			var patternExpression = methodCallExpression.Arguments[0];

			var positionExpression = Expression.GreaterThan(
				new SqlFunctionExpression("POSITION", typeof(int), new[]
				{
					patternExpression,
					methodCallExpression.Object,
				}),
				Expression.Constant(0));

			return patternExpression is ConstantExpression patternConstantExpression
				? ((string)patternConstantExpression.Value)?.Length == 0
					? (Expression)Expression.Constant(true)
					: positionExpression
				: Expression.OrElse(
					positionExpression,
					Expression.Equal(patternExpression, Expression.Constant(string.Empty)));
		}
	}
}
