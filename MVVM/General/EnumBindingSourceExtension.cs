using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;

//EnumBindingSourceExension by Brian Lagunas
//Link to used tutorial: https://youtu.be/Bp5LFXjwtQ0

namespace MVVM.MainView
{
    public class EnumBindingSourceExtension : MarkupExtension
    {
        public Type EnumType { get; private set; }

        public EnumBindingSourceExtension(Type enumType)
        {
            if (enumType is null || !enumType.IsEnum)
            {
                throw new Exception("Enumtype must not be null and must be an Enum");
            }

            EnumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(EnumType);
        }
    }
}
