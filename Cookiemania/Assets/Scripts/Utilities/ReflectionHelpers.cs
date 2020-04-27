using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace General_Utilities
{
    public static class ReflectionHelpers
    {
        public static bool PropertiesNonNull(object propertyHolder, IEnumerable<PropertyInfo> list)
        {
            foreach (var info in list)
            {
                object b = null;
                try
                {
                    b = info.GetValue(propertyHolder);
                }
                catch
                {
                    Debug.LogWarning("failed to get value, remove deprecated properties from list");
                    continue;
                }
                if (b == null)
                {
                    return false;
                }
            }
            return true;
        }

        public static List<PropertyInfo> GetValidProperties(object objToGetPropertiesFrom)
        {
            List<PropertyInfo> infos = new List<PropertyInfo>();
            var temp = objToGetPropertiesFrom.GetType().GetProperties();
            foreach (var i in temp)
            {
                try
                {
                    object b = i.GetValue(objToGetPropertiesFrom);
                }
                catch
                {
                    Debug.Log("get value not supported for component type " + i.PropertyType);
                    continue;
                }
                infos.Add(i);
            }
            return infos;
        }

    }

}