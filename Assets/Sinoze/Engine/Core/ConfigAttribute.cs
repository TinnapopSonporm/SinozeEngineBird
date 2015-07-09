using UnityEngine;
using System.Collections;
using System;

namespace Sinoze.Engine
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ConfigAttribute : Attribute
	{
		public string Name { get; private set; }
		public string Define { get; private set; }
		public Type Dependency { get; private set; }
	}
}
