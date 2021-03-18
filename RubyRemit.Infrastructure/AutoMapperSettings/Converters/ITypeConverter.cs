namespace RubyRemit.Infrastructure.AutoMapperSettings.Converters
{
    public interface ITypeConverter<TSource, TDestination>
    {
        TDestination Convert(TSource source);
    }
}