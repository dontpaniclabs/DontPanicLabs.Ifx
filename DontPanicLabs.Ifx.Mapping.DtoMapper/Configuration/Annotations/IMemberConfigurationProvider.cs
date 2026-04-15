namespace DontPanicLabs.Ifx.Mapping.DtoMapper.Configuration;

public interface IMemberConfigurationProvider
{
    void ApplyConfiguration(IMemberConfigurationExpression memberConfigurationExpression);
}