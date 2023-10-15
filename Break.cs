

namespace TurnitTest
{
    public class Break 
    {
        public int startTime {  get; set; }
        public int endTime { get; set; }

        public Break (int startTime, int endTime)
        {
            this.startTime = startTime;
            this.endTime = endTime;
        }

        public Break()
        {
        }
    }   
}
