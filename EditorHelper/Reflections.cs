using HarmonyLib;
using System.Reflection;

namespace EditorHelper
{
	public static class Reflections
	{
		public static FieldInfo Field(this object obj, string fieldName)
		{
			return AccessTools.Field(obj.GetType(), fieldName);
		}

		public static PropertyInfo Property(this object obj, string propertyName)
		{
			return AccessTools.Property(obj.GetType(), propertyName);
		}

		public static MethodInfo Method(this object obj, string methodName)
		{
			return AccessTools.Method(obj.GetType(), methodName);
		}
	}
}
