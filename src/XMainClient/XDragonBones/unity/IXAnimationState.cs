using System;

namespace XDragonBones
{
    public interface IXAnimationState
    {
        /**
         * @language zh_CN
         * 是否对插槽的显示对象有控制权。
         * @see DragonBones.Slot#displayController
         * @version DragonBones 3.0
         */
        bool displayControl { get; set; }
        /**
         * @language zh_CN
         * 是否以增加的方式混合。
         * @version DragonBones 3.0
         */
        bool additiveBlending { get; set; }
        /**
         * @language zh_CN
         * 是否能触发行为。
         * @version DragonBones 5.0
         */
        bool actionEnabled { get; set; }
        /**
         * @language zh_CN
         * 播放次数。 [0: 无限循环播放, [1~N]: 循环播放 N 次]
         * @version DragonBones 3.0
         */
        uint playTimes { get; set; }

        /**
         * @language zh_CN
         * 播放速度。 [(-N~0): 倒转播放, 0: 停止播放, (0~1): 慢速播放, 1: 正常播放, (1~N): 快速播放]
         * @version DragonBones 3.0
         */
        float timeScale { get; set; }
        /**
         * @language zh_CN
         * 混合权重。
         * @version DragonBones 3.0
         */
        float weight { get; set; }
        /**
         * @language zh_CN
         * 自动淡出时间。 [-1: 不自动淡出, [0~N]: 淡出时间] (以秒为单位)
         * 当设置一个大于等于 0 的值，动画状态将会在播放完成后自动淡出。
         * @version DragonBones 3.0
         */
        float autoFadeOutTime { get; set; }
        /**
         * @private
         */
        float fadeTotalTime { get; set; }


        void Play();

        void Stop();

        void FadeOut(float fadeOutTime, bool pausePlayhead = true);

        bool ContainsBoneMask(string name);

        int layer { get; }

        string group { get; }

        string name { get; }

        bool isCompleted { get; }

        bool isPlaying { get; }
    }
}
