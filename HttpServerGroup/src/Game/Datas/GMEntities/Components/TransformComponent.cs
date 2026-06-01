
using System;

namespace Game.Datas.GMEntities
{

	public struct Vector2
	{
		public float x;
		public float y;
		public Vector2(float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		public static float GetDistance(ref Vector2 a, ref Vector2 b)
		{
			return MathF.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
		}
		public static Vector2 GetDirection(Vector2 a, Vector2 b)
		{
			return new Vector2(b.x - a.x, b.y - a.y);
		}

        public static Vector2 Sub(ref Vector2 lhs, ref Vector2 rhs)
        {
            return new Vector2(lhs.x - rhs.x, lhs.y - rhs.y);
        }

        public static float Len(ref Vector2 dir)
        {
            return MathF.Sqrt((dir.x * dir.x) + (dir.y * dir.y));

        }
    }
	public struct Vector3
    {
        public float x;
        public float y;
        public float z;
        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static float GetDistance(ref Vector3 a, ref Vector3 b)
        {
            return MathF.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y) + (a.z - b.z) * (a.z - b.z));
        }
        public static Vector3 GetDirection(Vector3 a, Vector3 b)
        {
            return new Vector3(b.x - a.x, b.y - a.y, b.z - a.z);
        }
    }
    public struct TransformComponent
    {
        public Vector3 pos;
        public Vector3 eulerAngles;

        public void Init(GM_PlayerEntity entity)
        {
            entity.uTransform.pos = new Vector3(0, 0, 0);
            entity.uTransform.eulerAngles = new Vector3(0, 0, 0);
        }
    }
}
