
namespace XDragonBones
{
    public interface IXAnimation
    {
        float timeScale { get; set; }

        IXAnimationState XFadeIn(string animationName, float fadeInTime = -1.0f, int playTimes = -1,
            int layer = 0, string group = null, int fadeOutMode = 3,
            bool additiveBlending = false, bool displayControl = true,
            bool pauseFadeOut = true, bool pauseFadeIn = true);

        IXAnimationState XPlay(string animationName = null, int playTimes = -1);
    }
}
