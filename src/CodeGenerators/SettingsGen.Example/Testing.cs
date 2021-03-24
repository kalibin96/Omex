﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Omex.CodeGenerators.SettingsGen.Models.Attributes;

#nullable enable

namespace Microsoft.Omex.SettingsGen.Example
{
	/// <summary>
	/// 
	/// </summary>
	[Section(Name = "Renaming")]
	public class Testing
	{
		/// <summary>
		/// 
		/// </summary>
		public string Whatever { get; set; } = string.Empty;


		/// <summary>
		/// 
		/// </summary>
		[Parameter(Name = "TestingThis", Value = "Hmmmm")]
		public string? DiffName { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[Ignore]
		public int Hello { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[Parameter(IsEncrypted ="true", MustOverride = "true")]
		public string? Overriding { get; set; }
	}
}
