using System;
using Random = UnityEngine.Random;

namespace XMainClient
{
    public enum EXWeather : byte
    {
        ESunny, ECloudy, EFog, ERainy, EStorm, EHail, ESnowy, EWindy
    }

    public abstract class XWeather
    {
        public abstract EXWeather WeatherType();
        public int WeatherIdx { get; protected set; }

        public EXSeason SeasonType { get; protected set; }

        public int SeasonIdx { get; protected set; }

        public abstract bool HasSunMoon();

        public float Temprature { get; set; }

        public float Humidity { get; set; }

        public float Illumination { get; set; }

        public float Rainfall { get; set; }
    }

    public class XSunny : XWeather
    {
        public XSunny(EXSeason season)
        {
            switch (season)
            {
                case EXSeason.ESpring:
                     {
                        Temprature = Random.Range(16.0f, 25.0f);
                        Humidity = Random.Range(0.45f,0.60f);
                        Illumination = Random.Range(50f,60f);
                        Rainfall = 0f;
                     }
                     break;
                case EXSeason.ESummer:
                     {
                        Temprature = Random.Range(35.0f, 40.0f);
                        Humidity = Random.Range(0.45f, 0.50f);
                        Illumination = Random.Range(70f, 100f);
                        Rainfall = 0f;
                    }
                    break;
                case EXSeason.EAutumn:
                     {
                        Temprature = Random.Range(10.0f, 18.0f);
                        Humidity = Random.Range(0.35f, 0.45f);
                        Illumination = Random.Range(50f, 60f);
                        Rainfall = 0f;
                    }
                     break;
                case EXSeason.EWinter:
                     {
                        Temprature = Random.Range(6.0f, 10.0f);
                        Humidity = Random.Range(0.25f, 0.35f);
                        Illumination = Random.Range(40f, 50f);
                        Rainfall = 0f;
                    }
                     break;
            }

            SeasonType = season;
            SeasonIdx = (int)season;
            WeatherIdx = (int)EXWeather.ESunny;
        }

        public override EXWeather WeatherType()
        {
            return EXWeather.ESunny;
        }
   
        public override bool HasSunMoon() { return true; }
    }

    public class XCloudy : XWeather
    {
        public XCloudy(EXSeason season)
        {
            switch (season)
            {
                case EXSeason.ESpring:
                    {
                        Temprature = Random.Range(14.0f, 18.0f);
                        Humidity = Random.Range(0.5f, 0.70f);
                        Illumination = Random.Range(40f, 50f);
                        Rainfall = 0f;
                    }
                    break;
                case EXSeason.ESummer:
                    {
                        Temprature = Random.Range(30.0f, 36.0f);
                        Humidity = Random.Range(0.45f, 0.50f);
                        Illumination = Random.Range(50f, 80f);
                        Rainfall = 0f;
                    }
                    break;
                case EXSeason.EAutumn:
                    {
                        Temprature = Random.Range(11.0f, 17.0f);
                        Humidity = Random.Range(0.4f, 0.4f);
                        Illumination = Random.Range(40f, 50f);
                        Rainfall = 0f;
                    }
                    break;
                case EXSeason.EWinter:
                    {
                        Temprature = Random.Range(6.0f, 10.0f);
                        Humidity = Random.Range(0.25f, 0.35f);
                        Illumination = Random.Range(30f, 45f);
                        Rainfall = 0f;
                    }
                    break;
            }

            SeasonType = season;
            SeasonIdx = (int)season;
            WeatherIdx = (int)EXWeather.ECloudy;
        }

        public override EXWeather WeatherType()
        {
            return EXWeather.ECloudy;
        }

        public override bool HasSunMoon() { return false; }
    }

    public class XFog : XWeather
    {
        public XFog(EXSeason season)
        {
            switch (season)
            {
                case EXSeason.ESpring:
                    {
                        Temprature = Random.Range(15.0f, 19.0f);
                        Humidity = Random.Range(0.60f, 0.80f);
                        Illumination = Random.Range(40f, 50f);
                        Rainfall = 0f;
                    }
                    break;
                case EXSeason.ESummer:
                    {
                        Temprature = Random.Range(34.0f, 39.0f);
                        Humidity = Random.Range(0.47f, 0.53f);
                        Illumination = Random.Range(65f, 95f);
                        Rainfall = 0f;
                    }
                    break;
                case EXSeason.EAutumn:
                    {
                        Temprature = Random.Range(10.0f, 18.0f);
                        Humidity = Random.Range(0.45f, 0.55f);
                        Illumination = Random.Range(50f, 60f);
                        Rainfall = 0f;
                    }
                    break;
                case EXSeason.EWinter:
                    {
                        Temprature = Random.Range(-2.0f, 5.0f);
                        Humidity = Random.Range(0.45f, 0.55f);
                        Illumination = Random.Range(40f, 50f);
                        Rainfall = 0f;
                    }
                    break;
            }
            SeasonType = season;
            SeasonIdx = (int)season;
            WeatherIdx = (int)EXWeather.EFog;
        }

        public override EXWeather WeatherType()
        {
            return EXWeather.EFog;
        }

        public override bool HasSunMoon() { return true; }
    }

    public class XRainy : XWeather
    {
        public XRainy(EXSeason season)
        {
            switch (season)
            {
                case EXSeason.ESpring:
                    {
                        Temprature = Random.Range(14.0f, 17.0f);
                        Humidity = Random.Range(0.55f, 0.80f);
                        Illumination = Random.Range(30f, 40f);
                        Rainfall = 40f;
                    }
                    break;
                case EXSeason.ESummer:
                    {
                        Temprature = Random.Range(26.0f, 30.0f);
                        Humidity = Random.Range(0.50f, 0.55f);
                        Illumination = Random.Range(34f, 40f);
                        Rainfall = 70f;
                    }
                    break;
                case EXSeason.EAutumn:
                    {
                        Temprature = Random.Range(10.0f, 16.0f);
                        Humidity = Random.Range(0.35f, 0.45f);
                        Illumination = Random.Range(45f, 50f);
                        Rainfall = 50f;
                    }
                    break;
                case EXSeason.EWinter:
                    {
                        Temprature = Random.Range(-5.0f, 5.0f);
                        Humidity = Random.Range(0.45f, 0.55f);
                        Illumination = Random.Range(30f, 40f);
                        Rainfall = 40f;
                    }
                    break;
            }
            SeasonType = season;
            SeasonIdx = (int)season;
            WeatherIdx = (int)EXWeather.ERainy;
        }

        public override EXWeather WeatherType()
        {
            return EXWeather.ERainy;
        }

        public override bool HasSunMoon() { return false; }
    }

    public class XStorm : XWeather
    {
        public XStorm(EXSeason season)
        {
            switch (season)
            {
                case EXSeason.ESpring:
                    {
                        Temprature = Random.Range(14.0f, 17.0f);
                        Humidity = Random.Range(0.65f, 0.80f);
                        Illumination = Random.Range(30f, 40f);
                        Rainfall = 60f;
                    }
                    break;
                case EXSeason.ESummer:
                    {
                        Temprature = Random.Range(27.0f, 33.0f);
                        Humidity = Random.Range(0.45f, 0.60f);
                        Illumination = Random.Range(70f, 100f);
                        Rainfall = 100f;
                    }
                    break;
                case EXSeason.EAutumn:
                    {
                        Temprature = Random.Range(10.0f, 16.0f);
                        Humidity = Random.Range(0.35f, 0.55f);
                        Illumination = Random.Range(50f, 60f);
                        Rainfall = 60f;
                    }
                    break;
                case EXSeason.EWinter:
                    {
                        Temprature = Random.Range(-16.0f, -5.0f);
                        Humidity = Random.Range(0.35f, 0.45f);
                        Illumination = Random.Range(40f, 50f);
                        Rainfall = 40f;
                    }
                    break;
            }
            SeasonType = season;
            SeasonIdx = (int)season;
            WeatherIdx = (int)EXWeather.EStorm;
        }

        public override EXWeather WeatherType()
        {
            return EXWeather.EStorm;
        }

        public override bool HasSunMoon() { return false; }
    }

    public class XHail : XWeather
    {
        public XHail(EXSeason season)
        {
            switch (season)
            {
                case EXSeason.ESpring:
                    {
                        Temprature = Random.Range(16.0f, 20.0f);
                        Humidity = Random.Range(0.45f, 0.60f);
                        Illumination = Random.Range(50f, 60f);
                        Rainfall = 20f;
                    }
                    break;
                case EXSeason.ESummer:
                    {
                        Temprature = Random.Range(35.0f, 40.0f);
                        Humidity = Random.Range(0.45f, 0.50f);
                        Illumination = Random.Range(70f, 100f);
                        Rainfall = 30f;
                    }
                    break;
                case EXSeason.EAutumn:
                    {
                        Temprature = Random.Range(10.0f, 16.0f);
                        Humidity = Random.Range(0.35f, 0.45f);
                        Illumination = Random.Range(40f, 50f);
                        Rainfall = 20f;
                    }
                    break;
                case EXSeason.EWinter:
                    {
                        Temprature = Random.Range(-5.0f, 0.0f);
                        Humidity = Random.Range(0.25f, 0.35f);
                        Illumination = Random.Range(35f, 40f);
                        Rainfall = 10f;
                    }
                    break;
            }
            SeasonType = season;
            SeasonIdx = (int)season;
            WeatherIdx = (int)EXWeather.EHail;
        }

        public override EXWeather WeatherType()
        {
            return EXWeather.EHail;
        }

        public override bool HasSunMoon() { return false; }
    }

    public class XSnowy : XWeather
    {
        public XSnowy(EXSeason season)
        {
            switch (season)
            {
                case EXSeason.ESpring:
                    {
                       Temprature = Random.Range(16.0f, 20.0f);
                        Humidity = Random.Range(0.45f, 0.60f);
                        Illumination = Random.Range(50f, 60f);
                        Rainfall = 10f;
                    }
                    break;
                case EXSeason.ESummer:
                    {
                        Temprature = Random.Range(35.0f, 40.0f);
                        Humidity = Random.Range(0.45f, 0.50f);
                        Illumination = Random.Range(70f, 100f);
                        Rainfall = 10f;
                    }
                    break;
                case EXSeason.EAutumn:
                    {
                        Temprature = Random.Range(10.0f, 18.0f);
                        Humidity = Random.Range(0.35f, 0.45f);
                        Illumination = Random.Range(30f, 50f);
                        Rainfall = 50f;
                    }
                    break;
                case EXSeason.EWinter:
                    {
                        Temprature = Random.Range(-15.0f, -3.0f);
                        Humidity = Random.Range(0.25f, 0.35f);
                        Illumination = Random.Range(20f, 30f);
                        Rainfall = 70f;
                    }
                    break;
            }
            SeasonType = season;
            SeasonIdx = (int)season;
            WeatherIdx = (int)EXWeather.ESnowy;
        }

        public override EXWeather WeatherType()
        {
            return EXWeather.ESnowy;
        }

        public override bool HasSunMoon() { return true; }
    }

    public class XWindy : XWeather
    {
        public XWindy(EXSeason season)
        {
            switch (season)
            {
                case EXSeason.ESpring:
                    {
                        Temprature = Random.Range(8.0f, 15.0f);
                        Humidity = Random.Range(0.45f, 0.60f);
                        Illumination = Random.Range(30f, 50f);
                        Rainfall = 0f;
                    }
                    break;
                case EXSeason.ESummer:
                    {
                        Temprature = Random.Range(20.0f, 30.0f);
                        Humidity = Random.Range(0.45f, 0.50f);
                        Illumination = Random.Range(70f, 80f);
                        Rainfall = 0f;
                    }
                    break;
                case EXSeason.EAutumn:
                    {
                        Temprature = Random.Range(5.0f, 14.0f);
                        Humidity = Random.Range(0.35f, 0.45f);
                        Illumination = Random.Range(40f, 50f);
                        Rainfall = 0f;
                    }
                    break;
                case EXSeason.EWinter:
                    {
                        Temprature = Random.Range(-5.0f, 1.0f);
                        Humidity = Random.Range(0.25f, 0.35f);
                        Illumination = Random.Range(30f, 40f);
                        Rainfall = 0f;
                    }
                    break;
            }
            SeasonType = season;
            SeasonIdx = (int)season;
            WeatherIdx = (int)EXWeather.EWindy;
        }

        public override EXWeather WeatherType()
        {
            return EXWeather.EWindy;
        }

        public override bool HasSunMoon() { return true; }
    }
}
