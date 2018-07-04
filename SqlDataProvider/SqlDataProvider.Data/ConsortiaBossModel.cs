using System;
namespace SqlDataProvider.Data
{
    public class ConsortiaBossModel//guild boss
    {
        public DateTime endTime
        {
            get;
            set;
        }
        public int bossState
        {
            get;
            set;
        }
        public byte extendAvailableNum
        {
            get;
            set;
        }
        public int callBossLevel
        {
            get;
            set;
        }
    }
}
