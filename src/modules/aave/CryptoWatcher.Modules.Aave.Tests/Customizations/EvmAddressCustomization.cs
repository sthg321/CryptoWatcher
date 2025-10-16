using AutoFixture;
using AutoFixture.Kernel;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.AaveModule.Tests.Customizations;

public class EvmAddressCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customizations.Add(new EvmAddressGenerator());
    }

    private class EvmAddressGenerator : ISpecimenBuilder
    {
        private int _counter;
        
        public object Create(object request, ISpecimenContext context)
        {
            if (request is not Type type ||  type !=  typeof(EvmAddress))
            {
                return new NoSpecimen();
            }
            
            var address = $"0x{_counter++.ToString("X").PadLeft(40, '0')}";
            return EvmAddress.Create(address);
        }
    }
}