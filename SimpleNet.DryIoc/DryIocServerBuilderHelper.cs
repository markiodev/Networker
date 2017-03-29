namespace SimpleNet.DryIoc
{
    public static class DryIocServerBuilderHelper
    {
        public static ISimpleNetServerBuilder UseDryIoc(this ISimpleNetServerBuilder builder)
        {
            return builder;
        }
    }
}