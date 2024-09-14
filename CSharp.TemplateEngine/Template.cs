using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CSharp.TemplateEngine
{    
	public sealed class Template : IRenderizable
	{
		private RenderContext renderContext;

		/// <summary>
		/// The path that contains the file.
		/// </summary>
		public string Path { get; set; }

		public string BaseTemplate { get; set; }

		public static bool Debug { get; set; }

		public static string TagPrefix = "{{";
		
		public static string TagSuffix = "}}";

		public Func<string, object, RenderContext, bool> RenderValue
		{
			get{ return this.RenderContext.RenderValue; }
			set { this.RenderContext.RenderValue = value; }
		}

		public CultureInfo Culture 
		{
			get { return this.RenderContext.Culture; }
			set { this.RenderContext.Culture = value; }
		}

		public IDictionary<string, Template> Templates
		{
			get { return this.RenderContext.Templates; }
		}

		internal List<IRenderizable> Items { get; set; }

		internal RenderContext RenderContext 
		{
			get
			{
				if(renderContext == null)
				{
					renderContext = new RenderContext();
				}
				return renderContext;
			}
			set { renderContext = value; }
		}

		public Template ()
		{
			this.Items = new List<IRenderizable> ();
		}

		public static Template ParseFile(string file)
		{
			return Parser.ParseFile(file);
		}

		public static Template Parse (string text)
		{
			return Parser.Parse(text);
		}

		/// <summary>
		/// Add a new template that can be referenced this way (like an include).
		/// </summary>
		public Template ParseInclude (string name, string text)
		{
			var include = Parser.Parse(text);
			include.Path = name;
			this.RenderContext.Templates[name] = include;
			return include;
		}

		public void AddFunction(string name, Delegate func)
		{
			this.RenderContext.Functions.Add(name, func);
		}

		public void AddFunction<TResult>(string name, Func<TResult> func)
		{
			this.RenderContext.Functions.Add(name, func);
		}

		public void AddFunction<T1>(string name, Func<T1, object> func)
		{
			this.RenderContext.Functions.Add(name, func);
		}

		public void AddFunction<T1, T2>(string name, Func<T1, T2, object> func)
		{
			this.RenderContext.Functions.Add(name, func);
		}

		public void AddFunction<T1, T2, T3>(string name, Func<T1, T2, T3, object> func)
		{
			this.RenderContext.Functions.Add(name, func);
		}

		public void AddFunction<T1, T2, T3, T4>(string name, Func<T1, T2, T3, T4, object> func)
		{
			this.RenderContext.Functions.Add(name, func);
		}

		public string Render()
		{
			using(var w = new StringWriter())
			{
				this.RenderContext.Writer = w;
				Render(this.RenderContext);
				return w.ToString();
			}
		}

		public string Render (object model)
		{
			this.RenderContext.Model = model;
			using(var w = new StringWriter())
			{
				this.RenderContext.Writer = w;
				Render(this.RenderContext);
				return w.ToString();
			}
		}

		public void Render(RenderContext context)
		{
			try
			{
				if(this.BaseTemplate != null)
				{
					this.ExtendFromBaseTemplate(context);
				}
				else
				{
					foreach(var item in this.Items)
					{
						item.Render(context);
					}
				}
			}
			catch(Exception ex)
			{
				if(this.Path != null)
				{
					throw new Exception(string.Format("Error at {0}: {1}", this.Path, ex.Message), ex);
				}
				else
				{
					throw;
				}
			}
		}

		internal static object GetValue (RenderModel model, string key)
		{
			var value = GetModelValue (model.Model, key, model);

			if (value == null)
			{
				// If the key is not defined in the model, search in the internal values. It can be, for example, a loop.
				value = GetModelValue (model.RenderValues, key, model);
			}

			return value;
		}

		static object GetModelValue (object model, string key, RenderModel renderModel)
		{
			if (key == ".")
			{
				return model;
			}

			var pathItems = key.Split (new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

			var value = model;

			foreach (var pathItem in pathItems)
			{
				var modelKey = ModelKey.Parse (pathItem);

				value = GetPropertyValue (value, modelKey.Name);
				if(value == null)
				{
					return null;
				}

				if(modelKey.Index != null)
				{
					string modelIndex;

					// Get the value of the index:
					// it is a literal if it is between quotes
					if(modelKey.Index[0] == '"')
					{
						modelIndex = modelKey.Index.Trim('"');
					}
					else if(modelKey.Index[0] == '\'')
					{
						modelIndex = modelKey.Index.Trim('\'');
					}
					else if(modelKey.Index[0] == '.')
					{
						var v = GetValue(renderModel, modelKey.Index);
						modelIndex = v != null ? v.ToString() : null;
					}
					// If it doesn't have a dot, also try to obtain the value, in case it is the index of an indexed property.
					else if(char.IsLetter(modelKey.Index[0]))
					{
						var v = GetValue(renderModel, modelKey.Index);
						modelIndex = v != null ? v.ToString() : null;
					}
					else
					{
						modelIndex = modelKey.Index;
					}

					if(modelIndex == null)
					{
						// TODO: Throw an exception?
						return null;
					}

					// If the index being accessed is a number.
					if(char.IsDigit(modelIndex[0]))
					{
						var iIndex = int.Parse(modelIndex);

						var list = value as IList;
						if(list != null)
						{
							value = list[iIndex];
						}
						else
						{
							throw new TemplateException(string.Format(
								"Can't access {0} by index {1} because it is not a list",
								modelKey.Name, modelKey.Index));
						}
					}
					else
					{					
						var dic = value as IDictionary;
						if(dic != null)
						{
							value = dic[modelIndex];
						}
						else
						{
							throw new TemplateException(string.Format(
								"Can't access {0} by index {1} because it is not a dictionary",
								modelKey.Name, modelKey.Index));
						}
					}
				}
			}

			return value;
		}


		/// <summary>
		/// Gets the value of the property key in the object.
		/// </summary>
		static object GetPropertyValue (object value, string propertyName)
		{			
			var dic = value as IDictionary;
			if(dic != null)
			{
				return dic[propertyName];
			}
			
			if(value == null)
			{
				//throw new SException("The property %v does not exist", key);
				return null;
			}
			
			// if it is not a dictionary, obtain the property.
			var property = value.GetType().GetProperty(propertyName);
			if(property == null)
			{
				//throw new SException("The property %v does not exist", key);
				return null;
			}
			
			return property.GetValue(value, null);
		}

		/// <summary>
		/// if it is not a dictionary, obtain the property.
		/// </summary>
		void ExtendFromBaseTemplate(RenderContext context)
		{
			var extended = context.Templates[this.BaseTemplate];
			if(extended == null)
			{
				throw new TemplateException("Base template not found: " + this.BaseTemplate);
			}

			// Prepare the virtual template from the base and the extended one.
			var generated = new Template();

			// Add all elements by reassigning the main template in the blocks.
			foreach(var item in extended.Items)
			{
				var block = item as TemplateBlock;
				if(block != null)
				{
					block.MainTemplate = generated;
				}
				generated.Items.Add(item);
			}

			// Add the blocks of the extended template.
			foreach(var block in this.Items.OfType<TemplateBlock>())
			{
				block.MainTemplate = generated;
				context.Templates.Add(block.Path, block.Body);
			}
			
			generated.Render(context);
		}
	}

	/// <summary>
	/// The data that a template uses to render itself.
	/// </summary>
	sealed class RenderModel
	{
		public object Model;

		/// <summary>
		/// Contains additional values for the operation of templates. For example.
		/// In an iteration, the current value.
		/// </summary>
		internal Map RenderValues = new Map ();
	}

	public sealed class TemplateException : Exception
	{
		public TemplateException(string message) : base(message)
		{
		}
	}
			
	/// <summary>
	/// Access key to a property.
	/// It can be just the name:.name Or an index:.customer["name"]
	/// </summary>
	sealed class ModelKey
	{
		public string Name;
		public string Index;

		public static ModelKey Parse (string value)
		{
			var key = new ModelKey ();

			// La clave puede llevar un índice: .customer["name"]
			// The key can have an index:.customer["name"]
			var items = value.Trim ().Split ('[');
			
			// the name
			key.Name = items [0];
			
			// the possible index
			if (items.Length > 1)
			{
				key.Index = items [1].TrimEnd (']');
			}

			return key;
		}
	}
}