using System;
using UnityEngine;

namespace XMainClient
{
    public enum EXSeason : byte
    {
        ESpring, ESummer, EAutumn, EWinter
    }

    public interface IXSeason
    {
        EXSeason SeasonType();
        XWeather GetRandomWeather();
    }

    public abstract class XSeason:IXSeason
    {
        protected EXWeather[] weathers;
        protected float[] probs;

        public static XSeason Get(EXSeason season)
        {
            switch (season)
            {
                case EXSeason.ESpring:return new XSpring();
                case EXSeason.ESummer: return new XSummer();
                case EXSeason.EAutumn: return new XAutumn();
                case EXSeason.EWinter: return new XWinter();
                default: return new XSpring();
            }
        }

        public abstract EXSeason SeasonType();

        public virtual EXWeather[] GetWeathers() { return weathers; }

        public virtual float[] GetProb() { return probs; }

        private EXWeather GetRandomWeatherEnum()
        {
            float sum = 0f;
            int i = 0;
            for(;i<probs.Length;++i)
            {
                sum += probs[i];
            }
            sum *= 1000;
            int ret = UnityEngine.Random.Range(0, Mathf.RoundToInt(sum));
            sum = 0;
            for (i=0;i<probs.Length;++i)
            {
                sum += probs[i];
                if(ret < sum)
                {
                    break;
                }
            }
            if (i < weathers.Length)
            {
                return weathers[i];
            }
            return EXWeather.ESunny;
        }

        public XWeather GetRandomWeather()
        {
            XWeather weather = null;

            EXWeather e = GetRandomWeatherEnum();
            switch(e)
            {
                case EXWeather.ESunny:weather = new XSunny(SeasonType());break;
                case EXWeather.ECloudy:weather = new XCloudy(SeasonType()); break;
                case EXWeather.EFog:weather = new XFog(SeasonType());break;
                case EXWeather.ERainy:weather = new XRainy(SeasonType()); break;
                case EXWeather.EStorm:weather = new XStorm(SeasonType());break;
                case EXWeather.EHail:weather = new XHail(SeasonType());break;
                case EXWeather.ESnowy:weather = new XSnowy(SeasonType());break;
                case EXWeather.EWindy:weather = new XWindy(SeasonType());break;
            }

            return weather;
        }

    }

    public class XSpring : XSeason
    {
        public XSpring()
        {
            weathers = new EXWeather[4];
            weathers[0] = EXWeather.ESunny;
            weathers[1] = EXWeather.ECloudy;
            weathers[2] = EXWeather.ERainy;
            weathers[3] = EXWeather.EWindy;

            probs = new float[4];
            probs[0] = 0.3f;
            probs[1] = 0.2f;
            probs[2] = 0.3f;
            probs[3] = 0.2f;
        }

        public override EXSeason SeasonType()
        {
            return EXSeason.ESpring;
        }

    }

    public class XSummer : XSeason
    {
        public XSummer()
        {
            weathers = new EXWeather[6];
            weathers[0] = EXWeather.ESunny;
            weathers[1] = EXWeather.ECloudy;
            weathers[2] = EXWeather.ERainy;
            weathers[3] = EXWeather.EStorm;
            weathers[4] = EXWeather.EHail;
            weathers[5] = EXWeather.EWindy;

            probs = new float[6];
            probs[0] = 0.3f;
            probs[1] = 0.2f;
            probs[2] = 0.3f;
            probs[3] = 0.2f;
            probs[4] = 0.05f;
            probs[5] = 0.1f;
        }

        public override EXSeason SeasonType()
        {
            return EXSeason.ESummer;
        }
    }

    public class XAutumn : XSeason
    {
        public XAutumn()
        {
            weathers = new EXWeather[6];
            weathers[0] = EXWeather.ESunny;
            weathers[1] = EXWeather.ECloudy;
            weathers[2] = EXWeather.ERainy;
            weathers[3] = EXWeather.EStorm;
            weathers[4] = EXWeather.EWindy;
            weathers[5] = EXWeather.EFog;

            probs = new float[6];
            probs[0] = 0.3f;
            probs[1] = 0.2f;
            probs[2] = 0.2f;
            probs[3] = 0.05f;
            probs[4] = 0.1f;
            probs[5] = 0.1f;
        }

        public override EXSeason SeasonType()
        {
            return EXSeason.EAutumn;
        }
    }

    public class XWinter : XSeason
    {
        public XWinter()
        {
            weathers = new EXWeather[6];
            weathers[0] = EXWeather.ESunny;
            weathers[1] = EXWeather.ECloudy;
            weathers[2] = EXWeather.ERainy;
            weathers[3] = EXWeather.ESnowy;
            weathers[4] = EXWeather.EWindy;
            weathers[5] = EXWeather.EFog;

            probs = new float[6];
            probs[0] = 0.04f;
            probs[1] = 0.1f;
            probs[2] = 0.1f;
            probs[3] = 0.55f;
            probs[4] = 0.2f;
            probs[5] = 0.3f;
        }
        public override EXSeason SeasonType()
        {
            return EXSeason.EWinter;
        }
    }

}
