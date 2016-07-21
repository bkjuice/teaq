
namespace Teaq.Tests.FastReflection.SupportingTypes
{
    public class GenericMethodExample
    {
        public string NonGenericMethod(string value)
        {
            return value;
        }

        public string GenericMethod<T>(T value)
        {
            return value.ToString();
        }
    }
}
