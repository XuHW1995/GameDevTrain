namespace DefaultNamespace
{
    /// <summary>
    /// Vector3 learn
    /// </summary>
    public class LearnVector3
    {
        private float x;
        private float y;
        private float z;

        public float Dot(LearnVector3 v1, LearnVector3 v2)
        {
            return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
        }
    }
}