﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace CSharp.TemplateEngine
{
	static class Evaluator
	{
		public static object Eval(string name, List<string> arguments, RenderModel model, RenderContext context)
		{
			Delegate func;
			if(!context.Functions.TryGetValue(name, out func))
			{
				func = BuiltInFunctions.GetFunction(name, context);
			}

			if(func == null)
			{
				throw new TemplateException("Invalid function: " + name);
			}

			var values = ConvertFuncParameters(name, func.Method, arguments, model, context);
			return func.DynamicInvoke(values);
		}

		/// <summary>
		/// Convert the arguments provided in the template to the type received by the function.
		/// </summary>
		static object[] ConvertFuncParameters(string name,
			MethodInfo function, 
			List<string> templateArguments,
			RenderModel model, 
			RenderContext context)
		{
			var funcParameters = function.GetParameters();
			var convertedValues = new object[funcParameters.Length];

			if(funcParameters.Length > 0)
			{				
				var lastIndex = funcParameters.Length - 1;
				var last = funcParameters[lastIndex];

				if(templateArguments.Count > funcParameters.Length && 
				   last.ParameterType != typeof(string[]))
				{
					throw new TemplateException(string.Format(
						"The function {0} is expecting {1} parameters and receives {2}", 
						name, funcParameters.Length, templateArguments.Count));
				}

				// If the function has one more argument, it can only be a special type:
				// either RenderContext or string[] which is like a params in C#.
				if(templateArguments.Count == funcParameters.Length - 1)
				{
					if(last.ParameterType == typeof(string[]))
					{
						convertedValues[lastIndex] = new string[0];
					}
					else if(last.ParameterType != typeof(RenderContext))
					{
						throw new TemplateException(string.Format(
							"The function {0} is expecting {1} parameters and receives {2}", 
							name, funcParameters.Length, templateArguments.Count));
					}
				}

				if(last.ParameterType == typeof(RenderContext))
				{
					convertedValues[lastIndex] = context;
				}
				// esto es como un params string[]
				else if(last.ParameterType == typeof(string[]))
				{
					convertedValues[lastIndex] = templateArguments.Skip(lastIndex).Select(
						t => ConvertToString(context,t)).ToArray();
				}	
			}

			// convert all parameters
			for(int i = 0, l = templateArguments.Count; i < l; i++)
			{
				if(convertedValues[i] != null)
				{
					break; // A special parameter has already been reached (RenderContext or string[]).
				}

				var argument = templateArguments[i];

				// If it starts with a period, it means it is a value of the model.
				var value = argument[0] == '.' ? Template.GetValue(context.RenderModel, argument) : argument;

				try
				{
					if(value != null && funcParameters[i].ParameterType != typeof(object))
					{
						convertedValues[i] = Convert.ChangeType(value, funcParameters[i].ParameterType);
					}
					else
					{
						convertedValues[i] = value;
					}
				}
				catch(Exception ex)
				{
					throw new TemplateException(string.Format(
						"Error converting parameter {0} in {1}: {2}",  i, function.Name, ex.Message));
				}
			}

			return convertedValues;
		}

		static string ConvertToString(RenderContext context, string key)
		{
			var value = key[0] == '.' ? Template.GetValue(context.RenderModel, key) : key;
			return ConvertToString(value);
		}

		public static string ConvertToString(object value)
		{
			if(value == null)
			{
				return null;
			}

			if(value is bool)
			{
				return value.ToString().ToLower();
			}

			return value.ToString();
		}
	}
}

