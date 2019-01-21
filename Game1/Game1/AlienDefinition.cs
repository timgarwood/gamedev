namespace Game1
{
    public class AlienDefinition
    {
        public AlienDefinition()
        {
        }

        public string Name { get; set; }

        public string TextureName { get; set; }

        public int Hp { get; set; }

        public float Scale { get; set; }

        public float Friction { get; set; }

        public float Density { get; set; }

        public float AlienTurnVelocityDecrement { get; set; }

        public float MaxSpeed { get; set; }
        
        public float MoveImpulse { get; set; }

        public float DecisionFrequencySec { get; set; }
    }
}
