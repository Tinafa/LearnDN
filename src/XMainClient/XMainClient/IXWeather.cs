using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMainClient
{
    public interface IXWeather
    {
        void TemperatureChange(int temperature);

        void HumidityChange(int humidity);
    }
}
