using System.Reflection;

namespace OwlEdu_Manager_Server.Utils
{
    public static class ModelMapUtils
    {
        public static TTarget MapBetweenClasses<TSource, TTarget>(TSource source)
        where TTarget : new()
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            TTarget target = new();

            foreach (PropertyInfo sourceProp in typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                PropertyInfo? targetProp = typeof(TTarget).GetProperty(sourceProp.Name, BindingFlags.Public | BindingFlags.Instance);

                if (targetProp != null && targetProp.CanWrite)
                {
                    object? value = sourceProp.GetValue(source);

                    if (value == null)
                    {
                        targetProp.SetValue(target, null);
                    }
                    else if (targetProp.PropertyType.IsAssignableFrom(sourceProp.PropertyType))
                    {
                        targetProp.SetValue(target, value);
                    }
                    else
                    {
                        try
                        {
                            object? convertedValue = Convert.ChangeType(value, targetProp.PropertyType);
                            targetProp.SetValue(target, convertedValue);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
            }

            return target;
        }
    }
}
