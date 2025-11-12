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

            foreach (var sourceProp in typeof(TSource).GetProperties())
            {
                var targetProp = typeof(TTarget).GetProperty(sourceProp.Name);
                if (targetProp != null && targetProp.CanWrite)
                {
                    var value = sourceProp.GetValue(source);
                    if (value != null && targetProp.PropertyType.IsAssignableFrom(sourceProp.PropertyType))
                        targetProp.SetValue(target, value);
                }
            }

            return target;
        }
    }
}
